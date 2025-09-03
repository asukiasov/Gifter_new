using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages/{PageID:int}/properties")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PagePropertiesController : AdminControllerBase<PagePropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PagePropertiesController.Properties)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = Model.GetViewModel(viewModel: null);
            return View(ViewNames.Admin.Pages.Page.PagePropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(PagePropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);
            if(viewModel.HasErrors)
            {
                return View(ViewNames.Admin.Pages.Page.PagePropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("delete-image", Name = ControllerActionRouteNames.Admin.PagePropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }        
        #endregion
    }    
}