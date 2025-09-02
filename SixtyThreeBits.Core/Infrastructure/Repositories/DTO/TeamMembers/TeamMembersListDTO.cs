using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
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
}
