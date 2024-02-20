using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RolesModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Roles.GridAdd);

            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Roles.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Roles.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Roles.GridDelete);
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Roles.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Roles.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Roles.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Roles.GridDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            var viewModel = (await repository.RolesList()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName,
                RoleCode = Item.RoleCode
            }).ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? roleID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            await repository.RolesIUD(
                databaseAction: databaseAction,
                roleID: roleID,
                roleName: submitModel.RoleName,
                roleCode: submitModel.RoleCode
            );

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
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
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.RoleID));

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
