using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BeforePagesManagementPageLoad : IAsyncActionFilter
    {
        #region Properties
        ModelBase _model;
        #endregion

        #region Methods
        public BeforePagesManagementPageLoad()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<ModelBase>(filterContext.Controller);
            if (!_model.IsAjaxRequest)
            {
                reinitBreadCrumbs();
                initTabs();
            }
            await next();
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeletePreLastItem();
        }

        void initTabs()
        {
            var tabsParentID = _model.User.Permissions.FirstOrDefault(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.PagesManagemet.Root)?.PermissionID;

            if (tabsParentID != null)
            {
                var tabs = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == tabsParentID)
                .OrderBy(item => item.PermissionSortIndex)
                .Select(item => new ProjectMenuItem
                {
                    Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionMenuTitleOrCaption, item.PermissionMenuTitleOrCaptionEng),
                    NavigateUrl = _model.Url.RouteUrl(item.PermissionCodeName),
                    IsSelected = Regex.IsMatch(_model.UrlCurrentPageWithDomain, item.PermissionPagePath)
                }).ToList();

                if (tabs?.Count > 0)
                {
                    var selectedItem = tabs.FirstOrDefault(Item => Item.IsSelected);
                    if (selectedItem != null)
                    {
                        selectedItem.NavigateUrl = null;
                    }
                    _model.Tabs.AddRange(tabs);
                }
            }
        }
        #endregion
    }
}