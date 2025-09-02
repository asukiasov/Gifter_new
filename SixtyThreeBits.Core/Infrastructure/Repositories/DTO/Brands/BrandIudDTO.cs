namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{    
    public record BrandIudDTO
    {
        #region Properties
        public string BrandName { get; init; }
        public string BrandNameEng { get; init; }
        public string BrandImageFilename { get; init; }
        #endregion
    }
}
