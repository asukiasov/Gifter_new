namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class UsersController
            {
                public const string Users = $"{nameof(Admin)}{nameof(UsersController)}{nameof(Users)}";
                public const string Grid = $"{nameof(Admin)}{nameof(UsersController)}{nameof(Grid)}";
                public const string GridAdd = $"{nameof(Admin)}{nameof(UsersController)}{nameof(GridAdd)}";
                public const string GridUpdate = $"{nameof(Admin)}{nameof(UsersController)}{nameof(GridUpdate)}";
                public const string GridDelete = $"{nameof(Admin)}{nameof(UsersController)}{nameof(GridDelete)}";
            }
        }
    }
}
