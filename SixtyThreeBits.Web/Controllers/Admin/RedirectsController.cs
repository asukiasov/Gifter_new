using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{

    [Route("admin/redirects")]
    public class RedirectsController : AdminControllerBase<RedirectsModel>
    {
        #region Constructors
        public RedirectsController()
        {
            Model = new RedirectsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Redirects.Index)]
        public ActionResult Redirects()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableTemplate7(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Redirects.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Redirects.Grid)]
        public async Task<ActionResult> RedirectsGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Redirects.GridAdd)]
        public async Task<ActionResult> RedirectsGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.PageViewModel.GridModel.GridItem>() ?? new RedirectsModel.PageViewModel.GridModel.GridItem();
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Redirects.GridUpdate)]
        public async Task<ActionResult> RedirectsGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<RedirectsModel.PageViewModel.GridModel.GridItem>() ?? new RedirectsModel.PageViewModel.GridModel.GridItem();
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Redirects.GridDelete)]
        public async Task<ActionResult> RedirectsGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, redirectID: key, submitModel: new RedirectsModel.PageViewModel.GridModel.GridItem());
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