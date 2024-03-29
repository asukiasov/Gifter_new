namespace SixtyThreeBits.Core.DTO
{
    public record CountryDTO
    {
        #region Properties
        public int? CountryID { get; init; }
        public string CountryName { get; init; }
        public string CountryNameEng { get; init; }
        public string CountryCode2 { get; init; }
        public string CountryCode3 { get; init; } 
        #endregion
    }
}
