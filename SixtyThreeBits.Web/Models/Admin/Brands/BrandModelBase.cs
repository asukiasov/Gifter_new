using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;


namespace SixtyThreeBits.Web.Models.Admin
{
    public class BrandModelBase : ModelBase
    {
        #region Properties
        public BrandDTO DBItem { get; set; }
        #endregion
    }    
}
