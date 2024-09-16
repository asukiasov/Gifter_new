using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/menu-header")]
    [TypeFilter(typeof(BeforePagesManagementPageLoad), Order = 2)]
    public class MenuHeaderController : AdminControllerBase<MenuHeaderModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.MenuHeaderController.MenuHeader)]
        public async Task<IActionResult> MenuHeader()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableSortableJS(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.MenuHeader.Page, viewModel);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.MenuHeaderController.Add)]
        public async Task<IActionResult> Add(MenuHeaderModel.SubmitModel submitModel)
        {
            var viewModel = default(AjaxResponse);
            var errors = await Model.ValidateSubmitModel(submitModel);
            if (errors.HasErrors)
            {
                viewModel = new AjaxResponse();
                viewModel.Data = errors.ErrorsJson;
            }
            else
            {
                viewModel = await Model.Add(submitModel);
            }
            return Json(viewModel);
        }

        [HttpPost]
        [Route("update", Name = ControllerActionRouteNames.Admin.MenuHeaderController.Update)]
        public async Task<IActionResult> Update(MenuHeaderModel.SubmitModel submitModel)
        {
            var viewModel = default(AjaxResponse);
            var errors = await Model.ValidateSubmitModel(submitModel);
            if (errors.HasErrors)
            {
                viewModel = new AjaxResponse();
                viewModel.Data = errors.ErrorsJson;
            }
            else
            {
                viewModel = await Model.Update(submitModel);
            }
            return Json(viewModel);
        }

        [HttpPost]
        [Route("delete", Name = ControllerActionRouteNames.Admin.MenuHeaderController.Delete)]
        public async Task<IActionResult> Delete(MenuHeaderModel.SubmitModel submitModel)
        {
            var viewModel = await Model.Delete(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("sort", Name = ControllerActionRouteNames.Admin.MenuHeaderController.Sort)]
        public async Task<IActionResult> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.Sort(submitModel);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("get/{menuHeaderID:int}", Name = ControllerActionRouteNames.Admin.MenuHeaderController.Get)]
        public async Task<IActionResult> Get(int? menuHeaderID)
        {
            var viewModel = await Model.Get(menuHeaderID);
            return Json(viewModel);
        }
        #endregion
    }
}
