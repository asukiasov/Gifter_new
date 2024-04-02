using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class NewsModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);

            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.News.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.News.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.News.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetNewsRepository();
            var viewModel = (await repository.NewsList())
            ?.Select(item => new PageViewModel.GridModel.GridItem
            {
                NewsID = item.NewsID,
                NewsTitle = item.NewsTitle,
                NewsDatePublished = item.NewsDatePublished,
                NewsIsPublished = item.NewsIsPublished,
                NewsDateCreated = item.NewsDateCreated,
                UrlNewsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItem, new { newsID = item.NewsID })
            })
            .ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? newsID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetNewsRepository();
            await repository.NewsIUD(
                databaseAction: databaseAction,
                newsID: newsID,
                news: new NewsIudDTO
                {
                    NewsTitle = submitModel.NewsTitle,
                    NewsDatePublished = submitModel.NewsDatePublished,
                    NewsIsPublished = submitModel.NewsIsPublished
                }                
            );
            if (repository.IsError)
            {
                Form.AddError(repository.ErrorMessage);
            }
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.NewsID));
                    grid
                    .ID("NewsGrid")
                    .OnInitialized("newsModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlNewsProperties));
                        columns.AddFor(m => m.NewsTitle).Caption(Resources.TextTitle).Width(400).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.NewsDatePublished).Caption(Resources.TextDate).DataType(GridColumnDataType.Date).Width(150).InitDateColumn();
                        columns.AddFor(m => m.NewsIsPublished).Caption(Resources.TextPublished).DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                        columns.AddFor(m => m.NewsDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.Date).Width(150).InitDateColumn(formatDateTime: true).AllowEditing(false);
                        columns.Add();
                    });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? NewsID { get; set; }
                    public string NewsTitle { get; set; }
                    public DateTime? NewsDatePublished { get; set; }
                    public bool? NewsIsPublished { get; set; }
                    public DateTime? NewsDateCreated { get; set; }
                    public string UrlNewsProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class NewsModelBase : ModelBase
    {
        #region Properties
        public NewsDTO DBItem { get; set; }
        #endregion
    }

    public class NewsPropertiesModel : NewsModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.News].FolderName;
        #endregion

        #region Methods
        public PageViewModel GetPageViewModel(PageViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.NewsSlug = DBItem.NewsSlug;
                viewModel.NewsTitle = DBItem.NewsTitle;
                viewModel.NewsTitleEng = DBItem.NewsTitleEng;
                viewModel.NewsShortDescription = DBItem.NewsShortDescription;
                viewModel.NewsShortDescriptionEng = DBItem.NewsShortDescriptionEng;
                viewModel.NewsText = DBItem.NewsText;
                viewModel.NewsTextEng = DBItem.NewsTextEng;
                viewModel.NewsIsPublished = DBItem.NewsIsPublished;
                viewModel.NewsDatePublished = DBItem.NewsDatePublished;
            }

            viewModel.NewsImageFilename = DBItem.NewsImageFilename;
            viewModel.NewsImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.NewsImageFilename, _folderPath);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItemDeleteImage, new { newsID = DBItem.NewsID });

            var urlFileManager = Url.RouteUrl(ControllerActionRouteNames.Admin.FileManager.Page, new { ModuleName = Enums.FileManagerModules.News });
            viewModel.UrlFileManager = urlFileManager;

            return viewModel;
        }

        public async Task ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.NewsTitle)), valueToValidate: viewModel.NewsTitle));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.NewsSlug)), valueToValidate: viewModel.NewsSlug));
            viewModel.AddError(
                await Validation.ValidateAsync(
                    errorAction: async () =>
                    {
                        var repository = RepositoriesFactory.GetNewsRepository();
                        var IsUniq = await repository.NewsIsSlugUniq(newsSlug: viewModel.NewsSlug, newsID: DBItem.NewsID);
                        return !IsUniq;
                    },
                    errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.NewsSlug)),
                    errorMessage: Resources.ValidationSlugNotUniq
                )
            );
        }

        public async Task Save(PageViewModel viewModel)
        {
            var hasNewsImage = viewModel.NewsImageFile?.Length > 0;
            var newsImageFilename = hasNewsImage ? GetFilenameFromUploadedFile(viewModel.NewsImageFile) : null;
            if (hasNewsImage)
            {
                await DeleteUploadedFile(newsImageFilename, _folderPath);
            }

            var repository = RepositoriesFactory.GetNewsRepository();
            await repository.NewsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                newsID: DBItem.NewsID,
                news: new NewsIudDTO
                {
                    NewsSlug = viewModel.NewsSlug,
                    NewsTitle = viewModel.NewsTitle,
                    NewsTitleEng = viewModel.NewsTitleEng ?? Constants.NullValueFor.String,
                    NewsShortDescription = viewModel.NewsShortDescription ?? Constants.NullValueFor.String,
                    NewsShortDescriptionEng = viewModel.NewsShortDescriptionEng ?? Constants.NullValueFor.String,
                    NewsText = viewModel.NewsText ?? Constants.NullValueFor.String,
                    NewsTextEng = viewModel.NewsTextEng ?? Constants.NullValueFor.String,
                    NewsImageFilename = newsImageFilename,
                    NewsDatePublished = viewModel.NewsDatePublished,
                    NewsIsPublished = viewModel.NewsIsPublished
                }
            );

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasNewsImage)
                {
                    await SaveUploadedFile(viewModel.NewsImageFile, newsImageFilename, _folderPath);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await DeleteUploadedFile(filename: DBItem.NewsImageFilename, folderPath: _folderPath);

            var repository = RepositoriesFactory.GetNewsRepository();
            await repository.NewsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                newsID: DBItem.NewsID,
                news: new NewsIudDTO
                {
                    NewsImageFilename = Constants.NullValueFor.String
                }
                
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties             
            public string NewsSlug { get; set; }
            public string NewsTitle { get; set; }
            public string NewsTitleEng { get; set; }
            public string NewsShortDescription { get; set; }
            public string NewsShortDescriptionEng { get; set; }
            public string NewsText { get; set; }
            public string NewsTextEng { get; set; }
            public DateTime? NewsDatePublished { get; set; }
            public bool NewsIsPublished { get; set; }
            public string NewsImageFilename { get; set; }
            public string NewsImageHttpPath { get; set; }
            public bool HasNewsImage => !string.IsNullOrWhiteSpace(NewsImageFilename);
            public IFormFile NewsImageFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public string UrlFileManager { get; set; }

            public readonly string FormatDate = Constants.Formats.Date;

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextTitleEng = Resources.TextTitleEng;
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextDate = Resources.TextDate;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextDescription = Resources.TextDescription;
            public readonly string TextDescriptionEng = Resources.TextDescriptionEng;
            #endregion
        }
        #endregion
    }
}