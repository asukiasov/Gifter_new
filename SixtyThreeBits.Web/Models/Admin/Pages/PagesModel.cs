using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
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
    public class PagesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridAdd);
            viewModel.Grid = new ViewModel.GridModel();            
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.PagesController.GridDelete);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.PagesController.GridDelete);
            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreatePagesRepository();

            var viewModel = (await repository.PagesList())
            ?.Select(item => new ViewModel.GridModel.GridItem
            {
                PageID = item.PageID,
                PageTitle = item.PageTitle,
                PageTitleEng = item.PageTitleEng,
                PageIsPublished = item.PageIsPublished,
                PageDateCreated = item.PageDateCreated,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.PagePropertiesController.Properties, new { pageID = item.PageID })
            })
            .ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? pageID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesIUD(
                databaseAction: databaseAction,
                pageID: pageID,
                page: new PageIudDTO
                {
                    PageTitle = submitModel.PageTitle,
                    PageTitleEng = submitModel.PageTitleEng,
                    PageIsPublished = submitModel.PageIsPublished
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> Delete(int? pageID)
        {
            var viewModel = new AjaxResponse();

            await deleteProcessPageImageFilename(pageID);

            var repository = RepositoriesFactory.CreatePagesRepository();
            await repository.PagesDelete(pageID: pageID);
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        async Task deleteProcessPageImageFilename(int? pageID)
        {
            var repository = RepositoriesFactory.CreatePagesRepository();
            var page = await repository.PagesGetSingleByID(pageID);
            await FileStorage.DeleteFile(page.PageImageFilename);
        }

        public async Task<AjaxResponse> GetPagesJson()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePagesRepository();

            var pages = (await repository.PagesList())?
            .OrderBy(item => item.PageTitle)
            .Select(item => new KeyValueTuple<int?, string>
            {
                Key = item.PageID,
                Value = $"{string.Join(" | ", item.PageTitle, item.PageTitleEng)}"
            })
            .ToList();

            viewModel.Data = pages;
            viewModel.IsSuccess = true;

            return viewModel;
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
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.PageID));

                    grid
                    .ID("PagesGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProperties));
                        columns.AddFor(m => m.PageTitle).Caption(Resources.TextTitle).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.PageTitleEng).Caption(Resources.TextTitleEng).Width(300);                        
                        columns.AddFor(m => m.PageIsPublished).Caption(Resources.TextPublished).Width(100).InitCheckboxColumn();
                        columns.AddFor(m => m.PageDateCreated).Caption(Resources.TextDateCreated).DataType(GridColumnDataType.DateTime).Width(140).InitDateColumn(true).AllowEditing(false);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public record GridItem
                {
                    #region Properties
                    public int? PageID { get; init; }
                    public string PageSlug { get; init; }
                    public string PageTitle { get; init; }
                    public string PageTitleEng { get; init; }                    
                    public bool? PageIsPublished { get; init; }
                    public DateTime? PageDateCreated { get; init; }
                    public string UrlProperties { get; init; }                    
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }  
}