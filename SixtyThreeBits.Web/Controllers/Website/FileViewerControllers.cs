using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website;

namespace SixtyThreeBits.Web.Controllers.Website
{
    [Route("view")]
    public class PdfFileViewerController : WebsiteControllerBase<PdfFileViewerModel>
    {
        #region Actions
        [Route("pdf", Name = ControllerActionRouteNames.Website.PdfFileViewerController.PdfFileViewer)]
        public IActionResult PdfFileViewer(PdfFileViewerModel.SubmitModel submitModel)
        {
            var viewModel = Model.GetViewModel(submitModel);
            return View(ViewNames.Website.FileViewer.PdfViewerView, viewModel);
        }
        #endregion
    }
}
