using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class NewsModel : WebProjectModelBase
    {

        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.News.GridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.News.GridDelete);
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.News.Grid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.News.ListNews()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                NewsID = Item.NewsID,
                NewsTitle = Item.NewsTitle,
                NewsDatePublished = Item.NewsDatePublished,
                NewsIsPublished = Item.NewsIsPublished,
                UrlNewsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItem, new { NewsID = Item.NewsID })
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? NewsID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.News.GetSingleNewsByID(NewsID);
                Utilities.DeleteUploadedFile(DBItem?.NewsImageFilename);
            }
            await DataAccessFactory.News.NewsIUD(
                DatabaseAction: DatabaseAction,
                NewsID: NewsID,
                NewsTitle: SubmitModel.NewsTitle,                
                NewsDatePublished: SubmitModel.NewsDatePublished,
                NewsIsPublished: SubmitModel.NewsIsPublished
            );

            if (DataAccessFactory.News.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.NewsID));

                    Grid
                    .ID("NewsGrid")
                    .OnInitialized("NewsModel.OnNewsGridInit")
                    .Columns(Columns =>
                    {
                        Columns.Add().Width(30).Caption(" ").Alignment(HorizontalAlignment.Center).CellTemplate(new JS("NewsModel.GetDetailsButtonColumnCellHtml"));
                        Columns.AddFor(m => m.NewsTitle).Caption("Title").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.NewsDatePublished).Caption("Publish Date").DataType(GridColumnDataType.Date).Width(150).InitDateColumn();
                        Columns.AddFor(m => m.NewsIsPublished).Caption("Published?").DataType(GridColumnDataType.Boolean).Width(100).InitCheckboxColumn();
                        Columns.Add();
                    });

                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? NewsID { get; set; }
                    public string NewsTitle { get; set; }
                    public DateTime? NewsDatePublished { get; set; }
                    public bool NewsIsPublished { get; set; }
                    public string UrlNewsProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class NewsModelBase : WebProjectModelBase
    {
        #region Properties
        public News DBItemNews { get; set; }
        #endregion
    }

    public class NewsPropertiesModel : NewsModelBase
    {
        #region Methods
        public NewsPropertiesViewModel GetNewsPropertiesViewModel(NewsPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new NewsPropertiesViewModel();
                ViewModel.NewsSlug = DBItemNews.NewsSlug;
                ViewModel.NewsTitle = DBItemNews.NewsTitle;
                ViewModel.NewsTitleEng = DBItemNews.NewsTitleEng;
                ViewModel.NewsTitleRus = DBItemNews.NewsTitleRus;                
                ViewModel.NewsShortDescription = DBItemNews.NewsShortDescription;
                ViewModel.NewsShortDescriptionEng = DBItemNews.NewsShortDescriptionEng;
                ViewModel.NewsShortDescriptionRus = DBItemNews.NewsShortDescriptionRus;
                ViewModel.NewsText = DBItemNews.NewsText;
                ViewModel.NewsTextEng = DBItemNews.NewsTextEng;
                ViewModel.NewsTextRus = DBItemNews.NewsTextRus;
                ViewModel.NewsIsPublished = DBItemNews.NewsIsPublished;
                ViewModel.NewsDatePublished = DBItemNews.NewsDatePublished;
            }

            ViewModel.NewsImageFilename = DBItemNews.NewsImageFilename;
            ViewModel.NewsImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemNews.NewsImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItemDeleteImage, new { NewsID = DBItemNews.NewsID });

            return ViewModel;
        }

        public async Task ValidateNewsPropertiesViewModel(NewsPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.NewsTitle)), ValueToValidate:ViewModel.NewsTitle),
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.NewsSlug)), ValueToValidate:ViewModel.NewsSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.News.IsSlugUniq(NewsSlug:ViewModel.NewsSlug, NewsID: DBItemNews.NewsID);
                        return !IsUniq;
                    },
                    ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.NewsSlug)),
                    ErrorMessage: Resources.ValidationSlugNotUniq
                )
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task<bool> SaveNewsProperties(NewsPropertiesViewModel ViewModel)
        {
            var HasNewsImage = ViewModel.PostedFile?.Length > 0;
            var NewsImageFilename = HasNewsImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;
            if (HasNewsImage)
            {
                Utilities.DeleteUploadedFile(DBItemNews.NewsImageFilename);
            }

            await DataAccessFactory.News.NewsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                NewsID: DBItemNews.NewsID,
                NewsSlug: ViewModel.NewsSlug,
                NewsTitle: ViewModel.NewsTitle,
                NewsTitleEng: ViewModel.NewsTitleEng,
                NewsTitleRus: ViewModel.NewsTitleRus,
                NewsShortDescription: ViewModel.NewsShortDescription,
                NewsShortDescriptionEng: ViewModel.NewsShortDescriptionEng,
                NewsShortDescriptionRus: ViewModel.NewsShortDescriptionRus,
                NewsText: ViewModel.NewsText,
                NewsTextEng: ViewModel.NewsTextEng,
                NewsTextRus: ViewModel.NewsTextRus,
                NewsImageFilename: NewsImageFilename,
                NewsDatePublished: ViewModel.NewsDatePublished,
                NewsIsPublished: ViewModel.NewsIsPublished
            );

            if (!DataAccessFactory.News.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasNewsImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: NewsImageFilename);
                }
            }

            return ViewModel.IsSaved;
        }

        public async Task<AjaxResponse> DeleteImage(int? NewsID)
        {
            var NewsItem = await DataAccessFactory.News.GetSingleNewsByID(NewsID);
            Utilities.DeleteUploadedFile(NewsItem.NewsImageFilename);

            var AR = new AjaxResponse();
            await DataAccessFactory.News.NewsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                NewsID: NewsID,
                NewsImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.News.IsError;

            return AR;
        }

        #endregion

        #region Sub Classes
        public class NewsPropertiesViewModel : FormViewModelBase
        {
            #region Properties             
            public string NewsSlug { get; set; }
            public string NewsTitle { get; set; }
            public string NewsTitleEng { get; set; }
            public string NewsTitleRus { get; set; }
            public string NewsShortDescription { get; set; }
            public string NewsShortDescriptionEng { get; set; }
            public string NewsShortDescriptionRus { get; set; }
            public string NewsText { get; set; }
            public string NewsTextEng { get; set; }
            public string NewsTextRus { get; set; }
            public DateTime? NewsDatePublished { get; set; }
            public bool NewsIsPublished { get; set; }
            public string NewsImageFilename { get; set; }
            public string NewsImageHttpPath { get; set; }
            public bool HasNewsImage => !string.IsNullOrWhiteSpace(NewsImageFilename);
            public IFormFile PostedFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }

}
