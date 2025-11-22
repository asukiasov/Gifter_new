namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class NewsPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(NewsPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(NewsPropertiesController)}{nameof(DeleteImage)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
