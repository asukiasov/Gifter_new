using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/dictionaries")]
    public class DictionariesController : AdminControllerBase<DictionariesModel>
    {
        #region Constructors
        public DictionariesController()
        {
            Model = new DictionariesModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Dictionaries.Page)]
        public ActionResult Dictionaries()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Dictionaries.Page, ViewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree)]
        public async Task<ActionResult> DictionariesTree()
        {
            var ViewModel = await Model.GetTreeModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd)]
        public async Task<ActionResult> DictionariesTreeAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<DictionariesModel.PageViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, DictionaryID: key, SubmitModel: SubmitModel);
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
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate)]
        public async Task<ActionResult> DictionariesTreeUpdate(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<DictionariesModel.PageViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, DictionaryID: key, SubmitModel: SubmitModel);
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
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete)]
        public async Task<ActionResult> DictionariesTreeDelete(int? key)
        {
            await Model.DeleteRecursive(DictionaryID: key);
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