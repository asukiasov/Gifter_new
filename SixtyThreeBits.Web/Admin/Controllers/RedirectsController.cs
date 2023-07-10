using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
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
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Redirects.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Redirects.Grid)]
        public async Task<ActionResult> RedirectsGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Redirects.GridAdd)]
        public async Task<ActionResult> RedirectsGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<RedirectsModel.PageViewModel.GridModel.GridItem>() ?? new RedirectsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, RedirectID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.DeserializeJsonTo<RedirectsModel.PageViewModel.GridModel.GridItem>() ?? new RedirectsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, RedirectID: key, SubmitModel: SubmitModel);
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
            
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, RedirectID: key, SubmitModel: new RedirectsModel.PageViewModel.GridModel.GridItem());
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