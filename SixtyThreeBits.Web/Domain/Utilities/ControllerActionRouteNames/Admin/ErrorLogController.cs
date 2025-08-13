namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class ErrorLogController
            {
                #region Properties
                public const string ErrorLog = $"{nameof(Admin)}{nameof(ErrorLogController)}{nameof(ErrorLog)}";
                public const string Clear = $"{nameof(Admin)}{nameof(ErrorLogController)}{nameof(Clear)}";
                #endregion
            }
        }
    }
}
