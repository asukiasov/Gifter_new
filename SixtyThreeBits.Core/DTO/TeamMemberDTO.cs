namespace SixtyThreeBits.Core.DTO
{
    public class TeamMemberDTO
    {
        #region Properties
        public int? TeamMemberID { get; set; }
        public string TeamMemberFirstname { get; set; }
        public string TeamMemberLastname { get; set; }
        public string TeamMemberFullname { get; set; }
        public string TeamMemberPosition { get; set; }
        public string TeamMemberShortDescription { get; set; }
        public string TeamMemberLongDescription { get; set; }
        public string TeamMemberImageFilename { get; set; }
        public bool TeamMemberIsPublished { get; set; }
        public int TeamMemberCategoryID { get; set; }
        public int? TeamMemberSortIndex { get; set; }
        #endregion
    }
}
