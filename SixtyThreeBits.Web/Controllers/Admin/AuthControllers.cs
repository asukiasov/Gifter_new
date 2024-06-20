using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin")]
    public class AuthController : ControllerBase<AuthModel>
    {
        #region Actions
        [HttpGet]
        [Route("login", Name = ControllerActionRouteNames.Admin.AuthController.Login)]
        public IActionResult Login()
        {
            if (Model.IsUserLoggedIn())
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.HomeController.Index));
            }
            else
            {
                Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
                var viewModel = Model.GetViewModel();
                return View(ViewNames.Admin.Auth.LoginView, viewModel);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthModel.ViewModel submitModel)
        {
            Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
            var viewModel = Model.GetViewModel(submitModel);
            var isAuthenticated = await Model.AuthenticateUser(viewModel: viewModel);
            if (isAuthenticated)
            {
                var urlHome = Url.RouteUrl(ControllerActionRouteNames.Admin.HomeController.Index);
                return Redirect(urlHome);
            }
            else
            {
                return View(ViewNames.Admin.Auth.LoginView, viewModel);
            }
        }

        [Route("relogin", Name = ControllerActionRouteNames.Admin.AuthController.Relogin)]
        public async Task<IActionResult> Relogin()
        {
            await Model.ReloginUser();
            return Redirect(Model.UrlPreviousPage);
        }

        [Route("logout", Name = ControllerActionRouteNames.Admin.AuthController.Logout)]
        public IActionResult Logout()
        {
            Model.Logout();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.AuthController.Login));
        }
        #endregion
    }    
}