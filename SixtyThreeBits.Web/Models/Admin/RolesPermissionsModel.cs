using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class RolePermissionsModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowSaveButton = User.HasPermission(ControllerActionRouteNames.Admin.RolesPermissions.Save);
            viewModel.UrlPermissionsGetByRole = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesPermissions.PermissionsGetByRole);
            viewModel.UrlSave = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesPermissions.Save);

            viewModel.RolesGrid = new PageViewModel.RolesGridModel();
            viewModel.RolesGrid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesPermissions.RolesGrid);
            viewModel.PermissionsTree = new PageViewModel.PermissionsTreeModel();
            viewModel.PermissionsTree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.RolesPermissions.PermissionsTree);


            return viewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? roleID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var permissions = await repository.PermissionsListByRoleID(roleID);
            viewModel.IsSuccess = true;
            viewModel.Data = permissions;
            return viewModel;
        }

        public async Task<List<PageViewModel.RolesGridModel.GridItem>> GetRolesGridModel()
        {
            var repository = RepositoriesFactory.GetRolesRepository();
            var viewModel = (await repository.RolesList()).Select(Item => new PageViewModel.RolesGridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName
            }).ToList();
            return viewModel;
        }

        public async Task<List<PageViewModel.PermissionsTreeModel.TreeItem>> GetPermissionsTreeModel()
        {
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var viewModel = (await repository.PermissionsList()).Select(Item => new PageViewModel.PermissionsTreeModel.TreeItem
            {
                PermissionID = Item.PermissionID,
                PermissionParentID = Item.PermissionParentID,
                PermissionCaption = Utilities.GetValuesByLanguage(LanguageCultureCode, Item.PermissionCaption, Item.PermissionCaptionEng)
            }).ToList();
            return viewModel;
        }

        public async Task<AjaxResponse> SaveRolePermissions(PageViewModel.RolePermissionSaveSubmitModel submitModel)
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
        public class PageViewModel
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
            public class RolesGridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<RolesGridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(html: Html, keyFieldName: nameof(GridItem.RoleID));

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

            public class PermissionsTreeModel : DevExtremeGridViewModelBase, IDevExtremeTreeModel<PermissionsTreeModel.TreeItem>
            {
                #region Methods
                public TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var Tree = GetTreeWithStartupValues<TreeItem>(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    Tree
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

                    return Tree;
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