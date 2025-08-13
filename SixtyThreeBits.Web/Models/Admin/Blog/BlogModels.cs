using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.FileStorages;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
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
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.BlogPostsController.GridAdd);

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.BlogPostsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.BlogPostsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.BlogPostsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostsController.GridDelete);

            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreateBlogRepository();
            var viewModel = (await repository.BlogPostList())
            ?.Select(item => new ViewModel.GridModel.GridItem
            {
                BlogPostID = item.BlogPostID,
                BlogPostTitle = item.BlogPostTitle,
                BlogPostAuthorName = item.BlogPostAuthorName,
                BlogPostDate = item.BlogPostDate,
                BlogPostIsPublished = item.BlogPostIsPublished,
                BlogPostDateCreated = item.BlogPostDateCreated,
                UrlBlogPost = Url.RouteUrl(ControllerActionRouteNames.Admin.BlogPostPropertiesController.Properties, new { blogPostID = item.BlogPostID })
            }).ToList();
            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? blogPostID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();
            await iudProcessBlogPostImageFilename(databaseAction: databaseAction, blogPostID: blogPostID);

            var repository = RepositoriesFactory.CreateBlogRepository();
            await repository.BlogPostsIUD(
                databaseAction: databaseAction,
                blogPostID: blogPostID,
                blogPost: new BlogPostIudDTO
                {
                    BlogPostTitle = submitModel.BlogPostTitle,
                    BlogPostAuthorName = submitModel.BlogPostAuthorName,
                    BlogPostDate = Utilities.FormatDateSqlParseFriendly(submitModel.BlogPostDate),
                    BlogPostIsPublished = submitModel.BlogPostIsPublished
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        async Task iudProcessBlogPostImageFilename(Enums.DatabaseActions databaseAction, int? blogPostID)
        {
            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var repository = RepositoriesFactory.CreateBlogRepository();
                var blogPost = await repository.BlogPostGetSingleByID(blogPostID);
                await FileStorage.DeleteFile(filename: blogPost.BlogPostImageFilename, folderPath: _folderPath);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.BlogPostID));

                    grid
                    .ID("BlogGrid")
                    .OnInitialized("model.onGridInit")
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
}
