using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Reusables.Core
{
    #region Sub Classes
    public class ButtonAddNewViewModel
    {
        #region Properties
        public string UrlAddNew { get; set; }
        public bool HasUrlAddNew => !string.IsNullOrWhiteSpace(UrlAddNew);
        public string CssClass { get; set; } = "js-add-new-button";
        public string ButtonText { get; set; } = "დამატება";
        #endregion
    }

    public class ButtonSaveViewModel
    {
        #region Properties
        public string FormID { get; set; }
        public bool HasFormID => !string.IsNullOrWhiteSpace(FormID);
        public string CssClass { get; set; } = "js-save-button";
        public string ButtonText { get; set; } = "შენახვა";
        #endregion
    } 
    #endregion
}
