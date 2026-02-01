using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/giftlists")]
    public class GiftListsController : AdminControllerBase<GiftListsModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.GiftListsController.GiftLists)]
        public IActionResult GiftLists()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.GiftLists.GiftListsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.GiftListsController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.GiftListsController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<GiftListsModel.ViewModel.GridModel.GridItem>() ?? new GiftListsModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, giftListID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.GiftListsController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, giftListID: key, submitModel: new GiftListsModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }
}
