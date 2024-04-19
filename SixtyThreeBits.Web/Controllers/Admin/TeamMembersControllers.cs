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
        #region Constructors
        public TeamMembersController()
        {
            Model = new TeamMembersModel();
        }
        #endregion

        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.TeamMembersController.TeamMembers)]
        public async Task<IActionResult> TeamMembers()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.TeamMembers.TeamMembersView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.TeamMembersController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.ListGridItems();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.ViewModel.GridViewModel.GridItem>() ?? new TeamMembersModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.CREATE, teamMemberID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.ViewModel.GridViewModel.GridItem>() ?? new TeamMembersModel.ViewModel.GridViewModel.GridItem();
            await Model.IUD(databaseAction: Enums.DatabaseActions.UPDATE, teamMemberID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.TeamMembersController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.IUD(databaseAction: Enums.DatabaseActions.DELETE, teamMemberID: key, submitModel: new TeamMembersModel.ViewModel.GridViewModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
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
    [TypeFilter(typeof(BeforeTeamMemberPageLoad), Order = 2)]
    public class TeamMembersPropertiesController : AdminControllerBase<TeamMemberPropertiesModel>
    {
        #region Constructors
        public TeamMembersPropertiesController()
        {
            Model = new TeamMemberPropertiesModel();
        }
        #endregion

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
