using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Libraries;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class RolePermissionsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowSaveButton = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave);
            viewModel.UrlGetRolePermissions = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsGet);
            viewModel.UrlSave = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave);

            viewModel.RolesGrid = new PageViewModel.RolesGridModel();
            viewModel.RolesGrid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsRolesGrid);
            viewModel.PermissionsTree = new PageViewModel.PermissionsTreeModel();
            viewModel.PermissionsTree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsPermissionsTree);
              

            return viewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? roleID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var permissions = await repository.PermissionsListByRoleID(roleID);
            viewModel .IsSuccess = true;
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
            await repository.RolePermissionsUpdate(
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
            public string UrlGetRolePermissions { get; set; }
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
                    .OnInitialized("function(e){ rolePermissionsModel.onRolesGridInit(e); }")                    
                    .OnFocusedRowChanged("function(e){ rolePermissionsModel.onRolesGridFocusedRowChanged(e); }")
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
                    .OnInitialized("function(e){ rolePermissionsModel.onPermissionsTreeInit(e); }")
                    .OnContentReady("function(e){ rolePermissionsModel.onPermissionsTreeContentReady(e); }")
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