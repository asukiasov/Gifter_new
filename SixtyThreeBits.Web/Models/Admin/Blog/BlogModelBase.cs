using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class BlogModelBase : ModelBase
    {
        #region Properties
        public BlogPostDTO DBItem { get; set; }
        #endregion
    }    
}
