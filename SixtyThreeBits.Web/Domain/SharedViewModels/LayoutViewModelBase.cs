using SixtyThreeBits.Core.Abstractions.Web;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class LayoutViewModelBase
    {
        #region Properties
        public IPageTitle PageTitle { get; set; }
        public SuccessErrorPartialViewModel SuccessErrorPartialViewModel { get; set; }
        public List<ProjectMenuItem> Menu { get; set; }
        public bool HasMenu => Menu?.Count > 0;
        public Breadcrumbs Breadcrumbs { get; set; }
        public bool ShowBreadCrumbs { get; set; }
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        public string TabsLayoutViewName { get; set; } = ViewNames.Admin.Shared.Layout;
        public string UrlLogout { get; set; }
        public PluginsClient PluginsClient { get; set; }
        public readonly string TextError = Resources.TextError;
        public readonly string TextSuccess = Resources.TextSuccess;
        #endregion
    }
}
