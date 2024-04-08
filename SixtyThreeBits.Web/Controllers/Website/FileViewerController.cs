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
        [Route("pdf", Name = ControllerActionRouteNames.Website.FileViewer.Pdf)]
        public IActionResult PDF(FileViewerModel.SubmitModel submitModel)
        {
            var viewModel = Model.GetPDFViewModel(submitModel);
            return View(ViewNames.Website.FileViewer.Pdf, viewModel);
        }
        #endregion
    }
}
