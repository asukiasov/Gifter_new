namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class PagesController
            {
                public const string Pages = $"{nameof(Admin)}{nameof(PagesController)}{nameof(Pages)}";
                public const string Grid = $"{nameof(Admin)}{nameof(PagesController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(PagesController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(PagesController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(PagesController)}{nameof(GridDelete)}";
                public const string Json = $"{nameof(Admin)}{nameof(PagesController)}{nameof(Json)}";
            }
        }
    }
}
