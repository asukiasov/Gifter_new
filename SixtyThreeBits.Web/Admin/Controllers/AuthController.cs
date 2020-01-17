using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
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
        public ActionResult Login()
        {            
            if (Model.IsUserLoggedIn())
            {
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Page));
            }
            else
            {
                var ViewModel = Model.GetPageViewModel();
                return View(ViewNames.Admin.Auth.Login, ViewModel);
            }                        
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(AuthModel.LoginPageViewModel ViewModel)
        {
            var IsAuthenticated = await Model.AuthenticateUser(ViewModel: ViewModel);
            if (IsAuthenticated)
            {                                
                return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Home.Page));                
            }
            else
            {
                return View(ViewNames.Admin.Auth.Login, ViewModel);
            }
        }

        [Route("logout",Name =ControllerActionRouteNames.Admin.Auth.Logout)]
        public ActionResult Logout()
        {
            Model.Logout();
            return Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Auth.Login));
        }

        [Route("relogin", Name = ControllerActionRouteNames.Admin.Auth.Relogin)]
        public ActionResult Relogin()
        {
            Model.ReloginUser();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}