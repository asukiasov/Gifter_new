namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ViewNames
    {
        public static partial class Admin
        {
            public static class Shared
            {
                #region Properties
                public const string ButtonAddNewPartialView = "~/Views/Admin/Shared/ButtonAddNewPartial.cshtml";
                public const string ButtonDeletePartialView = "~/Views/Admin/Shared/ButtonDeletePartial.cshtml";
                public const string ButtonSavePartialView = "~/Views/Admin/Shared/ButtonSavePartial.cshtml";
                public const string Layout = "~/Views/Admin/Shared/Layout.cshtml";
                public const string TabsPartial = "~/Views/Admin/Shared/TabsPartial.cshtml";
                public const string SuccessErrorToastPartial = "~/Views/Admin/Shared/SuccessErrorToastPartial.cshtml";
                #endregion

                #region Nested Classes
                public static class FileTreeEditor
                {
                    #region Properties
                    public const string EditorPartial = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditorPartial.cshtml";
                    public const string FilePartial = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditorFilePartial.cshtml";
                    #endregion
                }
                #endregion
            }
        }
    }
}