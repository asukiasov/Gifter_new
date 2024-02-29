using System;
using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region RolesList
        public record RolesListEntity
        (
            int? RoleID,
            string RoleName,
            int? RoleCode,
            DateTime? RoleDateCreated
        );
        public IQueryable<RolesListEntity> RolesList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(RolesList)
            );
            var result = sqb.ExecuteQuery<RolesListEntity>();
            return result;
        }
        #endregion
    }
}