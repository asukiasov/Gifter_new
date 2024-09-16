using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    #region Nested Classes
    public class ButtonAddNewViewModel
    {
        #region Properties
        public string UrlAddNew { get; set; }
        public bool HasUrlAddNew => !string.IsNullOrWhiteSpace(UrlAddNew);
        public string CssClass { get; set; }
        public string JsSelector { get; set; } = "js-add-new-button";
        public string ButtonText { get; set; } = Resources.TextAdd;
        #endregion
    }

    public class ButtonDeleteViewModel
    {
        #region Properties
        public string CssClass { get; set; }
        public string JsSelector { get; set; } = "js-delete-button";
        public string ButtonText { get; set; } = Resources.TextDelete;
        #endregion
    }

    public class ButtonSaveViewModel
    {
        #region Properties
        public string FormID { get; set; }
        public bool HasFormID => !string.IsNullOrWhiteSpace(FormID);
        public string JsSelector { get; set; } = "js-save-button";
        public string CssClass { get; set; }
        public string ButtonText { get; set; } = Resources.TextSave;
        #endregion
    }
    #endregion
}
