using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class UsersModel : WebProjectModelBase
    {        
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd);
            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.Roles = (await DataAccessFactory.Roles.ListRoles())?.Select(Item => new SimpleKeyValue<int?, string> { Key = Item.RoleID, Value = Item.RoleName }).ToList();
            ViewModel.Grid.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.UsersGridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var Users = (await DataAccessFactory.Users.ListUsers())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                UserID = Item.UserID,
                UserFirstname = Item.UserFirstname,
                UserLastname = Item.UserLastname,
                UserEmail = Item.UserEmail,
                UserRoleID = Item.UserRoleID,
                IsActive = Item.UserIsActive
            }).ToList();
            return Users;
        }

        public async Task ValidateUserEmail(string UserEmail, int? UserID)
        {
            var IsUniq = await DataAccessFactory.Users.IsUserEmailUniq(UserEmail, UserID);
            if(!IsUniq)
            {
                Form.AddError(Resources.ValidationUserEmailNotUniq);
            }            
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? UserID, PageViewModel.GridModel.GridItem SubmitModel)
        {            
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.Users.GetSingleUserByID(UserID);
                if (DBItem != null)
                {
                    Utilities.DeleteUploadedFile(DBItem.UserAvatarFilename);
                }
                
            }

            await DataAccessFactory.Users.UsersIUD(
                DatabaseAction: DatabaseAction,
                UserID: UserID,
                UserEmail: SubmitModel.UserEmail,
                UserPassword: SubmitModel.UserPassword,
                UserFirstname: SubmitModel.UserFirstname,
                UserLastname: SubmitModel.UserLastname,
                UserRoleID: SubmitModel.UserRoleID,
                UserIsActive: SubmitModel.IsActive
            );

            if (DataAccessFactory.Users.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase, IDevexpressGridModel<GridModel.GridItem>
            {
                #region Properties
                public List<SimpleKeyValue<int?, string>> Roles { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> InitGrid(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.UserID));

                    Grid
                    .ID("UsersGrid")                    
                    .Scrolling(Options =>
                    {
                        Options.Mode(GridScrollingMode.Standard);
                        Options.ShowScrollbar(ShowScrollbarMode.Always);
                    })
                    .OnInitialized("function(s){ UsersModel.OnUsersGridInit(s); }")
                    .OnInitNewRow($"function(s){{ s.data.{nameof(GridItem.IsActive)} = false; }}")
                    .Columns(Columns =>
                    {                        
                        Columns.AddFor(m => m.UserFirstname).Caption("Firstname").Width(150).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.UserLastname).Caption("Lastname").Width(150);
                        Columns.AddFor(m => m.UserEmail).Caption("Email").Width(200).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                            Options.AddEmail();
                        });
                        Columns.AddFor(m => m.UserPassword).Caption("Password").Width(150);

                        var UserRoleIDColumn = Columns.AddFor(m => m.UserRoleID).Caption("Role").Width(150);
                        InitLookupColumn(Column: UserRoleIDColumn, Data: Roles);

                        var IsActiveColumn = Columns.AddFor(m => m.IsActive).Caption("Active").Width(80);
                        InitCheckboxColumn(IsActiveColumn);
                        
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion

                #region Sub Classes
                public class GridItem
                {
                    #region Properties
                    public int? UserID { get; set; }                    
                    public string UserFirstname { get; set; }
                    public string UserLastname { get; set; }
                    public string UserEmail { get; set; }
                    public string UserPassword { get; set; }
                    public int? UserRoleID { get; set; }
                    public bool IsActive { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        } 
        #endregion
    }

    public class RolesModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolesGridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();            
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolesGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolesGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolesGridDelete);
            ViewModel.Grid.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolesGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolesGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolesGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolesGridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.Roles.ListRoles()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName,
                RoleCode = Item.RoleCode
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? RoleID, PageViewModel.GridModel.GridItem SubmitModel)
        {           
            await DataAccessFactory.Roles.RolesIUD(
                DatabaseAction: DatabaseAction,
                RoleID: RoleID,
                RoleName: SubmitModel.RoleName,
                RoleCode: SubmitModel.RoleCode
            );

            if (DataAccessFactory.Roles.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevexpressGridViewModelBase, IDevexpressGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> InitGrid(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.RoleID));

                    Grid
                    .ID("RolesGrid")                    
                    .OnInitialized("function(s){ RolesModel.OnRolesGridInit(s); }")                    
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.RoleName).Caption("Role").Width(150).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.RoleCode).Caption("Code").DataType(GridColumnDataType.Number).Width(150);                                                
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion


                #region Sub CLasses
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

    public class PermissionsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd);

            ViewModel.Tree = new PageViewModel.TreeModel();
            ViewModel.Tree.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd);
            ViewModel.Tree.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate);
            ViewModel.Tree.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete);
            ViewModel.Tree.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTree);
            ViewModel.Tree.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeAdd);
            ViewModel.Tree.UrlUpdate = ViewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeUpdate);
            ViewModel.Tree.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.PermissionsTreeDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.TreeModel.TreeItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.Permissions.ListPermissions()).Select(Item => new PageViewModel.TreeModel.TreeItem
            {
                PermissionID = Item.PermissionID,
                PermissionParentID = Item.PermissionParentID,
                PermissionCaption = Item.PermissionCaption,
                PermissionPagePath = Item.PermissionPagePath,
                PermissionCodeName = Item.PermissionCodeName,
                PermissionCode = Item.PermissionCode,
                PermissionIsMenuItem = Item.PermissionIsMenuItem,
                PermissionMenuIcon = Item.PermissionMenuIcon,
                PermissionSortIndex = Item.PermissionSortIndex                
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? PermissionID, PageViewModel.TreeModel.TreeItem SubmitModel)
        {
            await DataAccessFactory.Permissions.PermissionsIUD(
                DatabaseAction: DatabaseAction,
                PermissionID: PermissionID,
                PermissionParentID: SubmitModel.PermissionParentID,
                PermissionCaption: SubmitModel.PermissionCaption,
                PermissionPagePath: SubmitModel.PermissionPagePath,
                PermissionCodeName: SubmitModel.PermissionCodeName,
                PermissionCode: SubmitModel.PermissionCode,
                PermissionIsMenuItem: SubmitModel.PermissionIsMenuItem,
                PermissionMenuIcon: SubmitModel.PermissionMenuIcon,
                PermissionSortIndex: SubmitModel.PermissionSortIndex
            );

            if (DataAccessFactory.Permissions.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        public async Task DeleteRecursive(int? PermissionID)
        {
            await DataAccessFactory.Permissions.DeleteRecursive(PermissionID);
            if (DataAccessFactory.Permissions.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public TreeModel Tree { get; set; }
            public string UrlUpdate { get; set; }
            #endregion

            #region Sub Classes
            public class TreeModel : DevexpressGridViewModelBase, IDevexpressTreeModel<TreeModel.TreeItem>
            {
                #region Methods
                public TreeListBuilder<TreeItem> InitTree(IHtmlHelper Html)
                {
                    var Tree = GetTreeWithStartupValues<TreeItem>(Html: Html, KeyFieldName: nameof(TreeItem.PermissionID), ParentFieldName: nameof(TreeItem.PermissionParentID));

                    Tree
                    .ID("PermissionsTree")
                    .OnInitialized("function(e){ PermissionsModel.OnPermissionsTreeInit(e); }")
                    .OnInitNewRow($"function(e){{ e.data.{nameof(TreeItem.PermissionIsMenuItem)} = false; }}")                    
                    .RowDragging(Options =>
                    {
                        if (AllowUpdate)
                        {
                            Options.AllowDropInsideItem(true);
                            Options.AllowReordering(false);
                            Options.ShowDragIcons(true);
                            Options.OnReorder("function(e){ PermissionsModel.OnPermissionsTreeReorder(e); }");
                        }
                    })
                    .AutoExpandAll(false)
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Paging(Options=>
                    {
                        Options.Enabled(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.PermissionCaption).Caption("Caption").Width(200).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.PermissionPagePath).Caption("Page Path").Width(300);
                        Columns.AddFor(m => m.PermissionCodeName).Caption("Codename").Width(300);
                        Columns.AddFor(m => m.PermissionCode).Caption("Code").Width(250);
                        Columns.AddFor(m => m.PermissionIsMenuItem).Caption("Is Menu").DataType(GridColumnDataType.Boolean).Width(80);
                        Columns.AddFor(m => m.PermissionMenuIcon).Caption("Menu Icon fa").Width(100);
                        Columns.AddFor(m => m.PermissionSortIndex).Caption("Sort Index").DataType(GridColumnDataType.Number).Width(80);
                        Columns.Add();

                    });
                    
                    return Tree;
                }
                #endregion

                #region Sub CLasses
                public class TreeItem
                {
                    #region Properties
                    public int? PermissionID { get; set; }
                    public int? PermissionParentID { get; set; }
                    public string PermissionCaption { get; set; }
                    public string PermissionPagePath { get; set; }
                    public string PermissionCodeName { get; set; }
                    public string PermissionCode { get; set; }
                    public bool? PermissionIsMenuItem { get; set; }
                    public string PermissionMenuIcon { get; set; }
                    public int? PermissionSortIndex { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class RolePermissionsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowSaveButton = User.HasPermission(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave);
            ViewModel.UrlGetRolePermissions = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsGet);
            ViewModel.UrlSave = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsSave);

            ViewModel.RolesGrid = new PageViewModel.RolesGridModel();
            ViewModel.RolesGrid.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsRolesGrid);
            ViewModel.PermissionsTree = new PageViewModel.PermissionsTreeModel();
            ViewModel.PermissionsTree.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.RolePermissionsPermissionsTree);
              

            return ViewModel;
        }

        public async Task<AjaxResponse> GetRolePermissions(int? RoleID)
        {
            var Permissions = await DataAccessFactory.Permissions.ListPermissionsByRoleID(RoleID);
            var AR = new AjaxResponse
            {
                IsSuccess = true,
                Data = Permissions
            };
            return AR;
        }

        public async Task<List<PageViewModel.RolesGridModel.GridItem>> GetRolesGridModel()
        {
            var ViewModel = (await DataAccessFactory.Roles.ListRoles()).Select(Item => new PageViewModel.RolesGridModel.GridItem
            {
                RoleID = Item.RoleID,
                RoleName = Item.RoleName                
            }).ToList();
            return ViewModel;
        }

        public async Task<List<PageViewModel.PermissionsTreeModel.TreeItem>> GetPermissionsTreeModel()
        {
            var ViewModel = (await DataAccessFactory.Permissions.ListPermissions()).Select(Item => new PageViewModel.PermissionsTreeModel.TreeItem
            {
                PermissionID = Item.PermissionID,
                PermissionParentID = Item.PermissionParentID,
                PermissionCaption = Item.PermissionCaption                
            }).ToList();
            return ViewModel;
        }

        public async Task<AjaxResponse> SaveRolePermissions(PageViewModel.RolePermissionSaveSubmitModel SubmitModel)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Roles.UpdateRolePermissions(
                RoleID: SubmitModel.RoleID,
                Permissions: SubmitModel.PermissionIDs
            );
            AR.IsSuccess = !DataAccessFactory.Roles.IsError;
            return AR;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowSaveButton { get; set; }
            public RolesGridModel RolesGrid { get; set; }
            public PermissionsTreeModel PermissionsTree { get; set; }
            public string UrlGetRolePermissions { get; set; }
            public string UrlSave { get; set; }
            #endregion

            #region Sub Classes
            public class RolesGridModel : DevexpressGridViewModelBase, IDevexpressGridModel<RolesGridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> InitGrid(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.RoleID));

                    Grid
                    .ID("RolesGrid")
                    .OnInitialized("function(e){ RolePermissionsModel.OnRolesGridInit(e); }")                    
                    .OnFocusedRowChanged("function(e){ RolePermissionsModel.OnRolesGridFocusedRowChanged(e); }")
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
                        Columns.AddFor(m => m.RoleName).Caption("Role");
                    });


                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? RoleID { get; set; }
                    public string RoleName { get; set; }
                    #endregion
                }
                #endregion
            }

            public class PermissionsTreeModel : DevexpressGridViewModelBase, IDevexpressTreeModel<PermissionsTreeModel.TreeItem>
            {
                #region Methods
                public TreeListBuilder<TreeItem> InitTree(IHtmlHelper Html)
                {
                    var Tree = GetTreeWithStartupValues<TreeItem>(Html: Html, KeyFieldName: nameof(TreeItem.PermissionID), ParentFieldName: nameof(TreeItem.PermissionParentID));

                    Tree
                    .ID("PermissionsTree")
                    .OnInitialized("function(e){ RolePermissionsModel.OnPermissionsTreeInit(e); }")
                    .OnContentReady("function(e){ RolePermissionsModel.OnPermissionsTreeContentReady(e); }")
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
                        Columns.AddFor(m => m.PermissionCaption).Caption("Permission");

                    });

                    return Tree;
                }
                #endregion

                #region Sub CLasses
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