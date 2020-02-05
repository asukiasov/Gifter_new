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

        public async Task CRUD(byte DatabaseAction, int? UserID, string SubmitModelJson = null)
        {
            var SubmitModel = SubmitModelJson.FromJsonTo<PageViewModel.GridModel.GridItem>() ?? new PageViewModel.GridModel.GridItem();

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
}