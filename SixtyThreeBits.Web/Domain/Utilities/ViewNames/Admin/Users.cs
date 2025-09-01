namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ViewNames
    {
        public static partial class Admin
        {
            public static class Users
            {
                #region Properties
                public const string UsersView = "~/Views/Admin/Users/UsersView.cshtml";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string UserLayout = "~/Views/Admin/Users/User/UserLayout.cshtml";
                    public const string UserPropertiesView = "~/Views/Admin/Users/User/UserPropertiesView.cshtml";
                    #endregion
                }
                #endregion
            }
        }
    }
}