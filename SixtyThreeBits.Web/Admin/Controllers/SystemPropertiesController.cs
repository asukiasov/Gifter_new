using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/system-properties")]
    public class SystemPropertiesController : AdminControllerBase<SystemPropertiesModel>
    {
        #region Constructors
        public SystemPropertiesController()
        {
            Model = new SystemPropertiesModel();
        } 
        #endregion

        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.SystemProperties.Page)]
        public async Task<IActionResult> SystemProperies()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.SystemProperties.Page, viewModel);
        }

        [HttpPost]
        [Route("test-email-smtp", Name = ControllerActionRouteNames.Admin.SystemProperties.TestEmailSmtp)]
        public async Task<IActionResult> TestEmailSmtp(SystemPropertiesModel.EmailSmtpTestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailSmtp(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-email-mailgun", Name = ControllerActionRouteNames.Admin.SystemProperties.TestEmailMailgun)]
        public async Task<IActionResult> TestEmailMailgun(SystemPropertiesModel.EmailMailgunTestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailMailgun(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-email-office365", Name = ControllerActionRouteNames.Admin.SystemProperties.TestEmailOffice365)]
        public async Task<IActionResult> TestEmailOffice365(SystemPropertiesModel.EmailOffice365TestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailOffice365(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-aws", Name = ControllerActionRouteNames.Admin.SystemProperties.TestAws)]
        public async Task<IActionResult> TestAws()
        {
            var viewModel = await Model.TestAws();
            return Json(viewModel);
        }

        [HttpPost]        
        [Route("")]
        public async Task<IActionResult> UpdateSystemProperies(SystemPropertiesModel.PageViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true);
            var viewModel = await Model.UpdateSystemProperties(SubmitModel);
            if (viewModel.IsSaved)
            {
                Model.ShowSuccess();
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.Page));
            }
            else
            {
                Model.ShowError();
                return View(ViewNames.Admin.SystemProperties.Page, viewModel);
            }
        }

    }
}