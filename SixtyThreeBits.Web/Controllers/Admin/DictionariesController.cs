using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
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
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Dictionaries.Page, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree)]
        public async Task<ActionResult> DictionariesTree()
        {
            var viewModel = await Model.GetTreeModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd)]
        public async Task<ActionResult> DictionariesTreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<DictionariesModel.PageViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, dictionaryID: key, submitModel: submitModel);
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
            var submitModel = values.DeserializeJsonTo<DictionariesModel.PageViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.PageViewModel.TreeModel.TreeItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, dictionaryID: key, submitModel: submitModel);
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
            await Model.DeleteRecursive(dictionaryID: key);
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