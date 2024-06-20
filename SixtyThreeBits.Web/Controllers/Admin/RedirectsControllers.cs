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
        public ActionResult Redirects()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Redirects.RedirectsView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.RedirectsController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.RedirectsController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.ViewModel.GridViewModel.GridItem>() ?? new RedirectsModel.ViewModel.GridViewModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, redirectID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.RedirectsController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.ViewModel.GridViewModel.GridItem>() ?? new RedirectsModel.ViewModel.GridViewModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, redirectID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.RedirectsController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, redirectID: key, submitModel: new RedirectsModel.ViewModel.GridViewModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }
        #endregion
    }
}