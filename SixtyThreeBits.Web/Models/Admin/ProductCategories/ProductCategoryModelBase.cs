using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductCategoryModelBase : ModelBase
    {
        #region Properties
        public ProductCategoryDTO DBItem { get; set; }
        #endregion
    }   
}