namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductImageIudDTO
    {
        #region Properties
        public int? ProductImageID { get; init; }
        public int? ProductID { get; init; }
        public string ProductImageFilename { get; init; }
        public string ProductImageAltText { get; init; }
        public int? ProductImageSortIndex { get; init; }
        #endregion
    }
}
