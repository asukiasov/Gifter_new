namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class RedirectsController
            {
                public const string Redirects = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(Redirects)}";
                public const string Grid = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(RedirectsController)}{nameof(GridDelete)}";
            }
        }
    }
}
