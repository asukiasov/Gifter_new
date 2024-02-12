using System;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.Infrastructure.DTO
{
    public class ProductCategoryDTO
    {
        #region Properties
        public int? ProductCategoryID { get; set; }
        public int? ProductCategoryParentID { get; set; }
        public string ProductCategorySlug { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductCategoryNameEng { get; set; }
        public string ProductCategoryImageFilename { get; set; }
        public int? ProductCategorySortIndex { get; set; }
        public string ProductCategoryDescriptionShort { get; set; }
        public string ProductCategoryDescriptionShortEng { get; set; }
        public DateTime? ProductCategoryDateCreated { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{ProductCategoryID} - {ProductCategoryName}";
        }
        #endregion
    }

    public class ProductCategoriesListForDeleteRecursiveDTO
    {
        #region Properties
        public int? ProductCategoryID { get; set; }
        public string ProductCategoryImageFilename { get; set; }
        #endregion
    }

    public class ProductDTO
    {
        #region Properties
        public int? ProductID { get; set; }
        public int? ProductCategoryID { get; set; }
        public int? CountryIDProducer { get; set; }
        public int? BrandID { get; set; }
        public string ProductName { get; set; }
        public string ProductNameEng { get; set; }
        public string ProductSlug { get; set; }
        public string ProductSlugEng { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? ProductPriceOld { get; set; }
        public decimal? ProductRemainder { get; set; }
        public string ProductImageFilename { get; set; }
        public string ProductDescriptionShort { get; set; }
        public string ProductDescriptionShortEng { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDescriptionEng { get; set; }
        public bool ProductIsPublished { get; set; }
        public bool ProductIsFeatured { get; set; }
        public string ProductSKU { get; set; }
        public string ProductIDExternal { get; set; }
        public DateTime? ProductDateCreated { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        #endregion

        #region Nested Classes
        public class ProductImage
        {
            #region Properties
            public int? ProductImageID { get; set; }
            public string ProductImageFilename { get; set; }
            #endregion
        }
        #endregion
    }

    public record ProductsListDTO
    {
        #region Properties
        public int? ProductID { get; set; }
        public int? ProductCategoryID { get; set; }
        public int? CountryIDProducer { get; set; }
        public int? BrandID { get; set; }
        public string ProductName { get; set; }
        public string ProductNameEng { get; set; }
        public string ProductSlug { get; set; }
        public string ProductSlugEng { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? ProductPriceOld { get; set; }
        public decimal? ProductRemainder { get; set; }
        public string ProductImageFilename { get; set; }
        public bool ProductIsPublished { get; set; }
        public bool ProductIsFeatured { get; set; }
        public string ProductSKU { get; set; }
        public string ProductIDExternal { get; set; }
        public DateTime? ProductDateCreated { get; set; }
        #endregion
    }
}