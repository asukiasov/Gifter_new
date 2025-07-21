using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/team-members")]
    public class TeamMembersController : AdminControllerBase<TeamMembersModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.TeamMembersController.TeamMembers)]
        public async Task<IActionResult> TeamMembers()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.TeamMembers.TeamMembersView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.TeamMembersController.Grid)]
        public async Task<IActionResult> Grid()
        {
            var viewModel = await Model.GetGridModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridAdd)]
        public async Task<IActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.ViewModel.GridModel.GridItem>() ?? new TeamMembersModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.INSERT, teamMemberID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridUpdate)]
        public async Task<IActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.ViewModel.GridModel.GridItem>() ?? new TeamMembersModel.ViewModel.GridModel.GridItem();
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, teamMemberID: key, submitModel: submitModel);
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridDelete)]
        public async Task<IActionResult> GridDelete(int? key)
        {
            var viewModel = await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, teamMemberID: key, submitModel: new TeamMembersModel.ViewModel.GridModel.GridItem());
            return DevExtremeGridActionResult(viewModel);
        }

        [HttpPost]
        [Route("grid/sort", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridSort)]
        public async Task<IActionResult> GridSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.Sort(submitModel);
            return Json(viewModel);
        }
        #endregion
    }

    [Route("admin/team-members/{teamMemberID:int}")]
    [TypeFilter(typeof(TeamMemberFilterAttribute), Order = 2)]
    public class TeamMembersPropertiesController : AdminControllerBase<TeamMemberPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.TeamMembersPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel(viewModel: null);
            Model.PageTitle.Set($"{Model.DBItem.TeamMemberFirstname} {Model.DBItem.TeamMemberLastname}");
            return View(ViewNames.Admin.TeamMembers.TeamMemberPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(TeamMemberPropertiesModel.ViewModel SubmitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel(viewModel: SubmitModel);

            Model.PageTitle.Set($"{Model.DBItem.TeamMemberFirstname} {Model.DBItem.TeamMemberLastname}");
            Model.Validate(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembersPropertiesController.Properties, new { teamMemberID = Model.DBItem.TeamMemberID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.TeamMembers.TeamMemberPropertiesView, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.TeamMembers.TeamMemberPropertiesView, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.TeamMembersPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
