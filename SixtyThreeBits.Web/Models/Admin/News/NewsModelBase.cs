using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class NewsModelBase : ModelBase
    {
        #region Properties
        public NewsDTO DBItem { get; set; }
        #endregion
    }
}