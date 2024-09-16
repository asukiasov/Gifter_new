using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    public class ProjectMenuViewItem
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
        public List<ProjectMenuViewItem> Children { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Caption} - {NavigateUrl}";
        }
        #endregion
    }
}
