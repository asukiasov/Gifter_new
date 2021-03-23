using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Models;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Controllers
{
    [Route("view")]
    public class FileViewerController : WebsiteControllerBase<FileViewerModel>
    {
        #region Constructors
        public FileViewerController()
        {
            Model = new FileViewerModel();
        }
        #endregion

        #region Actions
        [Route("pdf", Name = ControllerActionRouteNames.Website.FileViewer.Pdf)]
        public IActionResult PDF(string UrlPdfFile, bool CanDownload = true, string PageTitle = "PDF")
        {
            var ViewModel = Model.GetPDFViewModel(UrlPdfFile, CanDownload, PageTitle);
            return View(ViewNames.Website.FileViewer.Pdf, ViewModel);
        }
        #endregion
    }
}
