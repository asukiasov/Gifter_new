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
        #region Constructors
        public MenuHeaderController()
        {
            Model = new MenuHeaderModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Page)]
        public async Task<IActionResult> MenuHeader()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableSortableJS(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.PagesManagement.MenuHeader.Page, viewModel);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Add)]
        public async Task<IActionResult> MenuHeaderAdd(MenuHeaderModel.SubmitModel submitModel)
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
        [Route("update", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Update)]
        public async Task<IActionResult> MenuHeaderUpdate(MenuHeaderModel.SubmitModel submitModel)
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
        [Route("delete", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Delete)]
        public async Task<IActionResult> MenuHeaderDelete(MenuHeaderModel.SubmitModel submitModel)
        {
            var viewModel = await Model.Delete(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("sort", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Sort)]
        public async Task<IActionResult> MenuHeaderSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.Sort(submitModel);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("get/{menuHeaderID:int}", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuHeader.Get)]
        public async Task<IActionResult> MenuHeaderGet(int? menuHeaderID)
        {
            var viewModel = await Model.Get(menuHeaderID);
            return Json(viewModel);
        }
        #endregion
    }
}
