using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class TeamMembersModelBase : ModelBase
    {
        #region Properties        
        public TeamMemberDTO DBItem { get; set; }
        #endregion
    }
}
