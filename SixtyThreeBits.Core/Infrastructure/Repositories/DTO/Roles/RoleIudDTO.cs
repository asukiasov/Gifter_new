namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record RoleIudDTO
    {
        #region Properties
        public int? RoleID { get; init; }
        public string RoleName { get; init; }
        public int? RoleCode { get; init; }
        #endregion
    }
}
