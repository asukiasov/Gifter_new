using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin")]
    public class AuthController : WebProjectController<AuthModel>
    {
        #region Constructors
        public AuthController()
        {
            Model = new AuthModel();
        }
        #endregion

        [HttpGet]
        [Route("login", Name = ControllerActionRouteNames.Admin.Auth.Login)]
        public IActionResult Login()
        {
            if (Model.IsUserLoggedIn())
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Page));
            }
            else
            {
                Model.PluginsClient.EnableFontAwesome(true).EnableBootstrap(true).EnableAngle(true).Enable63BitsFonts(true);
                var ViewModel = Model.GetPageViewModel();
                return View(ViewNames.Admin.Auth.Login, ViewModel);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(AuthModel.LoginPageViewModel SubmitModel)
        {
            Model.PluginsClient.EnableFontAwesome(true).EnableBootstrap(true).EnableAngle(true).Enable63BitsFonts(true);
            var ViewModel = Model.GetPageViewModel(SubmitModel);
            var IsAuthenticated = await Model.AuthenticateUser(ViewModel: ViewModel);
            if (IsAuthenticated)
            {
                var UrlHome = Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Page);
                return Redirect(UrlHome);
            }
            else
            {
                return View(ViewNames.Admin.Auth.Login, ViewModel);
            }
        }

        [Route("logout", Name = ControllerActionRouteNames.Admin.Auth.Logout)]
        public IActionResult Logout()
        {
            Model.Logout();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login));
        }

        [Route("relogin", Name = ControllerActionRouteNames.Admin.Auth.Relogin)]
        public async Task<IActionResult> Relogin()
        {
            await Model.ReloginUser();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}