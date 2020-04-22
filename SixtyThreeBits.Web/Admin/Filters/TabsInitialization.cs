using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Reusables.Core;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class TabsInitialization : ActionFilterAttribute
    {
        public string ParentRoute { get; set; }

        public override void OnActionExecuted(ActionExecutedContext FilterContext)
        {

        }

        public override void OnActionExecuting(ActionExecutingContext FilterContext)
        {
            if (!LocalUtilities.IsAjaxRequest(FilterContext.HttpContext.Request))
            {
                var Model = LocalUtilities.GetWebProjectModelBaseFromController(FilterContext.Controller);
                InitTabs(Model);
            }
        }

        void InitTabs(WebProjectModelBase Model)
        {
            var TabsParentID = Model.User.Permissions.FindLast(Item => Item.PermissionCodeName == ParentRoute)?.PermissionID;

            if (TabsParentID != null)
            {
                var Tabs =  Model.User.Permissions
                .Where(Item => Item.PermissionIsMenuItem && Item.PermissionParentID == TabsParentID)
                .OrderBy(Item => Item.PermissionSortIndex)                
                .Select(Item => new ProjectMenuItem
                {
                    Caption = Item.PermissionCaption,
                    NavigateUrl = Model.Url.RouteUrl(Item.PermissionCodeName),
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