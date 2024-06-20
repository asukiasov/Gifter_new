using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/dictionaries")]
    public class DictionariesController : AdminControllerBase<DictionariesModel>
    {
        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.DictionariesController.Dictionaries)]
        public ActionResult Dictionaries()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Dictionaries.DictionariesView, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.DictionariesController.Tree)]
        public async Task<ActionResult> Tree()
        {
            var viewModel = await Model.ListTreeItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeAdd)]
        public async Task<ActionResult> TreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<DictionariesModel.ViewModel.TreeViewModel.TreeItem>() ?? new DictionariesModel.ViewModel.TreeViewModel.TreeItem();
            await Model.IUD(DatabaseAction: Enums.DatabaseActions.CREATE, dictionaryID: key, submitModel: submitModel);
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
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeUpdate)]
        public async Task<ActionResult> TreeUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<DictionariesModel.ViewModel.TreeViewModel.TreeItem>() ?? new DictionariesModel.ViewModel.TreeViewModel.TreeItem();
            await Model.IUD(DatabaseAction: Enums.DatabaseActions.UPDATE, dictionaryID: key, submitModel: submitModel);
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
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeDelete)]
        public async Task<ActionResult> TreeDelete(int? key)
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