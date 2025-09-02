using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users/{userID:int}/properties")]
    [TypeFilter(typeof(UserFilterAttribute), Order = 2)]
    public class UserPropertiesController : AdminControllerBase<UserPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            var viewModel = await Model.GetViewModel();
            return View(ViewNames.Admin.Users.User.UserPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(UserPropertiesModel.ViewModel submitModel)
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            var viewModel = await Model.Save(submitModel);
            if (viewModel.HasErrors)
            {
                return View(ViewNames.Admin.Users.User.UserPropertiesView, viewModel);
            }
            else
            {
                Model.ShowSuccessToastNotification();
                return Redirect(Model.UrlCurrentPageWithDomain);
            }
        }
        #endregion
    }
}