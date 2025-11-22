namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class NewsController
            {
                public const string News = $"{nameof(Admin)}{nameof(NewsController)}{nameof(News)}";
                public const string Grid = $"{nameof(Admin)}{nameof(NewsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridDelete)}";
            }
        }
    }
}
