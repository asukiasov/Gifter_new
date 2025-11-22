namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class BlogPostsController
            {
                #region Properties
                public const string BlogPosts = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(BlogPosts)}";
                public const string Grid = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(BlogPostsController)}{nameof(GridDelete)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
