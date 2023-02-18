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
            var ViewModel = (await DataAccessFactory.Blog.ListBlog())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                BlogPostID = Item.BlogPostID,
                BlogPostTitle = Item.BlogPostTitle,
                BlogPostAuthorName = Item.BlogPostAuthorName,
                BlogPostDate = Item.BlogPostDate,
                BlogPostIsPublished = Item.BlogPostIsPublished,
                UrlBlogPost = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostProperties, new { BlogPostID = Item.BlogPostID })
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? BlogPostID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.Blog.GetSingleBlogByID(BlogPostID);
                Utilities.DeleteUploadedFile(DBItem?.BlogPostImageFilename);
            }

            await DataAccessFactory.Blog.BlogIUD(
                DatabaseAction: DatabaseAction,
                BlogPostID: BlogPostID,
                BlogPostTitle: SubmitModel.BlogPostTitle,
                BlogPostAuthorName: SubmitModel.BlogPostAuthorName,
                BlogPostDate: SubmitModel.BlogPostDate,
                BlogPostIsPublished: SubmitModel.BlogPostIsPublished
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
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.BlogPostID));

                    Grid
                    .ID("BlogGrid")
                    .OnInitialized("BlogModel.OnGridInit")  
                    .Columns(Columns =>
                    {
                        Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlBlogPost));
                        Columns.AddFor(m => m.BlogPostTitle).Caption("სათაური").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.BlogPostAuthorName).Caption("ავტორი").Width(150);
                        Columns.AddFor(m => m.BlogPostDate).Caption("თარიღი").DataType(GridColumnDataType.Date).Width(150).InitDateColumn();
                        Columns.AddFor(m => m.BlogPostIsPublished).Caption("გამოქვეყნებული").DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? BlogPostID { get; set; }
                    public string BlogPostTitle { get; set; }
                    public string BlogPostAuthorName { get; set; }
                    public DateTime? BlogPostDate { get; set; }
                    public bool BlogPostIsPublished { get; set; }
                    public string UrlBlogPost { get; set; }
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
        public BlogPost DBItemBlog { get; set; }
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
                ViewModel.BlogPostIsPublished = DBItemBlog.BlogPostIsPublished;
                ViewModel.BlogPostSlug = DBItemBlog.BlogPostSlug;
                ViewModel.BlogPostTitle = DBItemBlog.BlogPostTitle;
                ViewModel.BlogPostShortText = DBItemBlog.BlogPostShortText;
                ViewModel.BlogPostText = DBItemBlog.BlogPostText;
                ViewModel.BlogPostAuthorName = DBItemBlog.BlogPostAuthorName;
                ViewModel.BlogPostDate = DBItemBlog.BlogPostDate;
            }

            ViewModel.BlogPostImageFilename = DBItemBlog.BlogPostImageFilename;
            ViewModel.BlogPostImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemBlog.BlogPostImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostPropertiesDeleteImage, new { BlogPostID = DBItemBlog.BlogPostID });

            return ViewModel;
        }

        public async Task ValidateBlogPropertiesViewModel(BlogPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.BlogPostTitle)), ValueToValidate:ViewModel.BlogPostTitle),
                Validation.ValidateRequired(ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.BlogPostSlug)), ValueToValidate:ViewModel.BlogPostSlug),
                await Validation.ValidateAsync(
                    ErrorAction: async () =>
                    {
                        var IsUniq = await DataAccessFactory.Blog.IsBlogSlugUniq(BlogPostSlug:ViewModel.BlogPostSlug, BlogPostID:DBItemBlog.BlogPostID);
                        return !IsUniq;
                    },
                    ErrorKey: Validation.GetJQueryNameSelectorFor(nameof(ViewModel.BlogPostSlug)),
                    ErrorMessage: Resources.ValidationSlugNotUniq
                )
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task SaveBlogProperties(BlogPropertiesViewModel ViewModel)
        {
            var HasBlogImage = ViewModel.PostedFile?.Length > 0;
            var BlogPostImageFilename = HasBlogImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;
            if (HasBlogImage)
            {
                Utilities.DeleteUploadedFile(DBItemBlog.BlogPostImageFilename);
            }

            await DataAccessFactory.Blog.BlogIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BlogPostID: DBItemBlog.BlogPostID,
                BlogPostSlug: ViewModel.BlogPostSlug,
                BlogPostTitle: ViewModel.BlogPostTitle,
                BlogPostShortText: ViewModel.BlogPostShortText,
                BlogPostText: ViewModel.BlogPostText,
                BlogPostAuthorName: ViewModel.BlogPostAuthorName,
                BlogPostImageFilename: BlogPostImageFilename,
                BlogPostDate: ViewModel.BlogPostDate,
                BlogPostIsPublished: ViewModel.BlogPostIsPublished
            );

            if (!DataAccessFactory.Blog.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasBlogImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: BlogPostImageFilename);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage(int? BlogPostID)
        {
            Utilities.DeleteUploadedFile(DBItemBlog.BlogPostImageFilename);

            var AR = new AjaxResponse();
            await DataAccessFactory.Blog.BlogIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BlogPostID: BlogPostID,
                BlogPostImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Blog.IsError;

            return AR;
        }

        #endregion

        #region Sub Classes
        public class BlogPropertiesViewModel : FormViewModelBase
        {
            #region Properties           
            public bool BlogPostIsPublished { get; set; }
            public string BlogPostSlug { get; set; }
            public string BlogPostTitle { get; set; }
            public string BlogPostShortText { get; set; }            
            public string BlogPostText { get; set; }
            public string BlogPostAuthorName { get; set; }
            public DateTime? BlogPostDate { get; set; }
            public string BlogPostImageFilename { get; set; }
            public string BlogPostImageHttpPath { get; set; }
            public bool HasBlogPostImage => !string.IsNullOrWhiteSpace(BlogPostImageFilename);
            public IFormFile PostedFile { get; set; }
            public string UrlDeleteImage { get; set; }
            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }
}
