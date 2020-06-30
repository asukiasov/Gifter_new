using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Areas.Admin.Controllers;
using SixtyThreeBits.Web.Reusables;
using SixtyThreeBits.Web.Reusables.Core;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforePagesPageLoad : IAsyncActionFilter
    {
        DataAccessFactory DataAccessFactory;

        public BeforePagesPageLoad(DataAccessFactory DataAccessFactory)
        {
            this.DataAccessFactory = DataAccessFactory;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context,ActionExecutionDelegate next)
        {
            var PageID = context.RouteData.Values["PageID"].ToString().ToInt();
            var DBItem = await DataAccessFactory.Pages.GetSinglePageByID(PageID);
            var C = context.Controller as PageController;

            if (DBItem == null)
            {
                context.Result = C.NotFoundAdmin();                
            }
            else
            {
                C.Model.Breadcrumbs.DeleteLastItem();
                C.Model.DBItemPage = DBItem;
                C.Model.DBItemPage.SetAppSettings(C.Model.AppSettings);
                InitTabs(C);
            }

            await next();
        }

        void InitTabs(PageController C)
        {
            var TabsParentID = C.Model.User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.Pages.Page.Root)?.PermissionID;

            if (TabsParentID != null)
            {
                var Tabs = C.Model.User.Permissions
                .Where(Item => Item.PermissionIsMenuItem && Item.PermissionParentID == TabsParentID)
                .OrderBy(Item => Item.PermissionSortIndex)
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.PermissionCaption,
                    NavigateUrl = C.Model.Url.RouteUrl(Item.PermissionCodeName, new { PageID = C.Model.DBItemPage.PageID }),
                    IsSelected = Regex.IsMatch(C.Model.UrlCurrentPage, Item.PermissionPagePath)
                }).ToList();

                if (Tabs?.Count > 0)
                {
                    var SelectedItem = Tabs.FirstOrDefault(Item => Item.IsSelected);
                    if (SelectedItem != null)
                    {
                        SelectedItem.NavigateUrl = null;
                    }
                    C.Model.Tabs.AddRange(Tabs);
                }
            }
        }
    }
}