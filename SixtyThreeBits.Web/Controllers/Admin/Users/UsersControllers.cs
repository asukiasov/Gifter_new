using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users")]
    public class UsersController : AdminControllerBase<UsersModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UsersController.Users)]
        public async Task<IActionResult> Users()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Users.UsersView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UsersController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UsersController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<UsersModel.ViewModel.GridModel.GridItem>() ?? new UsersModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction:Enums.DatabaseActions.INSERT, userID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UsersController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<UsersModel.ViewModel.GridModel.GridItem>() ?? new UsersModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, userID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UsersController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, userID: key, submitModel: new UsersModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }    
}