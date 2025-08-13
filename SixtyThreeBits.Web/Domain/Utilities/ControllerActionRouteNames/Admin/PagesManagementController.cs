namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class PagesManagementController
            {
                #region Properties
                public const string PagesManagement = $"{nameof(Admin)}{nameof(PagesManagementController)}{nameof(PagesManagement)}";
                #endregion
            }
        }
    }
}
