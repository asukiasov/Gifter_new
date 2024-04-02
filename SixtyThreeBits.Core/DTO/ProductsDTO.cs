using System;
using System.Collections.Generic;

namespace SixtyThreeBits.Core.DTO
{
    public record ProductCategoryDTO
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

    public class ProductCategoriesListDTO
    {
        #region Properties
        public int? ProductCategoryID { get; set; }
        public int? ProductCategoryParentID { get; set; }        
        public string ProductCategoryName { get; set; }        
        public int? ProductCategorySortIndex { get; set; }        
        public DateTime? ProductCategoryDateCreated { get; set; }
        #endregion        
    }

    public record ProductCategoryIudDTO
    {
        #region Properties
        public int? ProductCategoryID { get; init; }
        public int? ProductCategoryParentID { get; init; }
        public string ProductCategorySlug { get; init; }
        public string ProductCategoryName { get; init; }
        public string ProductCategoryNameEng { get; init; }
        public string ProductCategoryDescriptionShort { get; init; }
        public string ProductCategoryDescriptionShortEng { get; init; }
        public string ProductCategoryImageFilename { get; init; }
        public int? ProductCategorySortIndex { get; init; }
        public string ProductCategoryExternalID { get; init; }
        #endregion
    }

    public record ProductCategoriesListForDeleteRecursiveDTO
    {
        #region Properties
        public int? ProductCategoryID { get; init; }
        public string ProductCategoryImageFilename { get; init; }
        #endregion
    }

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
        public class ProductImage
        {
            #region Properties
            public int? ProductImageID { get; init; }
            public string ProductImageFilename { get; init; }
            #endregion
        }
        #endregion
    }

    public record ProductIudDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public int? ProductCategoryID { get; init; }
        public int? CountryIDProducer { get; init; }
        public int? BrandID { get; init; }
        public string ProductName { get; init; }
        public string ProductNameEng { get; init; }
        public string ProductSlug { get; init; }
        public decimal? ProductPrice { get; init; }
        public decimal? ProductPriceOld { get; init; }
        public decimal? ProductRemainder { get; init; }
        public string ProductImageFilename { get; init; }
        public string ProductDescriptionShort { get; init; }
        public string ProductDescriptionShortEng { get; init; }
        public string ProductDescription { get; init; }
        public string ProductDescriptionEng { get; init; }
        public bool? ProductIsPublished { get; init; }
        public bool? ProductIsFeatured { get; init; }
        public string ProductSKU { get; init; }
        public string ProductIDExternal { get; init; }
        #endregion
    }

    public record ProductsListDTO
    {
        #region Properties
        public int? ProductID { get; init; }
        public int? ProductCategoryID { get; init; }
        public string ProductName { get; init; }
        public decimal? ProductPrice { get; init; }
        public decimal? ProductPriceOld { get; init; }
        public decimal? ProductRemainder { get; init; }
        public bool ProductIsPublished { get; init; }
        public bool ProductIsFeatured { get; init; }
        public DateTime? ProductDateCreated { get; init; }
        #endregion
    }

    public record ProductImageIudDTO
    {
        #region Properties
        public int? ProductImageID { get; init; }
        public int? ProductID { get; init; }
        public string ProductImageFilename { get; init; }
        public int? ProductImageSortIndex { get; init; }
        #endregion
    }
}