using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Controllers.Website.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Website.Account;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Website
{
    public class AccountController : WebsiteControllerBase<AccountModel>
    {
        #region Actions

        [Route("account/login")]
        [Route($"{RouteValueLanguageCode}/account/login", Name = ControllerActionRouteNames.Website.AccountController.Login)]
        public IActionResult Login(string returnUrl = null)
        {
            var existingUser = Model.User;

            if (existingUser != null)
            {
                return Redirect($"/{Model.LanguageCultureCode}/dashboard");
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback), new { returnUrl }),
                Items = { { "returnUrl", returnUrl ?? "/" } }
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [Route("account/external-login-callback")]
        [Route($"{RouteValueLanguageCode}/account/external-login-callback", Name = ControllerActionRouteNames.Website.AccountController.ExternalLoginCallback)]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded || result.Principal == null)
            {
                return Redirect("/?error=auth_failed");
            }

            var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var fullName = result.Principal.FindFirstValue(ClaimTypes.Name);
            var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName);
            var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname);
            var pictureUrl = result.Principal.FindFirstValue("picture");

            if (string.IsNullOrEmpty(googleId) || string.IsNullOrEmpty(email))
            {
                return Redirect("/?error=missing_claims");
            }

            var user = await Model.AuthenticateGoogleUserAsync(
                googleId,
                email,
                fullName,
                firstName,
                lastName,
                pictureUrl
            );

            if (user == null)
            {
                return Redirect("/?error=user_creation_failed");
            }

            Model.StoreUserSession(user);

            var redirectUrl = returnUrl ?? $"/{Model.LanguageCultureCode}/dashboard";
            return Redirect(redirectUrl);
        }

        [Route("account/logout")]
        [Route($"{RouteValueLanguageCode}/account/logout", Name = ControllerActionRouteNames.Website.AccountController.Logout)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Model.ClearUserSession();
            return Redirect("/");
        }

        [Route("account/register")]
        [Route($"{RouteValueLanguageCode}/account/register", Name = ControllerActionRouteNames.Website.AccountController.Register)]
        public IActionResult Register() => NotFound();

        [Route("account/forgot-password")]
        [Route($"{RouteValueLanguageCode}/account/forgot-password", Name = ControllerActionRouteNames.Website.AccountController.ForgotPassword)]
        public IActionResult ForgotPassword() => NotFound();

        #endregion
    }
}
