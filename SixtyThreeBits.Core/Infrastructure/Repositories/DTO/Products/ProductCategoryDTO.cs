using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductCategoryDTO
    {
        #region Properties
        public int? ProductCategoryID { get; init; }
        public int? ProductCategoryParentID { get; init; }
        public string ProductCategorySlug { get; init; }
        public string ProductCategoryName { get; init; }
        public string ProductCategoryNameEng { get; init; }
        public string ProductCategoryImageFilename { get; init; }
        public int? ProductCategorySortIndex { get; init; }
        public string ProductCategoryDescriptionShort { get; init; }
        public string ProductCategoryDescriptionShortEng { get; init; }
        public DateTime? ProductCategoryDateCreated { get; init; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{ProductCategoryID} - {ProductCategoryName}";
        }
        #endregion
    }   
}