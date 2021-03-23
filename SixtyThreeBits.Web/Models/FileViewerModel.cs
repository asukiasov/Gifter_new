using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Models
{
    public class FileViewerModel : WebProjectModelBase
    {
        #region Methods
        public PdfPageViewModel GetPDFViewModel(string UrlPdfFile, bool? CanDownload, string PageTitle)
        {
            var ViewModel = new PdfPageViewModel();

            ViewModel.PageTitle = PageTitle;
            ViewModel.UrlPdfFile = UrlPdfFile;
            ViewModel.CanDownload = CanDownload ?? false;

            return ViewModel;
        }
        #endregion

        #region Sub Classes
        public class PdfPageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string UrlPdfFile { get; set; }
            public bool CanDownload { get; set; }
            #endregion
        } 
        #endregion
    }
}
