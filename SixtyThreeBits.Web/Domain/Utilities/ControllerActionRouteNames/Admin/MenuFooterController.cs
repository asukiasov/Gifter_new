namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public class MenuFooterController
            {
                public const string MenuFooter = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(MenuFooter)}";
                public const string Add = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(Add)}";
                public const string Update = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(Update)}";
                public const string Delete = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(Delete)}";
                public const string Sort = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(Sort)}";
                public const string Get = $"{nameof(Admin)}{nameof(MenuFooterController)}{nameof(Get)}";
            }
        }
    }
}
