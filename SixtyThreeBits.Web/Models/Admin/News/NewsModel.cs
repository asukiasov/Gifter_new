using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class NewsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.NewsController.GridAdd);

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.NewsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.NewsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.NewsController.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsController.GridDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateNewsRepository();

            var news = await repository.NewsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : news.Select(item => new ViewModel.GridModel.GridItem
            {
                NewsID = item.NewsID,
                NewsTitle = item.NewsTitle,
                NewsDatePublished = item.NewsDatePublished,
                NewsIsPublished = item.NewsIsPublished,
                NewsDateCreated = item.NewsDateCreated,
                UrlNewsProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.NewsPropertiesController.Properties, new { newsID = item.NewsID })
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? newsID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            await iudProcessNewsImageFilename(databaseAction: databaseAction, newsID: newsID);

            var repository = RepositoriesFactory.CreateNewsRepository();
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

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        async Task iudProcessNewsImageFilename(Enums.DatabaseActions databaseAction, int? newsID)
        {
            if(databaseAction == Enums.DatabaseActions.DELETE)
            {
                var repository = RepositoriesFactory.CreateNewsRepository();
                var newsItem = await repository.NewsGetSingleByID(newsID: newsID);
                await FileStorage.DeleteFile(filename: newsItem.NewsImageFilename);
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
            public class GridModel : DevExtremeGridModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.NewsID));
                    grid
                    .ID("NewsGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlNewsProperties));
                        columns.AddFor(m => m.NewsTitle).Caption(Resources.TextTitle).Width(400).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.NewsDatePublished).Caption(Resources.TextDate).Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.Date);
                        columns.AddFor(m => m.NewsIsPublished).Caption(Resources.TextPublished).Width(130).InitCheckboxColumn();
                        columns.AddFor(m => m.NewsDateCreated).Caption(Resources.TextDateCreated).Width(150).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime).AllowEditing(false);
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
}