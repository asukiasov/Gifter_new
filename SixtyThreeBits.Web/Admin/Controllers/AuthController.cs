using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin")]
    public class LoginController : WebProjectControllerBase<LoginModel>
    {
        #region Constructors
        public LoginController()
        {
            Model = new LoginModel();
        }
        #endregion

        #region Actions
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
                Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
                var viewModel = Model.GetPageViewModel();
                return View(ViewNames.Admin.Auth.Login, viewModel);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginModel.PageViewModel submitModel)
        {
            Model.PluginsClient.EnableAdminTheme(true).EnableFontAwesome(true).Enable63BitsFonts(true);
            var viewModel = Model.GetPageViewModel(submitModel);
            var isAuthenticated = await Model.AuthenticateUser(viewModel: viewModel);
            if (isAuthenticated)
            {
                var urlHome = Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Page);
                return Redirect(urlHome);
            }
            else
            {
                return View(ViewNames.Admin.Auth.Login, viewModel);
            }
        }

        [Route("relogin", Name = ControllerActionRouteNames.Admin.Auth.Relogin)]
        public async Task<IActionResult> Relogin()
        {
            await Model.ReloginUser();
            return Redirect(Model.UrlPreviousPage);
        } 
        #endregion
    }

    [Route("admin")]
    public class LogoutController : WebProjectControllerBase<LogoutModel>
    {
        #region Constructors
        public LogoutController()
        {
            Model = new LogoutModel();
        }
        #endregion

        #region Actions
        [Route("logout", Name = ControllerActionRouteNames.Admin.Auth.Logout)]
        public IActionResult Logout()
        {
            Model.Logout();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login));
        } 
        #endregion
    }
}