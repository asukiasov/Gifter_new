using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RedirectsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.RedirectsController.GridAdd);

            viewModel.Grid = new ViewModel.GridViewModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RedirectsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.RedirectsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.RedirectsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.RedirectsController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.RedirectsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.RedirectsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.RedirectsController.GridDelete);

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> ListGridItems()
        {
            var repository = RepositoriesFactory.CreateRedirectsRepository();
            var viewModel = (await repository.RedirectsList())
            ?.Select(Item => new ViewModel.GridViewModel.GridItem
            {
                RedirectID = Item.RedirectID,
                RedirectFrom = Item.RedirectFrom,
                RedirectTo = Item.RedirectTo
            }).ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? redirectID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.CreateRedirectsRepository();
            await repository.RedirectsIUD(
                databaseAction: databaseAction,
                redirectID: redirectID,
                redirect: new RedirectIudDTO
                {
                    RedirectFrom = submitModel.RedirectFrom,
                    RedirectTo = submitModel.RedirectTo
                }                
            );

            if (repository.IsError)
            {
                Form.AddError(repository.ErrorMessage);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridViewModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridViewModel : DevExtremeGridViewModelBase<GridViewModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.RedirectID));

                    grid
                   .ID("RedirectsGrid")
                   .OnInitialized("model.onGridInit")
                   .Columns(columns =>
                   {
                       columns.AddFor(m => m.RedirectFrom).Caption(Resources.TextRedirectFrom).Width(500);
                       columns.AddFor(m => m.RedirectTo).Caption(Resources.TextRedirectTo).Width(500);
                       columns.Add();
                   });

                    return grid;

                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? RedirectID { get; set; }
                    public string RedirectFrom { get; set; }
                    public string RedirectTo { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}