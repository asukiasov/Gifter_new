using DevExpress.XtraRichEdit.Layout.Engine;
using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
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
    public class RolesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.RolesControllers.GridAdd);

            viewModel.Grid = new ViewModel.GridViewModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.RolesControllers.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.RolesControllers.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.RolesControllers.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesControllers.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesControllers.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesControllers.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesControllers.GridDelete);

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> ListGridItem()
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            var viewModel = (await repository.RolesList())
            ?.Select(Item => new ViewModel.GridViewModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName,
                RoleCode = Item.RoleCode
            })
            .ToList();

            return viewModel;
        }

        public async Task IUD(Enums.DatabaseActions databaseAction, int? roleID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            await repository.RolesIUD(
                databaseAction: databaseAction,
                roleID: roleID,
                role: new RoleIudDTO
                {
                    RoleName = submitModel.RoleName,
                    RoleCode = submitModel.RoleCode
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
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.RoleID));

                    grid
                    .ID("Roles.Grid")
                    .OnInitialized("rolesModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.RoleName).Caption(Resources.TextName).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? RoleID { get; set; }
                    public string RoleName { get; set; }
                    public int? RoleCode { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
