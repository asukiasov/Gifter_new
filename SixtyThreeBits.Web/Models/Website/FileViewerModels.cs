using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Website
{
    public class FileViewerModel : ModelBase
    {
        #region Methods
        public PdfPageViewModel GetPDFViewModel(SubmitModel submitModel)
        {
            var viewModel = new PdfPageViewModel();

            viewModel.PageTitle = submitModel.PageTitle;
            viewModel.UrlPdfFile = submitModel.UrlPdfFile;
            viewModel.CanDownload = submitModel.CanDownload ?? false;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PdfPageViewModel
        {
            #region Properties
            public string PageTitle { get; set; }
            public string UrlPdfFile { get; set; }
            public bool CanDownload { get; set; }
            #endregion
        }

        public class SubmitModel
        {
            public string UrlPdfFile { get; set; }
            public bool? CanDownload { get; set; }
            public string PageTitle { get; set; } = "PDF";
        }
        #endregion
    }
}
