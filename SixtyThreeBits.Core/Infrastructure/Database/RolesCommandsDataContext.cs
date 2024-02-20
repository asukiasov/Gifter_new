using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task<int?> RolesIUD(Enums.DatabaseActions databaseAction, int? roleID, string roleName, int? roleCode)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(RolesIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    roleID.ToSqlParameter(nameof(roleID),SqlDbType.Int,true),
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
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(RolesPermissionsUpdate),
                itemType: null,
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