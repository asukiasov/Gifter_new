namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class AuthController
            {
                public const string Login = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Login)}";
                public const string Logout = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Logout)}";
                public const string Relogin = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Relogin)}";
            }
        }
    }
}
