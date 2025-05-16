using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record RedirectDTO
    {
        #region Properties
        public int? RedirectID { get; init; }
        public string RedirectFrom { get; init; }
        public string RedirectTo { get; init; }
        public DateTime? RedirectDateCreated { get; init; }
        #endregion
    }

    public record RedirectIudDTO
    {
        #region Properties
        public int? RedirectID { get; init; }
        public string RedirectFrom { get; init; }
        public string RedirectTo { get; init; }
        #endregion
    }
}
