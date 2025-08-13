using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/team-members/{teamMemberID:int}")]
    [TypeFilter(typeof(TeamMemberFilterAttribute), Order = 2)]
    public class TeamMemberPropertiesController : AdminControllerBase<TeamMemberPropertiesModel>
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
