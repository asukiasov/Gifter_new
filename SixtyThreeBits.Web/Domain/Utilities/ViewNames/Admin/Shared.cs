namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static partial class ViewNames
    {
        public static partial class Admin
        {
            public static class Shared
            {
                #region Properties
                public const string LayoutView = "~/Views/Admin/Shared/Layout.cshtml";
                public const string PopupLayoutView = "~/Views/Admin/Shared/PopupLayout.cshtml";
                public const string TabsView = "~/Views/Admin/Shared/Tabs.cshtml";
                public const string SuccessErrorToastPartialView = "~/Views/Admin/Shared/SuccessErrorToastPartialView.cshtml";

                public const string ButtonAddNewPartialView = "~/Views/Admin/Shared/ButtonAddNewPartialView.cshtml";
                public const string ButtonDeletePartialView = "~/Views/Admin/Shared/ButtonDeletePartialView.cshtml";
                public const string ButtonSavePartialView = "~/Views/Admin/Shared/ButtonSavePartialView.cshtml";
                #endregion

                #region Nested Classes
                public static class FileTreeEditor
                {
                    #region Properties
                    public const string Editor = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditor.cshtml";
                    public const string File = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                    #endregion
                }
                #endregion
            }
        }
    }
}