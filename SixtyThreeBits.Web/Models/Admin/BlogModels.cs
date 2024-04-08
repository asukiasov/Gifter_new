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
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class BlogModel : ModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Blog].FolderName;
        #endregion

        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridAdd);

            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Blog.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.GridDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetBlogRepository();
            var viewModel = (await repository.BlogPostList())
            ?.Select(item => new PageViewModel.GridModel.GridItem
            {
                BlogPostID = item.BlogPostID,
                BlogPostTitle = item.BlogPostTitle,
                BlogPostAuthorName = item.BlogPostAuthorName,
                BlogPostDate = item.BlogPostDate,
                BlogPostIsPublished = item.BlogPostIsPublished,
                BlogPostDateCreated = item.BlogPostDateCreated,
                UrlBlogPost = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostProperties, new { blogPostID = item.BlogPostID })
            }).ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? blogPostID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetBlogRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await repository.BlogPostGetSingleByID(blogPostID);
                await DeleteUploadedFile(filename: DBItem.BlogPostImageFilename, folderPath: _folderPath);
            }

            await repository.BlogPostsIUD(
                databaseAction: databaseAction,
                blogPostID: blogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostTitle = submitModel.BlogPostTitle,
                    BlogPostAuthorName = submitModel.BlogPostAuthorName,
                    BlogPostDate = submitModel.BlogPostDate,
                    BlogPostIsPublished = submitModel.BlogPostIsPublished
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
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.BlogPostID));

                    grid
                    .ID("BlogGrid")
                    .OnInitialized("blogModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlBlogPost));
                        columns.AddFor(m => m.BlogPostTitle).Caption(Resources.TextTitle).Width(400).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.BlogPostAuthorName).Caption(Resources.TextAuthor).Width(150);
                        columns.AddFor(m => m.BlogPostDate).Caption(Resources.TextDate).DataType(GridColumnDataType.Date).Width(150).InitDateColumn();
                        columns.AddFor(m => m.BlogPostIsPublished).Caption(Resources.TextPublished).DataType(GridColumnDataType.Boolean).Width(130).InitCheckboxColumn();
                        columns.AddFor(m => m.BlogPostDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.Date).Width(150).InitDateColumn(formatDateTime: true).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? BlogPostID { get; set; }
                    public string BlogPostTitle { get; set; }
                    public string BlogPostAuthorName { get; set; }
                    public DateTime? BlogPostDate { get; set; }
                    public DateTime? BlogPostDateCreated { get; set; }
                    public bool? BlogPostIsPublished { get; set; }
                    public string UrlBlogPost { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class BlogModelBase : ModelBase
    {
        #region Properties
        public BlogPostDTO DBItem { get; set; }
        #endregion
    }

    public class BlogPropertiesModel : BlogModelBase
    {
        #region Properties
        readonly string _folderPath = FileStorageManager.Modules[Enums.FileManagerModules.Blog].FolderName;
        #endregion

        #region Methods
        public BlogPropertiesViewModel GetBlogPropertiesViewModel(BlogPropertiesViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new BlogPropertiesViewModel();
                viewModel.BlogPostIsPublished = DBItem.BlogPostIsPublished;
                viewModel.BlogPostSlug = DBItem.BlogPostSlug;
                viewModel.BlogPostTitle = DBItem.BlogPostTitle;
                viewModel.BlogPostShortText = DBItem.BlogPostShortText;
                viewModel.BlogPostText = DBItem.BlogPostText;
                viewModel.BlogPostAuthorName = DBItem.BlogPostAuthorName;
                viewModel.BlogPostDate = DBItem.BlogPostDate;                
            }

            viewModel.BlogPostImageFilename = DBItem.BlogPostImageFilename;
            viewModel.BlogPostImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.BlogPostImageFilename, _folderPath);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Blog.PostPropertiesDeleteImage, new { blogPostID = DBItem.BlogPostID });

            return viewModel;
        }

        public async Task ValidatePageViewModel(BlogPropertiesViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostTitle)), valueToValidate: viewModel.BlogPostTitle));
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostSlug)), valueToValidate: viewModel.BlogPostSlug));
            viewModel.AddError(
                await Validation.ValidateAsync(
                    errorAction: async () =>
                    {
                        var repository = RepositoriesFactory.GetBlogRepository();
                        var isUniq = await repository.BlogPostIsSlugUniq(blogPostSlug: viewModel.BlogPostSlug, blogPostID: DBItem.BlogPostID);
                        return !isUniq;
                    },
                    errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.BlogPostSlug)),
                    errorMessage: Resources.ValidationSlugNotUniq
                )
            );
        }

        public async Task Save(BlogPropertiesViewModel viewModel)
        {
            var hasBlogImage = viewModel.BlogImageFile?.Length > 0;
            var blogPostImageFilename = hasBlogImage ? GetFilenameFromUploadedFile(viewModel.BlogImageFile) : null;
            if (hasBlogImage)
            {
                await DeleteUploadedFile(DBItem.BlogPostImageFilename, _folderPath);
            }

            var repository = RepositoriesFactory.GetBlogRepository();
            await repository.BlogPostsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                blogPostID: DBItem.BlogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostSlug = viewModel.BlogPostSlug,
                    BlogPostTitle = viewModel.BlogPostTitle,
                    BlogPostShortText = viewModel.BlogPostShortText,
                    BlogPostText = viewModel.BlogPostText,
                    BlogPostAuthorName = viewModel.BlogPostAuthorName,
                    BlogPostImageFilename = blogPostImageFilename,
                    BlogPostDate = viewModel.BlogPostDate,
                    BlogPostIsPublished = viewModel.BlogPostIsPublished
                }                
            );

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasBlogImage)
                {
                    await SaveUploadedFile(viewModel.BlogImageFile, blogPostImageFilename, _folderPath);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await DeleteUploadedFile(DBItem.BlogPostImageFilename, _folderPath);

            var repository = RepositoriesFactory.GetBlogRepository();
            await repository.BlogPostsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                blogPostID: DBItem.BlogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostImageFilename = Constants.NullValueFor.String
                }                
            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }

        #endregion

        #region Nested Classes
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
            public IFormFile BlogImageFile { get; set; }
            public string UrlDeleteImage { get; set; }

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            public readonly string TextTitle = Resources.TextTitle;
            public readonly string TextSlug = Resources.TextSlug;
            public readonly string TextGenerateFromTitle = Resources.TextGenerateFromTitle;
            public readonly string TextAuthor = Resources.TextAuthor;
            public readonly string TextDate = Resources.TextDate;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescription = Resources.TextDescription;

            public readonly string FormatDate = Constants.Formats.Date;
            #endregion
        }
        #endregion
    }
}
