using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
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
}
