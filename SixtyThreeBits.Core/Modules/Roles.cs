using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.DB.Tables;
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
        public RolesDataAccess(DBCoreDataContext db) : base(db) { }
        #endregion

        #region Methods

        public async Task<List<DB.Tables.Roles>> ListRoles()
        {
            return await TryToReturnStatic($"{nameof(ListRoles)}()", async () =>
            {
                return await db.Roles.OrderBy(Item => Item.RoleCode).ToListAsync();
            });
        }

        public async Task UpdateRolePermissions(int? RoleID, List<int?> Permissions)
        {
            await TryExecuteAsyncTask($"{nameof(UpdateRolePermissions)}({nameof(RoleID)} = {RoleID}, {nameof(Permissions)} = {Permissions.ToXml()})", async () =>
            {
                var PermissionsXml = Permissions.ToXml();                                
                await db.RolePermissionsUpdate(RoleID, PermissionsXml);                
            });
        }

        public async Task<int?> RolesIUD(byte? DatabaseAction = null, int? RoleID = null, string RoleName = null, int? RoleCode = null)
        {
            return await TryToReturnStaticAsyncTask($"{nameof(RolesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(RoleID)} = {RoleID}, {nameof(RoleName)} = {RoleName}, {nameof(RoleCode)} = {RoleCode})", async () =>
            {
                if(DatabaseAction == Enums.DatabaseActions.CREATE)
                {
                    var Role = new Roles
                    {
                        RoleName = RoleName,
                        RoleCode = RoleCode
                    };
                    await db.Roles.AddAsync(Role);
                    await db.SaveChangesAsync();
                    RoleID = Role.RoleID;
                }
                else if (DatabaseAction == Enums.DatabaseActions.UPDATE)
                {
                    var Role = await db.Roles.FirstOrDefaultAsync(Item => Item.RoleID == RoleID);
                    Role.RoleName = RoleName ?? Role.RoleName;
                    Role.RoleCode = RoleCode ?? Role.RoleCode;
                    db.Roles.Update(Role);
                    await db.SaveChangesAsync();
                }
                else if (DatabaseAction == Enums.DatabaseActions.DELETE)
                {
                    var Role = await db.Roles.FirstOrDefaultAsync(Item => Item.RoleID == RoleID);
                    db.Roles.Remove(Role);
                    await db.SaveChangesAsync();
                }

                return RoleID;
            });
        }
        #endregion
    }
}