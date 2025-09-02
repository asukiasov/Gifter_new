using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{   
    public record BrandListDTO
    {
        #region Properties
        public int? BrandID { get; init; }
        public string BrandName { get; init; }
        public string BrandNameEng { get; init; }
        public DateTime? BrandDateCreated { get; init; }
        #endregion
    }   
}
