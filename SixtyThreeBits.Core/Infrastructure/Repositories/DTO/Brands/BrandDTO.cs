using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record BrandDTO
    {
        #region Properties
        public int? BrandID { get; init; }
        public string BrandName { get; init; }
        public string BrandNameEng { get; init; }
        public string BrandImageFilename { get; init; }
        public DateTime? BrandDateCreated { get; init; }
        #endregion
    }  
}
