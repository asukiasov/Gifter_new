namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class BlogPostsController
            {
                public const string BlogPosts = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(BlogPosts)}";
                public const string Grid = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridDelete)}";
            }
        }
    }
}
