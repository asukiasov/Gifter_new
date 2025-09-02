using System;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record NewsListDTO
    {
        #region Properties
        public int? NewsID { get; init; }
        public string NewsTitle { get; init; }
        public DateTime? NewsDatePublished { get; init; }
        public bool NewsIsPublished { get; init; }
        public DateTime? NewsDateCreated { get; init; }
        #endregion        
    }
}
