using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/redirects")]
    public class RedirectsController : AdminControllerBase<RedirectsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.RedirectsController.Redirects)]
        public IActionResult Redirects()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Redirects.RedirectsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.RedirectsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.RedirectsController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.ViewModel.GridModel.GridItem>() ?? new RedirectsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, redirectID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.RedirectsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.ViewModel.GridModel.GridItem>() ?? new RedirectsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, redirectID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.RedirectsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, redirectID: key, submitModel: new RedirectsModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);   
        }
        #endregion
    }
}