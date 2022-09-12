using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Models
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
        #endregion
    }
}
