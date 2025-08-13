namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class TeamMemberPropertiesController
            {
                #region Properties
                public const string Properties = $"{nameof(Admin)}{nameof(TeamMemberPropertiesController)}{nameof(Properties)}";
                public const string DeleteImage = $"{nameof(Admin)}{nameof(TeamMemberPropertiesController)}{nameof(DeleteImage)}";
                #endregion
            }
        }
    }
}
