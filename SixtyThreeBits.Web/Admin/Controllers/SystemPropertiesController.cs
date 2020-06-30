using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
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
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.SystemProperties.Page, ViewModel);
        }

        [HttpPost]        
        [Route("")]
        public async Task<IActionResult> UpdateSystemProperies(SystemPropertiesModel.PageViewModel SubmitModel)
        {
            var ViewModel = await Model.UpdateSystemProperties(SubmitModel);
            if (ViewModel.IsSaved)
            {
                Model.ShowSuccess();
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.Page));
            }
            else
            {
                Model.ShowError();
                return View(ViewNames.Admin.SystemProperties.Page, ViewModel);
            }
        }
    }
}