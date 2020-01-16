using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class LayoutViewModelBase
    {
        #region Properties
        public string PageTitle { get; set; }
        public List<ProjectMenuItem> Menu { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorViewModel { get; set; }
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool HasBreadcrumbs => Breadcrumbs != null;
        public string UrlLogout { get; set; }
        #endregion
    }

    [BeforeWebProjectControllerLoaded]
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
        public DBCoreDataContext db { get; set; }
        public AppSettingsModel AppSettings { get; set; }
        public ISessionAssistance SessionAssistance { get; set; }
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
