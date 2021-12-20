using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
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
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.TeamMembers.TeamMembersPage, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGrid)]
        public async Task<ActionResult> TeamMembersGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersGridAdd)]
        public async Task<ActionResult> TeamMembersGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<TeamMembersModel.PageViewModel.GridModel.GridItem>() ?? new TeamMembersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, TeamMemberID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.DeserializeJsonTo<TeamMembersModel.PageViewModel.GridModel.GridItem>() ?? new TeamMembersModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, TeamMemberID: key, SubmitModel: SubmitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, TeamMemberID: key, SubmitModel: new TeamMembersModel.PageViewModel.GridModel.GridItem());
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
        [Route("grid/sync-sort-indexes", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMembersSyncSortIndexes)]
        public async Task<IActionResult> TeamMembersSyncSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var ViewModel = await Model.TeamMembersSyncSortIndexes(SubmitModel);
            return Json(ViewModel);
        }
        #endregion
    }

    [Route("admin/team-members/{TeamMemberID:int}")]
    [TypeFilter(typeof(BeforeTeamMemberPageLoad), Order = 2)]
    public class TeamMembersPropertiesController : AdminControllerBase<TeamMemberPropertiesModel>
    {
        #region Constructors
        public TeamMembersPropertiesController()
        {
            Model = new TeamMemberPropertiesModel();
        }
        #endregion

        #region TeamMember Properties
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).EnableFancybox(true);
            var ViewModel = await Model.GetTeamMembersPropertiesViewModel(ViewModel: null);
            Model.PageTitle.Set($"{Model.DBItemTeamMember.TeamMemberFirstname} {Model.DBItemTeamMember.TeamMemberLastname}");
            return View(ViewNames.Admin.TeamMembers.TeamMember, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(TeamMemberPropertiesModel.TeamMembersPropertiesViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var ViewModel = await Model.GetTeamMembersPropertiesViewModel(ViewModel: SubmitModel);

            Model.PageTitle.Set($"{Model.DBItemTeamMember.TeamMemberFirstname} {Model.DBItemTeamMember.TeamMemberLastname}");
            Model.ValidateTeamMemberPropertiesViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveTeamMemberProperties(ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.TeamMembers.TeamMember.Properties, new { TeamMemberID = Model.DBItemTeamMember.TeamMemberID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.TeamMembers.TeamMember, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.TeamMembers.TeamMember, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.TeamMembers.TeamMember.TeamMembersItemDeleteImage)]
        public async Task<IActionResult> TeamMemberItemDeleteImage(int? TeamMemberID)
        {
            var Result = await Model.DeleteImage(TeamMemberID);
            return Json(Result);
        }
        #endregion
    }
}
