using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class ProjectMenuItem
    {
        #region Properties
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string NavigateUrl { get; set; }
        public bool IsHashTag => NavigateUrl == "#";
        public bool IsSelected { get; set; }
        public bool IsHomePage { get; set; }
        public bool IsTargetBlank { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<ProjectMenuItem> Children { get; set; } 
        #endregion
    }

    public class TreeNodeItem
    {
        #region Properties
        public string NodeID { get; set; }
        public string ParentID { get; set; }
        public string Filename { get; set; }
        public string NavigateUrl { get; set; }
        public bool HasNavigateUrl => !string.IsNullOrEmpty(NavigateUrl);
        public bool IsTargetBlank { get; set; }
        public string Caption { get; set; }
        public string CaptionEng { get; set; }
        public bool IsToggler1Checked { get; set; }
        public bool IsToggler2Checked { get; set; }
        public string LanguageCode { get; set; }
        public bool IsFolder { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<TreeNodeItem> Children { get; set; }

        public bool IsDisabled { get; set; }
        public bool IsNoNestingEnabled { get; set; }

        public bool ShowAddNewButton { get; set; }
        public bool ShowEditButton { get; set; }
        public bool ShowDeleteButton { get; set; }
        public bool ShowToggler1 { get; set; }
        public bool ShowToggler2 { get; set; }
        public bool ShowCustomButton { get; set; }
        public bool ShowCustomButtonFirst { get; set; }
        public bool ShowCustomButtonLast => !ShowCustomButtonFirst;

        public string CustomButtonIcon { get; set; }
        public string TextToggler1 { get; set; } = "Published?";
        public string TextToggler2 { get; set; } = "Menu?";
        #endregion        
    }

    public class SyncSortIndexesModel
    {
        #region Properties
        public List<SyncSortIndexesItem> SortIndexes { get; set; }
        #endregion
    }

    public class SuccessErrorPartialViewModel
    {
        #region Properties
        public bool IsTop { set; get; }
        public string Message { set; get; }
        public bool ShowError { set; get; }
        public bool ShowSuccess { set; get; }
        public bool IsInitialized { get; set; }
        #endregion
    }
}