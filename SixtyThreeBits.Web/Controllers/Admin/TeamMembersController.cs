using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
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
        [Route("", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersPage)]
        public async Task<IActionResult> TeamMembers()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.TeamMembers.TeamMembersPage, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGrid)]
        public async Task<ActionResult> TeamMembersGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd)]
        public async Task<ActionResult> TeamMembersGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.PageViewModel.GridModel.GridItem>() ?? new TeamMembersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, teamMemberID: key, submitModel: submitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridUpdate)]
        public async Task<ActionResult> TeamMembersGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<TeamMembersModel.PageViewModel.GridModel.GridItem>() ?? new TeamMembersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, teamMemberID: key, submitModel: submitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridDelete)]
        public async Task<ActionResult> TeamMembersGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, teamMemberID: key, submitModel: new TeamMembersModel.PageViewModel.GridModel.GridItem());
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
        [Route("grid/sort", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridSort)]
        public async Task<IActionResult> TeamMembersGridSort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = await Model.TeamMembersSyncSortIndexes(submitModel);
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
        [Route("properties", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).EnableFancybox(true);
            var viewModel = await Model.GetTeamMembersPropertiesViewModel(viewModel: null);
            Model.PageTitle.Set($"{Model.DBItem.TeamMemberFirstname} {Model.DBItem.TeamMemberLastname}");
            return View(ViewNames.Admin.TeamMembers.TeamMemberProperties, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(TeamMemberPropertiesModel.PageViewModel SubmitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var viewModel = await Model.GetTeamMembersPropertiesViewModel(viewModel: SubmitModel);

            Model.PageTitle.Set($"{Model.DBItem.TeamMemberFirstname} {Model.DBItem.TeamMemberLastname}");
            Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.SaveTeamMemberProperties(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties, new { teamMemberID = Model.DBItem.TeamMemberID }));
                }
                else
                {
                    Model.ShowErrorToastNotification();
                    result = View(ViewNames.Admin.TeamMembers.TeamMemberProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.TeamMembers.TeamMemberProperties, viewModel);
            }
            return result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMember.PropertiesDeleteImage)]
        public async Task<IActionResult> TeamMemberItemDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
