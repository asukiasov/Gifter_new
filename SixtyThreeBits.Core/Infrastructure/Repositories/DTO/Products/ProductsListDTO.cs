using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
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
}
