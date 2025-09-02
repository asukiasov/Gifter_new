using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/system-properties")]
    public class SystemPropertiesController : AdminControllerBase<SystemPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.SystemPropertiesController.SystemProperties)]
        public async Task<IActionResult> SystemProperies()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.SystemProperties.SystemPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("test-email-smtp", Name = ControllerActionRouteNames.Admin.SystemPropertiesController.TestEmailSmtp)]
        public async Task<IActionResult> TestEmailSmtp(SystemPropertiesModel.EmailSmtpTestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailSmtp(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-email-mailgun", Name = ControllerActionRouteNames.Admin.SystemPropertiesController.TestEmailMailgun)]
        public async Task<IActionResult> TestEmailMailgun(SystemPropertiesModel.EmailMailgunTestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailMailgun(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-email-office365", Name = ControllerActionRouteNames.Admin.SystemPropertiesController.TestEmailOffice365)]
        public async Task<IActionResult> TestEmailOffice365(SystemPropertiesModel.EmailOffice365TestModel SubmitModel)
        {
            var viewModel = await Model.TestEmailOffice365(SubmitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("test-aws", Name = ControllerActionRouteNames.Admin.SystemPropertiesController.TestAws)]
        public async Task<IActionResult> TestAws()
        {
            var viewModel = await Model.TestAws();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> UpdateSystemProperies(SystemPropertiesModel.ViewModel SubmitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true);
            var viewModel = await Model.Save(SubmitModel);
            if (viewModel.HasErrors)
            {
                return View(ViewNames.Admin.SystemProperties.SystemPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        } 
        #endregion
    }
}