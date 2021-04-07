using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
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

        public BeforePagesPageLoad()
        {            
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<PageModelBase>(FilterContext.Controller);
            var PageID = FilterContext.RouteData.Values["PageID"].ToString().ToInt();

            Model.DBItemPage = await Model.DataAccessFactory.Pages.GetSinglePageByID(PageID);
            if (Model.DBItemPage == null)
            {
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {                
                ReinitBreadCrumbs(Model);
                InitTabs(Model);
                await next();
            }            
        }

        void ReinitBreadCrumbs(PageModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
        }

        void InitTabs(PageModelBase Model)
        {
            var TabsParentID = Model.User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.Pages.Page.Root)?.PermissionID;

            if (TabsParentID != null)
            {
                var Tabs = Model.User.Permissions
                .Where(Item => Item.PermissionIsMenuItem && Item.PermissionParentID == TabsParentID)
                .OrderBy(Item => Item.PermissionSortIndex)
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.PermissionCaption,
                    NavigateUrl = Model.Url.RouteUrl(Item.PermissionCodeName, new { PageID = Model.DBItemPage.PageID }),
                    IsSelected = Regex.IsMatch(Model.UrlCurrentPage, Item.PermissionPagePath)
                }).ToList();

                if (Tabs?.Count > 0)
                {
                    var SelectedItem = Tabs.FirstOrDefault(Item => Item.IsSelected);
                    if (SelectedItem != null)
                    {
                        SelectedItem.NavigateUrl = null;
                    }
                    Model.Tabs.AddRange(Tabs);
                }
            }
        }
    }
}