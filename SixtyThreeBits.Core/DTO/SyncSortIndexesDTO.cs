namespace SixtyThreeBits.Core.DTO
{
    public record SyncSortIndexesDTO
    {
        #region Properties
        public int? ID { get; init; }
        public int? ParentID { get; init; }
        public int? SortIndex { get; init; }
        #endregion        
    }
}
