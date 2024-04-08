using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    public class NotFoundViewModel
    {
        #region Properties
        public PluginsClientViewModel PluginsClient { get; set; }
        public string UrlLogout { get; set; }
        public readonly string PageTitle = Resources.TextPageNotFound;
        public readonly string TextPageNotFoundMessage1 = Resources.TextPageNotFoundMessage1;
        public readonly string TextPageNotFoundMessage2 = Resources.TextPageNotFoundMessage2;
        public readonly string TextBackToPreviousPage = Resources.TextBackToPreviousPage;
        public readonly string TextGetMeOuttaHere = Resources.TextGetMeOuttaHere;
        #endregion
    }
}
