namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ControllerActionRouteNames
    {
        public static partial class Website
        {
            #region Nested Classes
            public static class ContactController
            {
                #region Properties
                public const string Contact = $"{nameof(Website)}{nameof(ContactController)}{nameof(Contact)}";
                public const string ContactCulture = $"{nameof(Website)}{nameof(ContactController)}{nameof(ContactCulture)}";
                #endregion
            }
            #endregion
        }
    }
}
