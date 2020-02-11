using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.DB.Tables;
using SixtyThreeBits.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class PermissionsDataAccess : DataAccessBase
    {
        #region Contructors
        public PermissionsDataAccess(DBCoreDataContext db) : base(db) { }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? PermissionID)
        {
            await TryExecuteStaticAsyncTask($"{nameof(DeleteRecursive)}({nameof(PermissionID)} = {PermissionID})", async () =>
            {
                await db.PermissionsDeleteRecursive(PermissionID);                
            });
        }

        public async Task<List<Permissions>> ListPermissions()
        {
            return await TryToReturnStaticAsyncTask($"{nameof(ListPermissions)}()", async () =>
            {
                return await db.Permissions.OrderBy(P => P.PermissionSortIndex).ToListAsync();
            });
        }

        public async Task<List<int>> ListPermissionsByRoleID(int? RoleID)
        {
            return await TryToReturnStaticAsyncTask($"{nameof(ListPermissionsByRoleID)}({nameof(RoleID)} = {RoleID}", async () =>
            {
                return await db.RolePermissions.Where(Item => Item.RoleID == RoleID).Select(Item => Item.PermissionID).ToListAsync();
            });
        }

        //public static List<Permissions> ListPermissionsWithRoleMark(int? RoleID)
        //{
        //    return TryToReturnStatic($"{nameof(ListPermissionsWithRoleMark)}({nameof(RoleID)} = {RoleID})", () =>
        //    {
        //        using (var db = ConnectionFactory.GetDBCoreDataContext())
        //        {
        //            return db.PermissionsListPermissionsWithRoleMark(RoleID).OrderBy(P => P.SortIndex).ToList();
        //        }
        //    });
        //}

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions DatabaseAction, int? PermissionID = null, int? PermissionParentID = null, string PermissionCaption = null, string PermissionPagePath = null, string PermissionCodeName = null, string PermissionCode = null, int? PermissionSortIndex = null, bool? PermissionIsMenuItem = null, string PermissionMenuIcon = null)
        {
            return await TryToReturnAsyncTask($"{nameof(PermissionsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(PermissionID)} = {PermissionID}, {nameof(PermissionParentID)} = {PermissionParentID}, {nameof(PermissionCaption)} = {PermissionCaption}, {nameof(PermissionPagePath)} = {PermissionPagePath}, {nameof(PermissionCodeName)} = {PermissionCodeName}, {nameof(PermissionCode)} = {PermissionCode}, {nameof(PermissionSortIndex)} = {PermissionSortIndex}, {nameof(PermissionIsMenuItem)} = {PermissionIsMenuItem}, {nameof(PermissionMenuIcon)} = {PermissionMenuIcon})", async () =>
            {
                if(DatabaseAction == Enums.DatabaseActions.CREATE)
                {
                    var Permission = new Permissions
                    {
                        PermissionParentID = PermissionParentID,
                        PermissionCaption = PermissionCaption,
                        PermissionPagePath = PermissionPagePath,
                        PermissionCodeName = PermissionCodeName,
                        PermissionCode = PermissionCode,
                        PermissionSortIndex = PermissionSortIndex,
                        PermissionIsMenuItem = PermissionIsMenuItem ?? false,
                        PermissionMenuIcon = PermissionMenuIcon
                    };
                    await db.Permissions.AddAsync(Permission);
                    await db.SaveChangesAsync();
                }
                else if (DatabaseAction == Enums.DatabaseActions.UPDATE)
                {
                    var Permission = await db.Permissions.FirstOrDefaultAsync(Item => Item.PermissionID == PermissionID);
                    if (Permission != null)
                    {
                        Permission.PermissionParentID = PermissionParentID == Constants.NullValueFor.Int ? null : PermissionParentID ?? Permission.PermissionParentID;
                        Permission.PermissionCaption = PermissionCaption ?? Permission.PermissionCaption;
                        Permission.PermissionPagePath = PermissionPagePath == Constants.NullValueFor.String ? null : PermissionPagePath ?? Permission.PermissionPagePath;
                        Permission.PermissionCodeName = PermissionCodeName == Constants.NullValueFor.String ? null : PermissionCodeName ?? Permission.PermissionCodeName;
                        Permission.PermissionCode = PermissionCode == Constants.NullValueFor.String ? null : PermissionCode ?? Permission.PermissionCode;
                        Permission.PermissionSortIndex = PermissionSortIndex == Constants.NullValueFor.Int ? null : PermissionSortIndex ?? Permission.PermissionSortIndex;
                        Permission.PermissionIsMenuItem = PermissionIsMenuItem ?? Permission.PermissionIsMenuItem;
                        Permission.PermissionMenuIcon = PermissionMenuIcon == Constants.NullValueFor.String ? null : PermissionMenuIcon ?? Permission.PermissionMenuIcon;

                        db.Permissions.Update(Permission);
                        await db.SaveChangesAsync();
                    }
                }                

                return PermissionID;
            });
        }
        #endregion
    }

    [Serializable]
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