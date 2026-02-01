using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/orders")]
    public class OrdersController : AdminControllerBase<OrdersModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.OrdersController.Orders)]
        public IActionResult Orders()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Orders.OrdersView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.OrdersController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }
        #endregion
    }
}
