using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/file-manager")]
    public class FileManagerController : AdminControllerBase<FileManagerModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.FileManagerController.FileManager)]
        public IActionResult FileManager()
        {
            var viewModel = Model.GetViewModel();
            viewModel.PluginClient.EnableJQuery(true).EnableDevextreme(true).EnableFontAwesome(true).EnableFancybox(true).EnablePreloader(true).Enable63BitsComponents(true).EnableJQueryConfirm(true);
            return View(ViewNames.Admin.FileManager.FileManagerView, viewModel);
        }

        [Route("files", Name = ControllerActionRouteNames.Admin.FileManagerController.Files)]
        public async Task<IActionResult> FileManagerGetFile()
        {
            var viewModel = await Model.GetFiles();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("upload", Name = ControllerActionRouteNames.Admin.FileManagerController.Upload)]
        public async Task<IActionResult> FileManagerUpload()
        {
            var viewModel = await Model.UploadFile();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.FileManagerController.Delete)]
        public async Task<IActionResult> FileManagerDelete(string filename)
        {
            var viewModel = await Model.DeleteFile(filename);
            return Json(viewModel);
        }
        #endregion
    }
}