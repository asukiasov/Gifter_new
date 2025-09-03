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
        [Route("properties", Name = ControllerActionRouteNames.Admin.TeamMemberPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.EnableTinyMce(true).Enable63BitsForms(true).EnableFancybox(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.GetViewModel(viewModel: null);
            return View(ViewNames.Admin.TeamMembers.TeamMemberPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(TeamMemberPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).Enable63BitsSuccessErrorToast(true);
            var viewModel = await Model.Save(submitModel);
            if (viewModel.HasErrors)
            {
                return View(ViewNames.Admin.TeamMembers.TeamMemberPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.TeamMemberPropertiesController.DeleteImage)]
        public async Task<IActionResult> DeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
