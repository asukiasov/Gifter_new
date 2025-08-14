using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PageModelBase : ModelBase
    {
        #region Properties
        public PageDTO Page { get; set; }
        #endregion
    }    
}