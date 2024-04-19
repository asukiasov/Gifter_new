using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
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
        [Route("pdf", Name = ControllerActionRouteNames.Website.FileViewerController.Pdf)]
        public IActionResult Pdf(FileViewerModel.SubmitModel submitModel)
        {
            var viewModel = Model.GetPDFViewModel(submitModel);
            return View(ViewNames.Website.FileViewer.PdfViewerView, viewModel);
        }
        #endregion
    }
}
