using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class FileManagerModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel(string moduleName)
        {
            var viewModel = new PageViewModel();
            viewModel.PluginClient = new PluginsClient();

            var allowedExtensions = Request.Query[WebConstants.QueryStringKeys.FileManagerAllowedExtensions].ToString();
            var allowChooseMultiple = Request.Query[WebConstants.QueryStringKeys.FileManagerAllowChooseMultiple].ToString().ToLower().ToBooleanValue();
            var onSelectedFilesChooseClientCallback = Request.Query[WebConstants.QueryStringKeys.FileManagerOnSelectedFilesChooseClientCallback].ToString();

            viewModel.UrlGetFiles = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Files, new { moduleName });
            viewModel.UrlUpload = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Upload, new { moduleName });
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Delete, new { moduleName });

            viewModel.OnSelectedFilesChooseClientCallback = onSelectedFilesChooseClientCallback;

            viewModel.FileManager = new PageViewModel.FileManagerPartialViewModel();
            viewModel.FileManager.ModuleName = moduleName;
            viewModel.FileManager.AllowedExtensions = string.IsNullOrWhiteSpace(allowedExtensions) ? new string[0] : allowedExtensions.Split(',');
            viewModel.FileManager.AllowChooseMultiple = allowChooseMultiple;


            return viewModel;
        }

        public async Task<AjaxResponse> GetFiles(string moduleName)
        {
            var viewModel = new AjaxResponse();
            var fileStorageModule = FileStorageManager.Modules[moduleName];

            var files = await FileStorage.GetFiles(folderPath: fileStorageModule.FolderName);
            var fileManagerItems = files
            .OrderByDescending(item => item.FileDateCreated)
            .Select((Item, Index) => new FileManagerItem
            {
                Name = Item.Filename,
                Size = Item.FilesizeBytes,
                SizeString = Utilities.FormatFileSizeBytes(Item.FilesizeBytes),
                UrlDownload = FileStorage.GetUploadedFileHttpPath(filename: Item.Filename, folderPath: fileStorageModule.FolderName),
                UrlThumbnail = GetUrlThumbnail(Item.Filename, fileStorageModule.ThumbnailFolderPath),
                DateModified = Item.FileDateUpdated,
                DateModifiedString = Utilities.FormatDateTime(Item.FileDateUpdated),
                IsLastItem = Index == files.Count - 1
            }).ToList();

            viewModel.IsSuccess = true;
            viewModel.Data = fileManagerItems.ToJson();

            return viewModel;
        }
        string GetUrlThumbnail(string filename, string thumbnailFolderPath)
        {
            string urlThumbnail;
            var ext = Path.GetExtension(filename);
            var isImage = ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".png";
            if (isImage)
            {
                urlThumbnail = FileStorage.GetUploadedFileHttpPath(filename: ThumbnailGenerator.GetThumbnailnameFromFilename(filename), folderPath: thumbnailFolderPath);
            }
            else
            {
                switch (ext)
                {
                    case ".doc":
                    case ".docx":
                        {
                            urlThumbnail = "/images/icons/filemanager/word.svg";
                            break;
                        }
                    case ".xls":
                    case ".xlsx":
                        {
                            urlThumbnail = "/images/icons/filemanager/excel.svg";
                            break;
                        }
                    case ".ppt":
                    case ".pptx":
                        {
                            urlThumbnail = "/images/icons/filemanager/powerpoint.svg";
                            break;
                        }
                    case ".pdf":
                        {
                            urlThumbnail = "/images/icons/filemanager/adobe_reader.svg";
                            break;
                        }
                    default:
                        {
                            urlThumbnail = "/images/icons/filemanager/file.svg";
                            break;
                        }
                }
            }
            return urlThumbnail;
        }

        public async Task<AjaxResponse> UploadFile(string moduleName)
        {
            var viewModel = new AjaxResponse();
            var fileManagerModule = FileStorageManager.Modules[moduleName];

            var postedFile = Request.Form.Files[0];
            var filename = Request.Form["Filename"].ToString();
            var chunkIndex = Request.Form["ChunkIndex"].ToString().ToInt();
            var chunkCount = Request.Form["ChunkCount"].ToString().ToInt();

            var tempFilePath = Path.Combine(path1: AppSettings.WebRootPath, path2: filename);
            using (var tempFile = File.Open(path: tempFilePath, mode: FileMode.Append))
            {
                await postedFile.CopyToAsync(tempFile);
            }

            if (chunkIndex == chunkCount - 1)
            {
                await FileStorage.SaveUploadedFile(sourceFilePhysicalPath: tempFilePath, filename: filename, folderPath: fileManagerModule.FolderName);

                var tg = new ThumbnailGenerator(tempFilePath);
                var thumbnailBytes = await tg.GetThumbnail(128, 128);
                if (thumbnailBytes != null)
                {
                    var thumbnailFilename = tg.GetThumbnailFilename();
                    await FileStorage.SaveUploadedFile(sourceFileBytes: thumbnailBytes, filename: thumbnailFilename, folderPath: fileManagerModule.ThumbnailFolderPath);
                }

                File.Delete(tempFilePath);
            }

            viewModel.IsSuccess = true;
            return viewModel;
        }

        public async Task<AjaxResponse> DeleteFile(string moduleName, string filename)
        {
            var viewModel = new AjaxResponse();
            var fileManagerModule = FileStorageManager.Modules[moduleName];

            var tg = new ThumbnailGenerator(filename);
            var thumbnailFilename = tg.GetThumbnailFilename();

            await FileStorage.DeleteFile(filename: filename, folderPath: fileManagerModule.FolderName);
            await FileStorage.DeleteFile(filename: thumbnailFilename, folderPath: fileManagerModule.ThumbnailFolderPath);

            viewModel.IsSuccess = true;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public PluginsClient PluginClient { get; set; }
            public string UrlGetFiles { get; set; }
            public string UrlUpload { get; set; }
            public string UrlDelete { get; set; }
            public string OnSelectedFilesChooseClientCallback { get; set; }
            public bool HasOnSelectedFilesChooseClientCallback => !string.IsNullOrWhiteSpace(OnSelectedFilesChooseClientCallback);
            public FileManagerPartialViewModel FileManager { get; set; }
            #endregion

            #region Nested Classes
            public class FileManagerPartialViewModel
            {
                #region Properties
                public string ModuleName { get; set; }
                public bool AllowChooseMultiple { get; set; }
                public string[] AllowedExtensions { get; set; }
                #endregion

                #region Methods
                public FileManagerBuilder Render(IHtmlHelper Html)
                {
                    return Html.DevExtreme().FileManager()
                        .OnInitialized("fileManagerModel.onInitialized")
                        .CustomizeThumbnail("fileManagerModel.customizeThumbnail")
                        .AllowedFileExtensions(AllowedExtensions)
                        .FileSystemProvider(provider =>
                            provider.Custom()
                            .UploadFileChunk("fileManagerModel.uploadFileChunk")
                            .GetItems("fileManagerModel.getFiles")
                            .DeleteItem("fileManagerModel.deleteFile")
                            .DownloadItems("fileManagerModel.downloadFile")
                        )
                        .Toolbar(toolbar =>
                        {
                            toolbar.Items(items =>
                            {
                                items.Add().Name(FileManagerToolbarItem.Upload);
                                items.Add().Name(FileManagerToolbarItem.Refresh);
                            });

                            toolbar.FileSelectionItems(items =>
                            {
                                items.Add().Name(FileManagerToolbarItem.Download);
                                items.Add().Name(FileManagerToolbarItem.Separator);
                                items.Add().Name(FileManagerToolbarItem.Delete);

                                items.Add()
                                    .Widget(Widget => Widget.Menu()
                                        .Items(MenuItems =>
                                        {
                                            MenuItems.Add().Text("Get Url").Icon("link").Option("commandName", "getLinkButton");
                                            MenuItems.Add().Text("Choose").Icon("check").Option("commandName", "choosePictureButton");
                                        })
                                        .OnItemClick("fileManagerModel.onCustomButtonClick"))
                                        .Location(ToolbarItemLocation.Before);

                                items.Add().Name(FileManagerToolbarItem.ClearSelection);
                            });
                        })
                        .SelectionMode(AllowChooseMultiple ? FileManagerSelectionMode.Multiple : FileManagerSelectionMode.Single)
                        .Permissions(permissions =>
                        {
                            permissions.Upload(true);
                            permissions.Delete(true);
                            permissions.Download(true);
                        })
                        .Permissions(permissions =>
                        {
                            permissions.Upload(true);
                            permissions.Delete(true);
                            permissions.Download(true);
                        })
                        .ItemView(itemView =>
                        {
                            itemView.ShowFolders(false);
                            itemView.ShowParentFolder(false);
                            itemView.Mode(FileManagerItemViewMode.Thumbnails);
                        });

                }
                #endregion
            }
            #endregion
        }

        public class FileManagerItem
        {
            #region Properties
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("id")]
            public int? ID { get; set; }

            [JsonProperty("isDirectory")]
            public bool IsDirectory { get; set; }

            [JsonProperty("size")]
            public long? Size { get; set; }

            [JsonProperty("sizeString")]
            public string SizeString { get; set; }

            [JsonProperty("urlDownload")]
            public string UrlDownload { get; set; }

            [JsonProperty("urlThumbnail")]
            public string UrlThumbnail { get; set; }

            [JsonProperty("dateModified")]
            public DateTime? DateModified { get; set; }

            [JsonProperty("dateModifiedString")]
            public string DateModifiedString { get; set; }

            [JsonProperty("isLastItem")]
            public bool IsLastItem { get; set; }

            [JsonProperty("items")]
            public List<FileManagerItem> Children { get; set; }
            #endregion
        }
        #endregion
    }
}
