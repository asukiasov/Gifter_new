using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RolesPermissionsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowSaveButton = User.HasPermission(ControllerActionRouteNames.Admin.RolePermissionsController.Save);
            viewModel.UrlPermissionsGetByRole = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.GetPermissionsByRole);
            viewModel.UrlSave = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.Save);

            viewModel.RolesGrid = new ViewModel.RolesGridModel();
            viewModel.RolesGrid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.RolesGrid);
            viewModel.PermissionsTree = new ViewModel.PermissionsTreeModel();
            viewModel.PermissionsTree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolePermissionsController.PermissionsTree);

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? roleID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePermissionsRepository();
            var permissionIDs = (await repository.PermissionsListByRoleID(roleID))?.Select(item => item.PermissionID).ToList();

            viewModel.IsSuccess = true;
            viewModel.Data = permissionIDs;

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolesGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateRolesRepository();

            var roles = await repository.RolesList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : roles.Select(Item => new ViewModel.RolesGridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> GetPermissionsTreeItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreatePermissionsRepository();

            var permissions = await repository.PermissionsList();

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : permissions.Select(Item => new ViewModel.PermissionsTreeModel.TreeItem
            {
                PermissionID = Item.PermissionID,
                PermissionParentID = Item.PermissionParentID,
                PermissionCaption = Utilities.GetValuesByLanguage(LanguageCultureCode, Item.PermissionCaption, Item.PermissionCaptionEng)
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> Save(SubmitModelRolePermissionSave submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateRolesRepository();

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
            public RolesGridModel RolesGrid { get; set; }
            public PermissionsTreeModel PermissionsTree { get; set; }
            public string UrlPermissionsGetByRole { get; set; }
            public string UrlSave { get; set; }

            public readonly string TextRoles = Resources.TextRoles;
            public readonly string TextPermissions = Resources.TextPermissions;
            #endregion

            #region Nested Classes
            public class RolesGridModel : DevExtremeGridModelBase<RolesGridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = CreateGridWithStartupValues(html: Html, keyFieldName: nameof(GridItem.RoleID));

                    Grid
                    .ID("RolesGrid")
                    .OnInitialized("model.onGridInit")
                    .OnFocusedRowChanged("model.onGridFocusedRowChanged")
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

            public class PermissionsTreeModel : DevExtremeTreeModelBase<PermissionsTreeModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var tree = CreateTreeWithStartupValues(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    tree
                    .ID("PermissionsTree")
                    .OnInitialized("model.onTreeInit")
                    .OnContentReady("model.onTreeContentReady")
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
            #endregion
        }

        public class SubmitModelRolePermissionSave
        {
            #region Properties
            public int? RoleID { get; set; }
            public List<int?> PermissionIDs { get; set; }
            #endregion
        }
        #endregion
    }
}