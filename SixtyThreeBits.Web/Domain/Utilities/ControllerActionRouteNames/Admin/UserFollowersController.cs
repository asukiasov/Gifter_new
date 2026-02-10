namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        #region Netsed Classes
        public static partial class Admin
        {
            #region Netsed Classes
            public static class UserFollowersController
            {
                #region Properties
                public const string Followers = $"{nameof(Admin)}{nameof(UserFollowersController)}{nameof(Followers)}";
                public const string FollowersGrid = $"{nameof(Admin)}{nameof(UserFollowersController)}{nameof(FollowersGrid)}";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
