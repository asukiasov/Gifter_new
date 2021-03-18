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
    public class BlogModel : WebProjectModelBase
    {

        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridDelete);
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.Grid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.Blog.ListBlog()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                BlogID = Item.BlogID,
                BlogTitle = Item.BlogTitle,
                BlogAuthorName = Item.BlogAuthorName,
                BlogDate = Item.BlogDate,
                UrlBlogProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.BlogItem, new { BlogID = Item.BlogID })
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? BlogID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.Blog.GetSingleBlogByID(BlogID);
                Utilities.DeleteUploadedFile(DBItem?.BlogImageFilename);
            }
            await DataAccessFactory.Blog.BlogIUD(
                DatabaseAction: DatabaseAction,
                BlogID: BlogID,
                BlogTitle: SubmitModel.BlogTitle,
                BlogAuthorName: SubmitModel.BlogAuthorName,
                BlogDate: SubmitModel.BlogDate
            );

            if (DataAccessFactory.Blog.IsError)
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
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.BlogID));

                    Grid
                    .ID("BlogGrid")
                    .OnInitialized("BlogModel.OnBlogGridInit")
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.BlogTitle).Caption("Title").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.BlogAuthorName).Caption("Author Name").Width(150);
                        var ColumnBlogDate = Columns.AddFor(m => m.BlogDate).Caption("Date").DataType(GridColumnDataType.Date).Width(150);
                        InitDateColumn(ColumnBlogDate);
                        Columns.Add().Width(100).Caption(" ").CellTemplate(new JS("BlogModel.GetDetailsButtonColumnCellHtml"));
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? BlogID { get; set; }
                    public string BlogTitle { get; set; }
                    public string BlogAuthorName { get; set; }
                    public DateTime? BlogDate { get; set; }
                    public string UrlBlogProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class BlogModelBase : WebProjectModelBase
    {
        #region Properties
        public Blog DBItemBlog { get; set; }
        #endregion
    }

    public class BlogPropertiesModel : BlogModelBase
    {
        #region Methods
        public BlogPropertiesViewModel GetBlogPropertiesViewModel(BlogPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new BlogPropertiesViewModel();
                ViewModel.BlogSlug = DBItemBlog.BlogSlug;
                ViewModel.BlogTitle = DBItemBlog.BlogTitle;
                ViewModel.BlogText = DBItemBlog.BlogText;
                ViewModel.BlogAuthorName = DBItemBlog.BlogAuthorName;
                ViewModel.BlogDate = DBItemBlog.BlogDate;
            }

            ViewModel.BlogImageFilename = DBItemBlog.BlogImageFilename;
            ViewModel.BlogImageHttpPath = DBItemBlog.BlogImageFilenameHttpPath;
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.BlogItemDeleteImage,new {BlogID = DBItemBlog.BlogID });

            return ViewModel;
        }

        public async Task ValidateBlogPropertiesViewModel(BlogPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.BlogTitle)}\"]", ValueToValidate:ViewModel.BlogTitle),
                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.BlogSlug)}\"]", ValueToValidate:ViewModel.BlogSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.Blog.IsBlogSlugUniq(BlogSlug:ViewModel.BlogSlug,BlogID:DBItemBlog.BlogID);
                        return !IsUniq;
                    },
                    ErrorKey: $"[name=\"{nameof(ViewModel.BlogSlug)}\"]",
                    ErrorMessage: Resources.ValidationBlogsSlugNotUniq
                )
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task<bool> SaveBlogProperties(BlogPropertiesViewModel ViewModel)
        {
            var HasBlogImage = ViewModel.PostedFile?.Length > 0;
            var BlogImageFilename = HasBlogImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;
            if (HasBlogImage)
            {
                Utilities.DeleteUploadedFile(DBItemBlog.BlogImageFilename, DBItemBlog.FolderPhysicalPath);
            }

            await DataAccessFactory.Blog.BlogIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BlogID: DBItemBlog.BlogID,
                BlogSlug: ViewModel.BlogSlug,
                BlogTitle: ViewModel.BlogTitle,
                BlogText: ViewModel.BlogText,
                BlogAuthorName: ViewModel.BlogAuthorName,
                BlogImageFilename: BlogImageFilename,
                BlogDate: ViewModel.BlogDate
            );

            if (!DataAccessFactory.Blog.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasBlogImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: BlogImageFilename, FolderPhysicalPath: DBItemBlog.FolderPhysicalPath);
                }
            }

            return ViewModel.IsSaved;
        }

        public async Task<AjaxResponse> DeleteImage(int? BlogID)
        {
            var BlogItem = await DataAccessFactory.Blog.GetSingleBlogByID(BlogID);
            Utilities.DeleteUploadedFile(BlogItem.BlogImageFilename);

            var AR = new AjaxResponse();
            var DAL = DataAccessFactory.Blog;
            await DAL.BlogIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BlogID: BlogID,
                BlogImageFilename: Constants.NullValueFor.String
                );

            AR.IsSuccess = !DAL.IsError;

            return AR;
        }

        #endregion

        #region Sub Classes
        public class BlogPropertiesViewModel : FormViewModelBase
        {

            #region Properties             
            public string BlogSlug { get; set; }
            public string BlogTitle { get; set; }
            public string BlogText { get; set; }
            public string BlogAuthorName { get; set; }
            public DateTime? BlogDate { get; set; }
            public string BlogImageFilename { get; set; }
            public string BlogImageHttpPath { get; set; }
            public bool HasBlogImage => !string.IsNullOrWhiteSpace(BlogImageFilename);
            public IFormFile PostedFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }
}
