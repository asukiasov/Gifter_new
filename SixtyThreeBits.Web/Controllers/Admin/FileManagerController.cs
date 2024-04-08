using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/file-manager")]
    public class FileManagerController : AdminControllerBase<FileManagerModel>
    {
        #region Constructors
        public FileManagerController()
        {
            Model = new FileManagerModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("{moduleName}", Name = ControllerActionRouteNames.Admin.FileManager.Page)]
        public IActionResult FileManager(string moduleName)
        {
            var viewModel = Model.GetPageViewModel(moduleName);
            viewModel.PluginClient.EnableJQuery(true).EnableDevextreme(true).EnableFontAwesome(true).EnableFancybox(true).EnablePreloader(true).Enable63BitsComponents(true).EnableJQueryConfirm(true);
            return View(ViewNames.Admin.FileManager.Page, viewModel);
        }

        [Route("{moduleName}/files", Name = ControllerActionRouteNames.Admin.FileManager.Files)]
        public async Task<IActionResult> FileManagerGetFile(string moduleName)
        {
            var viewModel = await Model.GetFiles(moduleName);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("{moduleName}/upload", Name = ControllerActionRouteNames.Admin.FileManager.Upload)]
        public async Task<IActionResult> FileManagerUpload(string moduleName)
        {
            var viewModel = await Model.UploadFile(moduleName);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("{moduleName}/delete", Name = ControllerActionRouteNames.Admin.FileManager.Delete)]
        public async Task<IActionResult> FileManagerDelete(string moduleName, string filename)
        {
            var viewModel = await Model.DeleteFile(moduleName, filename);
            return Json(viewModel);
        }
        #endregion
    }
}