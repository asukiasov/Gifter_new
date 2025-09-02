using System;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record ProductDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public int? ProductCategoryID { get; init; }
        public int? CountryIDProducer { get; init; }
        public int? BrandID { get; init; }
        public string ProductName { get; init; }
        public string ProductNameEng { get; init; }
        public string ProductSlug { get; init; }
        public string ProductSlugEng { get; init; }
        public decimal? ProductPrice { get; init; }
        public decimal? ProductPriceOld { get; init; }
        public decimal? ProductRemainder { get; init; }
        public string ProductImageFilename { get; init; }
        public string ProductImageAltText { get; init; }
        public string ProductDescriptionShort { get; init; }
        public string ProductDescriptionShortEng { get; init; }
        public string ProductDescription { get; init; }
        public string ProductDescriptionEng { get; init; }
        public bool ProductIsPublished { get; init; }
        public bool ProductIsFeatured { get; init; }
        public string ProductSKU { get; init; }
        public string ProductIDExternal { get; init; }
        public DateTime? ProductDateCreated { get; init; }

        public List<ProductImage> ProductImages { get; init; }
        #endregion

        #region Nested Classes
        public record ProductImage
        {
            #region Properties
            public int? ProductImageID { get; init; }
            public string ProductImageFilename { get; init; }
            public string ProductImageAltText { get; init; }
            #endregion
        }
        #endregion
    }
}
