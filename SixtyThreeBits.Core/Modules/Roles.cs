using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class RolesDataAccess : DataAccessBase
    {
        #region Contructors
        public RolesDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods

        public async Task<List<DBCoreDataContext.RolesListResultItem>> ListRoles()
        {
            return await TryToReturnStatic($"{nameof(ListRoles)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.RolesList().OrderBy(Item => Item.RoleCode).ToListAsync();
                }
            });
        }

        public async Task UpdateRolePermissions(int? RoleID, List<int?> Permissions)
        {
            await TryExecuteAsyncTask($"{nameof(UpdateRolePermissions)}({nameof(RoleID)} = {RoleID}, {nameof(Permissions)} = {Permissions.ToXml()})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var PermissionsXml = Permissions.ToXml();
                    await db.RolePermissionsUpdate(RoleID, PermissionsXml);
                }
            });
        }

        public async Task<int?> RolesIUD(Enums.DatabaseActions DatabaseAction, int? RoleID = null, string RoleName = null, int? RoleCode = null)
        {
            return await TryToReturnStaticAsyncTask($"{nameof(RolesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(RoleID)} = {RoleID}, {nameof(RoleName)} = {RoleName}, {nameof(RoleCode)} = {RoleCode})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    RoleID = await db.RolesIUD(DatabaseAction, RoleID, RoleName, RoleCode);
                    return RoleID;
                }
            });
        }
        #endregion
    }
}