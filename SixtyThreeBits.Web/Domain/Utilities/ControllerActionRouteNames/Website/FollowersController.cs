namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            #region Nested Classes
            public static class FollowersController
            {
                #region Properties
                public const string Follow = $"{nameof(Website)}{nameof(FollowersController)}{nameof(Follow)}";
                public const string Unfollow = $"{nameof(Website)}{nameof(FollowersController)}{nameof(Unfollow)}";
                public const string IsFollowing = $"{nameof(Website)}{nameof(FollowersController)}{nameof(IsFollowing)}";
                public const string Following = $"{nameof(Website)}{nameof(FollowersController)}{nameof(Following)}";
                public const string MyFollowers = $"{nameof(Website)}{nameof(FollowersController)}{nameof(MyFollowers)}";
                #endregion
            }
            #endregion
        }
    }
}
