using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/users")]
    public class UsersController : AdminControllerBase<UsersModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UsersController.Users)]
        public async Task<ActionResult> Users()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Users.UsersView, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.UsersController.Grid)]
        public async Task<ActionResult> Grid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.UsersController.GridAdd)]
        public async Task<ActionResult> GridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();
            await Model.ValidateUserEmail(userEmail: submitModel.UserEmail, userID: key);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, userID: key, submitModel: submitModel);
                if (Model.Form.HasErrors)
                {
                    return GetDevexpressErrorResult(Model.Form.ErrorMessage);
                }
                else
                {
                    return GetDevexpressSuccessResult();
                }
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.UsersController.GridUpdate)]
        public async Task<ActionResult> GridUpdate(int? key, string values)
        {
            var result = default(ActionResult);
            var submitModel = values.DeserializeJsonTo<UsersModel.PageViewModel.GridModel.GridItem>() ?? new UsersModel.PageViewModel.GridModel.GridItem();

            await Model.ValidateUserEmail(userEmail: submitModel.UserEmail, userID: key);
            if (Model.Form.HasErrors)
            {
                result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, userID: key, submitModel: submitModel);
                if (Model.Form.HasErrors)
                {
                    result = GetDevexpressErrorResult(Model.Form.ErrorMessage);
                }
                else
                {
                    result = GetDevexpressSuccessResult();
                }
            }

            return result;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.UsersController.GridDelete)]
        public async Task<ActionResult> GridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, userID: key, submitModel: new UsersModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/users/{userID:int}/properties")]
    [TypeFilter(typeof(BeforeUserPageLoad), Order = 2)]
    public class UserPropertiesController : AdminControllerBase<UserPropertiesModel>
    {
        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.UserPropertiesController.Properties)]
        public async Task<IActionResult> Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            var viewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Users.User.UserPropertiesView, viewModel);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(UserPropertiesModel.PageViewModel viewModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).Enable63BitsSuccessErrorToast(true).EnableDevextreme(true).EnableJQueryMaskedInput(true);
            viewModel = await Model.GetPageViewModel(viewModel);

            await Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsValid)
                {
                    Model.ShowSuccessToastNotification();
                    result = Redirect(Model.UrlCurrentPageWithDomain);                    
                }
                else
                {
                    Model.ShowErrorToastNotification(viewModel.ErrorMessage);
                    result = base.View(ViewNames.Admin.Users.User.UserPropertiesView, viewModel);                    
                }
            }
            else
            {
                result = base.View(ViewNames.Admin.Users.User.UserPropertiesView, viewModel);
            }

            return result;
        }
        #endregion
    }
}