namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record PermissionIudDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        public int? PermissionParentID { get; init; }
        public string PermissionCaption { get; init; }
        public string PermissionCaptionEng { get; init; }
        public string PermissionPagePath { get; init; }
        public string PermissionCodeName { get; init; }
        public string PermissionCode { get; init; }
        public bool? PermissionIsMenuItem { get; init; }
        public string PermissionMenuIcon { get; init; }
        public string PermissionMenuTitle { get; init; }
        public string PermissionMenuTitleEng { get; init; }
        public int? PermissionSortIndex { get; init; }
        #endregion
    }   
}
