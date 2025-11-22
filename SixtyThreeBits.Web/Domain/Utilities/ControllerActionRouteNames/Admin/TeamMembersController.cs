namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class TeamMembersController
            {
                public const string TeamMembers = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(TeamMembers)}";
                public const string Grid = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(GridDelete)}";
                public const string GridSort = $"{nameof(Admin)}{nameof(TeamMembersController)}{nameof(GridSort)}";
            }
        }
    }
}
