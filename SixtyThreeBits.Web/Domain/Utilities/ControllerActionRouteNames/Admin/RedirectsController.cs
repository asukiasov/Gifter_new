namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class RedirectsController
            {
                #region Properties
                public const string Redirects = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(Redirects)}";
                public const string Grid = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridDelete)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}