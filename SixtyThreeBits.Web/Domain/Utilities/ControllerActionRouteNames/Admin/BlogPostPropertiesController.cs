namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class BlogPostPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(BlogPostPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(BlogPostPropertiesController)}{nameof(DeleteImage)}";
                #endregion
            }
        }
    }
}
