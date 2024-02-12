using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforePagesPageLoad : IAsyncActionFilter
    {
        #region Properties
        PageModelBase _model;
        #endregion

        #region Methods
        public BeforePagesPageLoad()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = LocalUtilities.GetModelFromController<PageModelBase>(filterContext.Controller);
            var pageID = filterContext.RouteData.Values[Constants.RouteValues.PageID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetPagesRepository();
            _model.DBItem = await repository.PagesGetSingleByID(pageID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                reinitBreadCrumbs();
                initTabs();
                await next();
            }
        }

        void reinitBreadCrumbs()
        {            
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.PageTitle);
        }

        void initTabs()
        {
            var tabsParentID = _model.User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.Pages.Page.Root)?.PermissionID;

            if (tabsParentID != null)
            {
                var tabs = _model.User.Permissions
                .Where(item => item.PermissionIsMenuItem && item.PermissionParentID == tabsParentID)
                .OrderBy(item => item.PermissionSortIndex)
                .Select(item => new ProjectMenuItem
                {
                    Caption = _model.Utilities.GetValuesByLanguage(_model.LanguageCultureCode, item.PermissionMenuTitleOrCaption, item.PermissionMenuTitleOrCaptionEng),
                    NavigateUrl = _model.Url.RouteUrl(item.PermissionCodeName, new { pageID = _model.DBItem.PageID }),
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