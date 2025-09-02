namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductCategoriesListForDeleteRecursiveDTO
    {
        #region Properties
        public int? ProductCategoryID { get; init; }
        public string ProductCategoryImageFilename { get; init; }
        #endregion
    }
}
