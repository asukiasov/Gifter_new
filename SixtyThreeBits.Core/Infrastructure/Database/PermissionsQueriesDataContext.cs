using System;
using System.Data;
using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region PermissionsList
        public record PermissionsListEntity
        (
            int? PermissionID,
            int? PermissionParentID,
            string PermissionCaption,
            string PermissionCaptionEng,
            string PermissionPagePath,
            string PermissionCodeName,
            string PermissionCode,
            bool PermissionIsMenuItem,
            string PermissionMenuIcon,
            string PermissionMenuTitle,
            string PermissionMenuTitleEng,
            int? PermissionSortIndex,
            DateTime? PermissionDateCreated
        );
        public IQueryable<PermissionsListEntity> PermissionsList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(PermissionsList),
                itemType: typeof(PermissionsListEntity)
            );
            var result = sqb.ExecuteQuery<PermissionsListEntity>();
            return result;
        }
        #endregion

        #region PermissionsListByRoleID
        public record PermissionsListByRoleIDItem
        {
            #region Properties
            public int? PermissionID { get; init; }
            #endregion
        }
        public IQueryable<PermissionsListByRoleIDItem> PermissionsListByRoleID(int? roleID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(PermissionsListByRoleID),
                itemType: typeof(PermissionsListByRoleIDItem),
                sqlParameters:
                [
                    roleID.ToSqlParameter(nameof(roleID), SqlDbType.Int)
                ]
            );
            var result = sqb.ExecuteQuery<PermissionsListByRoleIDItem>();
            return result;
        }
        #endregion
    }
}