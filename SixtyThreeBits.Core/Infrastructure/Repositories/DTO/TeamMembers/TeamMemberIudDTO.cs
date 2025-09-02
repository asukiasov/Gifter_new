namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
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
