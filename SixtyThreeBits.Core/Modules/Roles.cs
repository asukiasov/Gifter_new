using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
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

        //public void UpdateRolePermissions(int? RoleID, List<int?> Permissions)
        //{
        //    TryExecute($"{nameof(UpdateRolePermissions)}({nameof(RoleID)} = {RoleID}, {nameof(Permissions)} = {Permissions.ToJSON()})", () =>
        //    {
        //        var PermissionsXml = new XElement("Permissions", Permissions?.Select(P => new XElement("Permission", new XElement("ID", P))));
        //        using (var db = ConnectionFactory.GetDBCoreDataContext())
        //        {
        //            db.RolePermissionsUpdate(RoleID, PermissionsXml);
        //        }
        //    });
        //}

        //public int? RolesIUD(byte? DatabaseAction = null, int? RoleID = null, string RoleName = null, int? RoleCode = null)
        //{
        //    return TryToReturn($"{nameof(RolesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(RoleID)} = {RoleID}, {nameof(RoleName)} = {RoleName}, {nameof(RoleCode)} = {RoleCode})", () =>
        //    {
        //        using (var db = ConnectionFactory.GetDBCoreDataContext())
        //        {
        //            db.RolesIUD(DatabaseAction, ref RoleID, RoleName, RoleCode);
        //            return RoleID;
        //        }
        //    });
        //}
        #endregion
    }
}