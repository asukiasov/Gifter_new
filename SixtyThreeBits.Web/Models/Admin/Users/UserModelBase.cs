using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UserModelBase : ModelBase
    {
        #region Properties
        public UserDTO dbItem { get; set; }
        #endregion
    }    
}
