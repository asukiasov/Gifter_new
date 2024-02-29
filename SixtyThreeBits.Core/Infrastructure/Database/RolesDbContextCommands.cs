using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task<int?> RolesIUD(Enums.DatabaseActions databaseAction, int? roleID, string roleName, int? roleCode)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(RolesIUD),
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    roleID.ToSqlOutputParameter(nameof(roleID),SqlDbType.Int),
                    roleName.ToSqlParameter(nameof(roleName),SqlDbType.NVarChar),
                    roleCode.ToSqlParameter(nameof(roleCode),SqlDbType.Int),
                ]
             );

            await sqb.ExecuteCommand();
            roleID = sqb.GetNextOutputParameterValue<int?>();
            return roleID;
        }

        public async Task RolesPermissionsUpdate(int? roleID, string permissionIDsJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(RolesPermissionsUpdate),
                sqlParameters:
                [
                    roleID.ToSqlParameter(nameof(roleID),SqlDbType.Int),
                    permissionIDsJson.ToSqlParameter(nameof(permissionIDsJson),SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
        }
        #endregion
    }
}