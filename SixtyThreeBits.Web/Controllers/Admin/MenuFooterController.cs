using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/menu-footer")]
    [TypeFilter(typeof(BeforePagesManagementPageLoad), Order = 2)]
    public class MenuFooterController : AdminControllerBase<MenuFooterModel>
    {
        #region Constructors
        public MenuFooterController()
        {
            Model = new MenuFooterModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Page)]
        public async Task<IActionResult> MenuFooter()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableSortableJS(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.PagesManagement.MenuFooter.Page, viewModel);
        }

        [HttpPost]
        [Route("add", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Add)]
        public async Task<IActionResult> MenuFooterAdd(MenuFooterModel.SubmitModel submitModel)
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
        [Route("update", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Update)]
        public async Task<IActionResult> MenuFooterUpdate(MenuFooterModel.SubmitModel submitModel)
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
        [Route("delete", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Delete)]
        public async Task<IActionResult> MenuFooterDelete(MenuFooterModel.SubmitModel submitModel)
        {
            var viewModel = await Model.Delete(submitModel);
            return Json(viewModel);
        }

        [HttpPost]
        [Route("sort", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Sort)]
        public async Task<IActionResult> MenuFooterSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.Sort(submitModel);
            return Json(viewModel);
        }

        [HttpGet]
        [Route("get/{menuFooterID:int}", Name = ControllerActionRouteNames.Admin.PagesManagemet.MenuFooter.Get)]
        public async Task<IActionResult> MenuFooterGet(int? menuFooterID)
        {
            var viewModel = await Model.Get(menuFooterID);
            return Json(viewModel);
        }        
        #endregion
    }
}
