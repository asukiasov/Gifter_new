using System;
using System.Collections.Generic;

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