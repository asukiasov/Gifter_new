namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Admin
        {
            public static class PageBuilderController
            {
                public const string Builder = $"{nameof(Admin)}{nameof(PageBuilderController)}{nameof(Builder)}";
                public const string BuilderLanguage = $"{nameof(Admin)}{nameof(PageBuilderController)}{nameof(BuilderLanguage)}";
            }
        }
    }
}
