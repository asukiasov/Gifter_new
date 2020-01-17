using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public interface IDevexpressGridModel<T> where T : class
    {
        #region Properties
        DataGridBuilder<T> InitGrid(IHtmlHelper Html); 
        #endregion
    }

    public class DevexpressTypesBase
    {
        #region Properties        
        public bool ShowAddNewButton { get; set; }
        public bool ShowUpdateButton { get; set; }
        public bool ShowDeleteButton { get; set; }

        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }
        public string UrlNodeDragDrop { get; set; }
        public string UrlDelete { get; set; }
        public string UrlList { get; set; }
        public string UrlCustomAction { get; set; }

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        #endregion        
    }

    public class DevexpressGridViewModelBase : DevexpressTypesBase
    {

    }

    public class LayoutViewModelBase
    {
        #region Properties
        public string PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorViewModel { get; set; }
        public bool IsSuccessErrorPartialInitiated => SuccessErrorViewModel != null;
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;        
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool HasBreadcrumbs => Breadcrumbs != null;
        public string UrlLogout { get; set; }
        #endregion
    }

    [BeforeWebProjectControllerLoaded(Order = 0)]
    public class WebProjectController<T> :  Controller
    {
        #region Properties
        public T Model { get; set; }        
        #endregion
    }
    
    public class WebProjectModelBase 
    {
        #region Properties
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string UrlCurrentPage { get; set; }
        public string WebsiteDomain { get; set; }
        public DataAccessFactory DataAccessFactory { get; set; }
        public AppSettingsModel AppSettings { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
        public ICookieAssistance CookieAssistance { get; set; }
        public IUrlHelper Url { get; set; }
        public string Language { get; set; }
        public User User { get; set; }
        public bool IsLoggedIn => User != null;
        #endregion

        #region Methods
        public string GetRouteByName(string RouteName, object RouteValues = null, bool GetFullPath = false, string Protocol = Constants.Protocols.HTTP)
        {
            if (GetFullPath)
            {
                return Url.RouteUrl((string.IsNullOrWhiteSpace(Language) || Language == Constants.Languages.GEORGIAN) ? RouteName : $"{RouteName}Culture", RouteValues, Protocol);
            }
            else
            {
                return Url.RouteUrl((string.IsNullOrWhiteSpace(Language) || Language == Constants.Languages.GEORGIAN) ? RouteName : $"{RouteName}Culture", RouteValues);
            }
        }
        #endregion
    }
}
