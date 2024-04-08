using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.ViewModels.Website
{
    public class WebsiteLayoutViewModel : LayoutViewModelBase
    {
        #region Properties           
        public string UrlKa { get; set; }
        public string UrlEn { get; set; }
        public bool ShowUrlKa { get; set; }
        public bool ShowUrlEn { get; set; }
        public string ScriptsHeader { get; set; }
        public string ScriptsBodyStart { get; set; }
        public string ScriptsBodyEnd { get; set; }

        public List<ProjectMenuViewItem> FooterMenu1 { get; set; }
        public bool HasFooterMenu1 => FooterMenu1?.Any() == true;
        public List<ProjectMenuViewItem> FooterMenu2 { get; set; }
        public bool HasFooterMenu2 => FooterMenu2?.Any() == true;

        public string FacebookUrl { get; set; }
        public bool HasFacebookUrl => !string.IsNullOrWhiteSpace(FacebookUrl);
        public string InstagramUrl { get; set; }
        public bool HasInstagramUrl => !string.IsNullOrWhiteSpace(InstagramUrl);
        public string TwitterUrl { get; set; }
        public bool HasTwitterUrl => !string.IsNullOrWhiteSpace(TwitterUrl);
        public string YoutubeUrl { get; set; }
        public bool HasYoutubeUrl => !string.IsNullOrWhiteSpace(YoutubeUrl);
        public string LinkedInUrl { get; set; }
        public bool HasLinkedInUrl => !string.IsNullOrWhiteSpace(LinkedInUrl);

        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        #endregion
    }
}
