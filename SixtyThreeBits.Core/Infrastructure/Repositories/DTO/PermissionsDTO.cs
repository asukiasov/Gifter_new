using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record PermissionDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        public int? PermissionParentID { get; init; }
        public string PermissionCaption { get; init; }
        public string PermissionCaptionEng { get; init; }
        public string PermissionPagePath { get; init; }
        public string PermissionCodeName { get; init; }
        public string PermissionCode { get; init; }
        public bool PermissionIsMenuItem { get; init; }
        public string PermissionMenuIcon { get; init; }
        public int? PermissionSortIndex { get; init; }

        public string PermissionMenuTitle { get; init; }
        public bool HasPermissionMenuTitle => !string.IsNullOrWhiteSpace(PermissionMenuTitle);
        public string PermissionMenuTitleOrCaption => HasPermissionMenuTitle ? PermissionMenuTitle : PermissionCaption;

        public string PermissionMenuTitleEng { get; init; }
        public bool HasPermissionMenuTitleEng => !string.IsNullOrWhiteSpace(PermissionMenuTitleEng);
        public string PermissionMenuTitleOrCaptionEng => HasPermissionMenuTitleEng ? PermissionMenuTitleEng : PermissionCaptionEng;

        public bool PermissionIsSelected { get; init; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{PermissionCaption} {PermissionPagePath}";
        }
        #endregion
    }

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

    public record PermissionsListDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        public int? PermissionParentID { get; init; }
        public string PermissionCaption { get; init; }
        public string PermissionCaptionEng { get; init; }
        public string PermissionPagePath { get; init; }
        public string PermissionCodeName { get; init; }
        public string PermissionCode { get; init; }
        public bool PermissionIsMenuItem { get; init; }
        public string PermissionMenuIcon { get; init; }
        public string PermissionMenuTitle { get; init; }
        public string PermissionMenuTitleEng { get; init; }
        public int? PermissionSortIndex { get; init; }
        public DateTime? PermissionDateCreated { get; init; }
        #endregion
    }

    public record PermissionsListByRoleIDDTO
    {
        #region Properties
        public int? PermissionID { get; init; }
        #endregion
    }
}
