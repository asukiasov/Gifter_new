using System;

namespace SixtyThreeBits.Core.DTO
{
    public record TeamMemberDTO
    {
        #region Properties
        public int? TeamMemberID { get; init; }
        public string TeamMemberFirstname { get; init; }
        public string TeamMemberLastname { get; init; }
        public string TeamMemberFullname { get; init; }
        public string TeamMemberPosition { get; init; }
        public string TeamMemberShortDescription { get; init; }
        public string TeamMemberLongDescription { get; init; }
        public string TeamMemberImageFilename { get; init; }
        public bool TeamMemberIsPublished { get; init; }
        public int? TeamMemberCategoryID { get; init; }
        public int? TeamMemberSortIndex { get; init; }
        public DateTime? TeamMemberDateCreated { get; init; }
        #endregion
    }

    public record TeamMembersListDTO
    {
        #region Properties
        public int? TeamMemberID { get; init; }
        public string TeamMemberFirstname { get; init; }
        public string TeamMemberLastname { get; init; }
        public string TeamMemberPosition { get; init; }
        public bool TeamMemberIsPublished { get; init; }
        public int? TeamMemberCategoryID { get; init; }
        public int? TeamMemberSortIndex { get; init; }
        public DateTime? TeamMemberDateCreated { get; init; }
        #endregion
    }

    public record TeamMemberIudDTO
    {
        #region Properties
        public int? TeamMemberID { get; init; }
        public string TeamMemberFirstname { get; init; }
        public string TeamMemberLastname { get; init; }
        public string TeamMemberFullname { get; init; }
        public string TeamMemberPosition { get; init; }
        public string TeamMemberShortDescription { get; init; }
        public string TeamMemberLongDescription { get; init; }
        public string TeamMemberImageFilename { get; init; }
        public int? TeamMemberSortIndex { get; init; }
        public bool? TeamMemberIsPublished { get; init; }
        public int? TeamMemberCategoryID { get; init; }
        #endregion
    }
}
