using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.ViewModels.Shared
{
    public class SyncSortIndexesSubmitModel
    {
        #region Properties
        public List<SyncSortIndexesDTO> SortIndexes { get; set; }
        #endregion
    }
}
