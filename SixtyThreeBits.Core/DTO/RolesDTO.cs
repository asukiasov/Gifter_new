using System;

namespace SixtyThreeBits.Core.DTO
{
    public class RoleDTO
    {
        #region Properties
        public int? RoleID { get; init; }
        public string RoleName { get; init; }
        public int? RoleCode { get; init; }
        public DateTime? RoleDateCreated { get; init; }
        #endregion
    }
}
