namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record RedirectIudDTO
    {
        #region Properties
        public int? RedirectID { get; init; }
        public string RedirectFrom { get; init; }
        public string RedirectTo { get; init; }
        #endregion
    }
}
