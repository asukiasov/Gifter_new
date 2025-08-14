using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductModelBase : ModelBase
    {
        #region Properties        
        public ProductDTO Product { get; set; }
        #endregion
    }    
}