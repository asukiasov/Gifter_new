using SixtyThreeBits.Core.Infrastructure.DTO;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class SyncSortIndexesSubmitModel
    {
        #region Properties
        public List<SyncSortIndexesDTO> SortIndexes { get; set; }
        #endregion
    }
}
