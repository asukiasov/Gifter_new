namespace SixtyThreeBits.Core.DTO
{
    public class PermissionDTO
    {
        #region Properties
        public int? PermissionID { get; set; }
        public int? PermissionParentID { get; set; }
        public string PermissionCaption { get; set; }
        public string PermissionCaptionEng { get; set; }
        public string PermissionPagePath { get; set; }
        public string PermissionCodeName { get; set; }
        public string PermissionCode { get; set; }
        public bool PermissionIsMenuItem { get; set; }
        public string PermissionMenuIcon { get; set; }
        public int? PermissionSortIndex { get; set; }

        public string PermissionMenuTitle { get; set; }
        public bool HasPermissionMenuTitle => !string.IsNullOrWhiteSpace(PermissionMenuTitle);
        public string PermissionMenuTitleOrCaption => HasPermissionMenuTitle ? PermissionMenuTitle : PermissionCaption;

        public string PermissionMenuTitleEng { get; set; }
        public bool HasPermissionMenuTitleEng => !string.IsNullOrWhiteSpace(PermissionMenuTitleEng);
        public string PermissionMenuTitleOrCaptionEng => HasPermissionMenuTitleEng ? PermissionMenuTitleEng : PermissionCaptionEng;

        public bool PermissionIsSelected { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{PermissionCaption} {PermissionPagePath}";
        }
        #endregion
    }
}
