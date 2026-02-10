namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            #region Nested Classes
            public static class AccountController
            {
                #region Properties
                public const string Login = "web-account-login";
                public const string ExternalLoginCallback = "web-account-external-callback";
                public const string Logout = "web-account-logout";
                public const string Register = "web-account-register";
                public const string ForgotPassword = "web-account-forgot-password";
                #endregion
            }
            #endregion
        }
    }
}
