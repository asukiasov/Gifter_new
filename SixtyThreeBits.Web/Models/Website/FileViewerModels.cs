using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Website
{
    public class PdfFileViewerModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel(SubmitModel submitModel)
        {
            var viewModel = new ViewModel();

            viewModel.PageTitle = submitModel.PageTitle;
            viewModel.UrlPdfFile = submitModel.UrlPdfFile;
            viewModel.CanDownload = submitModel.CanDownload ?? false;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
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
