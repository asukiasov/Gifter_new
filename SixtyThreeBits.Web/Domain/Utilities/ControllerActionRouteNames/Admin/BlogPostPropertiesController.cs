namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class BlogPostPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(BlogPostPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(BlogPostPropertiesController)}{nameof(DeleteImage)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
