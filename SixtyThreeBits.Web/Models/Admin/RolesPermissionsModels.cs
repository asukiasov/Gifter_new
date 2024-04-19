using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RolePermissionsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowSaveButton = User.HasPermission(ControllerActionRouteNames.Admin.RolePermissionsController.Save);
            viewModel.UrlPermissionsGetByRole = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.GetPermissionsByRole);
            viewModel.UrlSave = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.Save);

            viewModel.RolesGrid = new ViewModel.RolesGridViewModel();
            viewModel.RolesGrid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.RolesGrid);
            viewModel.PermissionsTree = new ViewModel.PermissionsTreeViewModel();
            viewModel.PermissionsTree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.PermissionsTree);

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? roleID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var permissionIDs = (await repository.PermissionsListByRoleID(roleID))
                ?.Select(item => item.PermissionID)
                .ToList();

            viewModel.IsSuccess = true;
            viewModel.Data = permissionIDs;

            return viewModel;
        }

        public async Task<List<ViewModel.RolesGridViewModel.GridItem>> ListRolesGridItems()
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            var viewModel = (await repository.RolesList())
            ?.Select(Item => new ViewModel.RolesGridViewModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName
            })
            .ToList();

            return viewModel;
        }

        public async Task<List<ViewModel.PermissionsTreeViewModel.TreeItem>> ListPermissionsTreeItems()
        {
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var viewModel = (await repository.PermissionsList())
            ?.Select(Item => new ViewModel.PermissionsTreeViewModel.TreeItem
            {
                PermissionID = Item.PermissionID,
                PermissionParentID = Item.PermissionParentID,
                PermissionCaption = Utilities.GetValuesByLanguage(LanguageCultureCode, Item.PermissionCaption, Item.PermissionCaptionEng)
            })
            .ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> Save(ViewModel.RolePermissionSaveSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetRolesRepository();

            await repository.RolesPermissionsUpdate(
                roleID: submitModel.RoleID,
                permissionIDs: submitModel.PermissionIDs
            );
            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowSaveButton { get; set; }
            public RolesGridViewModel RolesGrid { get; set; }
            public PermissionsTreeViewModel PermissionsTree { get; set; }
            public string UrlPermissionsGetByRole { get; set; }
            public string UrlSave { get; set; }

            public readonly string TextRoles = Resources.TextRoles;
            public readonly string TextPermissions = Resources.TextPermissions;
            #endregion

            #region Nested Classes
            public class RolesGridViewModel : DevExtremeGridViewModelBase<RolesGridViewModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = CreateGridWithStartupValues(html: Html, keyFieldName: nameof(GridItem.RoleID));

                    Grid
                    .ID("RolesGrid")
                    .OnInitialized("rolesPermissionsModel.onRolesGridInit")
                    .OnFocusedRowChanged("rolesPermissionsModel.onRolesGridFocusedRowChanged")
                    .FilterRow(Options =>
                    {
                        Options.Visible(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.RoleName).Caption(Resources.TextRole);
                    });


                    return Grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? RoleID { get; set; }
                    public string RoleName { get; set; }
                    #endregion
                }
                #endregion
            }

            public class PermissionsTreeViewModel : DevExtremeTreeViewModelBase<PermissionsTreeViewModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var tree = CreateTreeWithStartupValues(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    tree
                    .ID("PermissionsTree")
                    .OnInitialized("rolesPermissionsModel.onPermissionsTreeInit")
                    .OnContentReady("rolesPermissionsModel.onPermissionsTreeContentReady")
                    .FilterRow(Options =>
                    {
                        Options.Visible(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Selection(Options =>
                    {
                        Options.Mode(SelectionMode.Multiple);
                        Options.Recursive(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.PermissionCaption).Caption(Resources.TextPermission);

                    });

                    return tree;
                }
                #endregion

                #region Nested Classes
                public class TreeItem
                {
                    #region Properties
                    public int? PermissionID { get; set; }
                    public int? PermissionParentID { get; set; }
                    public string PermissionCaption { get; set; }
                    #endregion
                }
                #endregion
            }

            public class RolePermissionSaveSubmitModel
            {
                #region Properties
                public int? RoleID { get; set; }
                public List<int?> PermissionIDs { get; set; }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}