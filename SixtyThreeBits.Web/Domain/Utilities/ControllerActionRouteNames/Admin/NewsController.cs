namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class NewsController
            {
                #region Properties
                public const string News = $"{nameof(Admin)}{nameof(NewsController)}{nameof(News)}";
                public const string Grid = $"{nameof(Admin)}{nameof(NewsController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(NewsController)}{nameof(GridDelete)}"; 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
