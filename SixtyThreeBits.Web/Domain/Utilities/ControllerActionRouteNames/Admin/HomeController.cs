namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class HomeController
            {
                public const string Index = $"{nameof(Admin)}{nameof(HomeController)}{nameof(Index)}";
            }
        }
    }
}
