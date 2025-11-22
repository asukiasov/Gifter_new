namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class MenuHeaderController
            {
                public const string MenuHeader = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(MenuHeader)}";
                public const string Add = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(Add)}";
                public const string Update = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(Update)}";
                public const string Delete = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(Delete)}";
                public const string Sort = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(Sort)}";
                public const string Get = $"{nameof(Admin)}{nameof(MenuHeaderController)}{nameof(Get)}";
            }
        }
    }
}
