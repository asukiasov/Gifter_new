using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class PermissionsDataAccess : DataAccessBase
    {
        #region Contructors
        public PermissionsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? PermissionID)
        {
            await TryExecuteAsyncTask($"{nameof(DeleteRecursive)}({nameof(PermissionID)} = {PermissionID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.PermissionsDeleteRecursive(PermissionID);
                }
            });
        }

        public async Task<List<DBCoreDataContext.PermissionsListResultItem>> ListPermissions()
        {
            return await TryToReturnAsyncTask($"{nameof(ListPermissions)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.PermissionsList().OrderBy(P => P.PermissionSortIndex).ToListAsync();
                }
            });
        }

        public async Task<List<int?>> ListPermissionsByRoleID(int? RoleID)
        {
            return await TryToReturnAsyncTask($"{nameof(ListPermissionsByRoleID)}({nameof(RoleID)} = {RoleID}", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.RolePermissionsList(RoleID).Select(Item => Item.PermissionID).ToListAsync();
                }
            });
        }
        
        public async Task<int?> PermissionsIUD(Enums.DatabaseActions DatabaseAction, int? PermissionID = null, int? PermissionParentID = null, string PermissionCaption = null, string PermissionPagePath = null, string PermissionCodeName = null, string PermissionCode = null, int? PermissionSortIndex = null, bool? PermissionIsMenuItem = null, string PermissionMenuIcon = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PermissionsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PermissionID)} = {PermissionID}, {nameof(PermissionParentID)} = {PermissionParentID}, {nameof(PermissionCaption)} = {PermissionCaption}, {nameof(PermissionPagePath)} = {PermissionPagePath}, {nameof(PermissionCodeName)} = {PermissionCodeName}, {nameof(PermissionCode)} = {PermissionCode}, {nameof(PermissionSortIndex)} = {PermissionSortIndex}, {nameof(PermissionIsMenuItem)} = {PermissionIsMenuItem}, {nameof(PermissionMenuIcon)} = {PermissionMenuIcon})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    PermissionID = await db.PermissionsIUD(DatabaseAction, PermissionID, PermissionParentID, PermissionCaption, PermissionPagePath, PermissionCodeName, PermissionCode, PermissionIsMenuItem, PermissionMenuIcon, PermissionSortIndex);
                    return PermissionID;
                }
            });
        }
        #endregion
    }

    public class Permission
    {
        #region Properties
        public int? PermissionID { get; set; }
        public int? PermissionParentID { get; set; }
        public string PermissionCaption { get; set; }
        public string PermissionPagePath { get; set; }
        public string PermissionCodeName { get; set; }
        public string PermissionCode { get; set; }
        public bool PermissionIsMenuItem { get; set; }
        public string PermissionMenuIcon { get; set; }
        public int? PermissionSortIndex { get; set; }
        public bool PermissionIsSelected { get; set; }        
        #endregion
    }    
}