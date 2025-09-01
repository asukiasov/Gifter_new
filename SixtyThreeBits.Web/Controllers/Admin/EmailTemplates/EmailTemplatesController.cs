using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/email-templates")]
    public class EmailTemplatesController : AdminControllerBase<EmailTemplatesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.EmailTemplatesController.EmailTemplates)]
        public IActionResult EmailTemplates()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.EmailTemplates.EmailTemplatesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.EmailTemplatesController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }
        #endregion
    }    
}