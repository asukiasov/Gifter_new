using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using DevExtreme.AspNet.Mvc.Factories;
using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.IO;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class FileManagerModel : WebProjectModelBase
    {
        #region Properties
        public const string FileManagerFolderName = "FileManager";
        #endregion

        #region Methods
        public PageViewModel GetPageViewModel(string FolderVirtualPathHash, string FolderPhysicalPathHash, bool AllowSelectMultiple, string AllowedExtensions, string OnSelectedFilesChooseClientCallback, string Opener)
        {
            var ViewModel = new PageViewModel();
            ViewModel.PluginClient = new PluginsClient();
            ViewModel.IsTinyMce = (Opener?.ToLower() == "tinymce").ToString().ToLower();
            ViewModel.UrlUploadFiles = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.FilesUpload, new { FolderVirtualPathHash, FolderPhysicalPathHash, AllowSelectMultiple, AllowedExtensions });
            ViewModel.OnSelectedFilesChooseClientCallback = OnSelectedFilesChooseClientCallback;
            ViewModel.FileManager = new PageViewModel.FileManagerPartialViewModel();

            ViewModel.FileManager.FolderVirtualPath = FolderVirtualPathHash.AESDecryptString();
            ViewModel.FileManager.FolderPhysicalPath = FolderPhysicalPathHash.AESDecryptString();
            ViewModel.FileManager.AllowedExtensions = AllowedExtensions == null ? new string[0] : AllowedExtensions.Split(',');
            ViewModel.FileManager.AllowSelectMultiple = AllowSelectMultiple;
            ViewModel.FileManager.UrlListFiles = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Files, new { FolderVirtualPathHash, FolderPhysicalPathHash, AllowSelectMultiple, AllowedExtensions });

            ViewModel.PluginClient.EnableJQuery(true).EnableDevextreme(true).EnableFontAwesome(true).EnableFancybox(true);
            return ViewModel;
        }

        public object GetFileManagerResult(HttpRequest Request, FileSystemCommand Command, string Arguments, string FolderVirtualPathHash, string FolderPhysicalPathHash, string AllowedExtensions, bool AllowSelectMultiple)
        {
            var FileManagerPartialViewModel = new PageViewModel.FileManagerPartialViewModel();
            FileManagerPartialViewModel.AllowSelectMultiple = AllowSelectMultiple;
            var FileManagerFolderPhysicalPath = FolderPhysicalPathHash.AESDecryptString();
            var FileManagerFolderVirtualPath = FolderVirtualPathHash.AESDecryptString();
            if (!Directory.Exists(FileManagerFolderPhysicalPath))
            {
                Directory.CreateDirectory(FileManagerFolderPhysicalPath);
            }
            var Config = new FileSystemConfiguration();
            Config.Request = Request;

            Config.FileSystemProvider = new PhysicalFileSystemProvider(
                FileManagerFolderPhysicalPath,
                (FileSystemItem, ClientItem) =>
                {
                    if (!ClientItem.IsDirectory)
                    {
                        ClientItem.Thumbnail = $"{WebsiteDomain}{FileManagerFolderVirtualPath}{FileSystemItem.Name}";
                        ClientItem.CustomFields["url"] = $"{WebsiteDomain}{FileManagerFolderVirtualPath}{FileSystemItem.Name}";
                        if (Utilities.IsImage(FileSystemItem.Name))
                        {
                            ClientItem.CustomFields["thumbnailUrl"] = $"{WebsiteDomain}{FileManagerFolderVirtualPath}{FileSystemItem.Name}?width=100";
                        }
                    }
                }
            );
            Config.AllowedFileExtensions = AllowedExtensions == null ? new string[0] : AllowedExtensions.Split(',');

            //uncomment the code below to enable file/directory management
            //Config.AllowCopy = true;
            //Config.AllowCreate = true;
            //Config.AllowMove = true;

            //Config.FileSystemProvider = new PhysicalFileSystemProvider(FileManagerFolderPhysicalPath, ThumbnailGenerator.AssignThumbnailUrl);


            Config.AllowDelete = true;
            Config.AllowRename = true;
            Config.AllowUpload = true;
            Config.AllowDownload = true;

            var Processor = new FileSystemCommandProcessor(Config);
            var Result = Processor.Execute(Command, Arguments);

            return Result.GetClientCommandResult();
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public PluginsClient PluginClient { get; set; }
            public string UrlUploadFiles { get; set; }
            public string OnSelectedFilesChooseClientCallback { get; set; }
            public bool HasOnSelectedFilesChooseClientCallback => !string.IsNullOrWhiteSpace(OnSelectedFilesChooseClientCallback);
            public string IsTinyMce { get; set; }
            public FileManagerPartialViewModel FileManager { get; set; }
            #endregion

            #region Nested Classes
            public class FileManagerPartialViewModel
            {
                #region Properties
                public string UrlListFiles { get; set; }
                public string FolderVirtualPath { get; set; }
                public string FolderPhysicalPath { get; set; }
                public bool AllowSelectMultiple { get; set; }
                public string[] AllowedExtensions { get; set; }
                #endregion

                #region Methods
                public FileManagerBuilder Render(IHtmlHelper Html)
                {
                    return Html.DevExtreme().FileManager()
                        .OnInitialized("FileManagerModel.OnInitialized")
                        .CurrentPath(FolderPhysicalPath)
                        .Toolbar(Toolbar =>
                        {
                            Toolbar.Items(Items =>
                            {
                                Items.Add().Name(FileManagerToolbarItem.Upload);
                                Items.Add().Name(FileManagerToolbarItem.Refresh);
                            });

                            Toolbar.FileSelectionItems(Items => {
                                Items.Add().Name(FileManagerToolbarItem.Download);
                                Items.Add().Name(FileManagerToolbarItem.Separator);
                                Items.Add().Name(FileManagerToolbarItem.Rename);
                                Items.Add().Name(FileManagerToolbarItem.Separator);
                                Items.Add().Name(FileManagerToolbarItem.Delete);
                                Items.Add().Name(FileManagerToolbarItem.Separator);

                                Items.Add()
                                    .Widget(Widget => Widget.Menu()
                                        .Items(MenuItems => {
                                            MenuItems.Add().Text("Get Url").Icon("link").Option("commandName", "GetLinkButton");
                                            MenuItems.Add().Text("Choose").Icon("check").Option("commandName", "ChoosePictureButton");
                                        })
                                        .OnItemClick("FileManagerModel.OnFileManagerCustomCommand"))
                                        .Location(ToolbarItemLocation.Before);

                                Items.Add().Name(FileManagerToolbarItem.ClearSelection);
                            });
                        })
                        .AllowedFileExtensions(AllowedExtensions)
                        .FileSystemProvider(Provider => Provider.Remote().Url(UrlListFiles))
                        .SelectionMode(AllowSelectMultiple ? FileManagerSelectionMode.Multiple : FileManagerSelectionMode.Single)
                        .CustomizeThumbnail("function (fileManagerItem) { return fileManagerItem.dataItem ? fileManagerItem.dataItem.thumbnailUrl : null; }")
                        .Permissions(Permissions =>
                        {
                            Permissions.Upload(true);
                            Permissions.Delete(true);
                            Permissions.Download(true);
                            Permissions.Rename(true);
                        })
                        .ItemView(ItemView => {
                            ItemView.ShowFolders(false);
                            ItemView.ShowParentFolder(false);
                            ItemView.Mode(FileManagerItemViewMode.Thumbnails);
                        })
                        .OnSelectedFileOpened("FileManagerModel.OnSelectedFileOpened");
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
