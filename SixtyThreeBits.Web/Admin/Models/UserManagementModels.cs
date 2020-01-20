using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class UsersModel : WebProjectModelBase
    {
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.Grid = new PageViewModel.GridViewModel();
            ViewModel.Grid.Roles = (await DataAccessFactory.Roles.ListRoles())?.Select(Item => new SimpleKeyValue<int?, string> { Key = Item.RoleID, Value = Item.RoleName }).ToList();
            ViewModel.Grid.UrlList = Url.RouteUrl(ControllerActionRouteNames.Admin.UserManagement.UsersGrid);
            return ViewModel;
        }

        public async Task<List<PageViewModel.GridViewModel.GridItem>> GetGridViewModel()
        {
            var Users = (await DataAccessFactory.Users.ListUsers())?.Select(Item => new PageViewModel.GridViewModel.GridItem
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

        public class PageViewModel
        {
            #region Properties
            public GridViewModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridViewModel : DevexpressGridViewModelBase, IDevexpressGridModel<GridViewModel.GridItem>
            {
                #region Properties
                public List<SimpleKeyValue<int?,string>> Roles { get; set; }
                #endregion

                #region Methods
                public DataGridBuilder<GridItem> InitGrid(IHtmlHelper Html)
                {
                    return Html.DevExtreme()
                    .DataGrid<GridItem>()
                    .ID("UsersGrid")
                    .Width("100%")
                    .ShowBorders(true)
                    .FocusedRowEnabled(true)
                    .FilterRow(Options =>
                    {
                        Options.Visible(true);
                        Options.ApplyFilter(GridApplyFilterMode.Auto);
                    })
                    .DataSource(d =>
                        d.RemoteController()
                        .LoadUrl(UrlList)
                        .InsertUrl(UrlAddNew)
                        .UpdateUrl(UrlUpdate)
                        .DeleteUrl(UrlDelete)
                        .Key(nameof(GridItem.UserID))
                    )
                    .Editing(Editing => {
                          Editing.Mode(GridEditMode.Row);
                          Editing.AllowAdding(ShowAddNewButton);
                          Editing.AllowDeleting(ShowDeleteButton);
                          Editing.AllowUpdating(ShowUpdateButton);
                    })
                    .Pager(Options =>
                    {
                        Options.AllowedPageSizes(new[] { 15, 30, 50, 100});
                        Options.ShowInfo(true);
                        Options.ShowPageSizeSelector(true);
                        Options.Visible(true);
                    })
                    .Paging(Options =>
                    {                        
                        Options.Enabled(true);
                        Options.PageSize(30);                        
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.UserFirstname).Caption("Firstname").Width(100);
                        Columns.AddFor(m => m.UserLastname).Caption("Lastname").Width(100);
                        Columns.AddFor(m => m.UserEmail).Caption("Email").Width(150);
                        Columns.AddFor(m => m.UserPassword).Caption("Password").Width(100);                        
                        Columns.AddFor(m => m.UserRoleID).Caption("Role").Lookup(Options =>
                        {
                            Options.DataSource(d => d.Array().Data(Roles).Key(nameof(SimpleKeyValue<object, object>.Key))).ValueExpr(nameof(SimpleKeyValue<object, object>.Key)).DisplayExpr(nameof(SimpleKeyValue<object, object>.Value));                            
                        });
                        Columns.AddFor(m => m.IsActive).Caption("Active").Width(80); 
                        Columns.Add();
                    });
                    


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
    }
}