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
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.DictionariesController.Dictionaries)]
        public IActionResult Dictionaries()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetViewModel();
            return View(ViewNames.Admin.Dictionaries.DictionariesView, viewModel);
        }

        [Route("tree", Name = ControllerActionRouteNames.Admin.DictionariesController.Tree)]
        public async Task<IActionResult> Tree()
        {
            var viewModel = await Model.GetTreeItems();
            return DevExtremeGridResult(viewModel);
        }

        [HttpPost]
        [Route("tree/add", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeAdd)]
        public async Task<IActionResult> TreeAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<DictionariesModel.ViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.ViewModel.TreeModel.TreeItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, dictionaryID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("tree/update", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeUpdate)]
        public async Task<IActionResult> TreeUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<DictionariesModel.ViewModel.TreeModel.TreeItem>() ?? new DictionariesModel.ViewModel.TreeModel.TreeItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, dictionaryID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("tree/delete", Name = ControllerActionRouteNames.Admin.DictionariesController.TreeDelete)]
        public async Task<IActionResult> TreeDelete(int? key)
        {
            var viewModel = await Model.DeleteRecursive(dictionaryID: key);
            return DevExtremeGridActionResult(viewModel);
        }
        #endregion
    }
}