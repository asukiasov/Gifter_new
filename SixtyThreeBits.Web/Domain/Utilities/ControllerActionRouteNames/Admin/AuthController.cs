namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Nested Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class AuthController
            {
                #region Properties
                public const string Login = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Login)}";
                public const string Logout = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Logout)}";
                public const string Relogin = $"{nameof(Admin)}{nameof(AuthController)}{nameof(Relogin)}";
                #endregion
            }
            #endregion
        } 
        #endregion
    }
}
