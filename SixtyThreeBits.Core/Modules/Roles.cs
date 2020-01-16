using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SixtyThreeBits.Core.Modules
{
    public class RolesDataAccess : SixtyThreeBitsDataObject
    {
        #region Methods
        public static List<RolesListResult> ListRoles(string NullText = null)
        {
            return TryToReturnStatic($"{nameof(ListRoles)}({nameof(NullText)} = {NullText})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return db.RolesList().OrderBy(Item => Item.RoleCode).ToList();
                }
            });
        }

        public void UpdateRolePermissions(int? RoleID, List<int?> Permissions)
        {
            TryExecute($"{nameof(UpdateRolePermissions)}({nameof(RoleID)} = {RoleID}, {nameof(Permissions)} = {Permissions.ToJSON()})", () =>
            {
                var PermissionsXml = new XElement("Permissions", Permissions?.Select(P => new XElement("Permission", new XElement("ID", P))));
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.RolePermissionsUpdate(RoleID, PermissionsXml);
                }
            });
        }

        public int? RolesIUD(byte? DatabaseAction = null, int? RoleID = null, string RoleName = null, int? RoleCode = null)
        {
            return TryToReturn($"{nameof(RolesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(RoleID)} = {RoleID}, {nameof(RoleName)} = {RoleName}, {nameof(RoleCode)} = {RoleCode})", () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    db.RolesIUD(DatabaseAction, ref RoleID, RoleName, RoleCode);
                    return RoleID;
                }
            });
        }
        #endregion
    }    
}