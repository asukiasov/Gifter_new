using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages")]
    [TypeFilter(typeof(PagesManagementFilterAttribute), Order = 2)]
    public class PagesController : AdminControllerBase<PagesModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PagesController.Pages)]
        public IActionResult Pages()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Pages.PagesView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.PagesController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.PagesController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridModel.GridItem>() ?? new PagesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, pageID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.PagesController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<PagesModel.ViewModel.GridModel.GridItem>() ?? new PagesModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, pageID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.PagesController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.Delete(pageID: key);
            return DevExtremeGridActionResult(viewModel);
        }

        [Route("get", Name = ControllerActionRouteNames.Admin.PagesController.Get)]
        public async Task<ActionResult> Get()
        {
            var viewModel = await Model.GetPagesData();
            return Json(viewModel);
        }
        #endregion
    }    
}