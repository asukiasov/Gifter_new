using System;

namespace SixtyThreeBits.Core.DTO
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

    public record BrandListDTO
    {
        #region Properties
        public int? BrandID { get; init; }
        public string BrandName { get; init; }
        public string BrandNameEng { get; init; }
        public DateTime? BrandDateCreated { get; init; }
        #endregion
    }

    public record BrandIudDTO
    {
        #region Properties
        public int? BrandID { get; init; }
        public string BrandSlug { get; init; }
        public string BrandName { get; init; }
        public string BrandNameEng { get; init; }
        public string BrandImageFilename { get; init; }
        #endregion
    }
}
