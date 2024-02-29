using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task PermissionsDeleteRecursive(int? permissionID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PermissionsDeleteRecursive),
                sqlParameters:
                [
                    permissionID.ToSqlParameter(nameof(permissionID),SqlDbType.Int)
                ]
            );

            await sqb.ExecuteCommand();
        }

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions databaseAction, int? permissionID, int? permissionParentID, string permissionCaption, string permissionCaptionEng, string permissionPagePath, string permissionCodeName, string permissionCode, bool? permissionIsMenuItem, string permissionMenuIcon, string permissionMenuTitle, string permissionMenuTitleEng, int? permissionSortIndex)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PermissionsIUD),
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    permissionID.ToSqlOutputParameter(nameof(permissionID),SqlDbType.Int),
                    permissionParentID.ToSqlParameter(nameof(permissionParentID),SqlDbType.Int),
                    permissionCaption.ToSqlParameter(nameof(permissionCaption),SqlDbType.NVarChar),
                    permissionCaptionEng.ToSqlParameter(nameof(permissionCaptionEng),SqlDbType.NVarChar),
                    permissionPagePath.ToSqlParameter(nameof(permissionPagePath),SqlDbType.NVarChar),
                    permissionCodeName.ToSqlParameter(nameof(permissionCodeName),SqlDbType.NVarChar),
                    permissionCode.ToSqlParameter(nameof(permissionCode),SqlDbType.VarChar),
                    permissionIsMenuItem.ToSqlParameter(nameof(permissionIsMenuItem),SqlDbType.Bit),
                    permissionMenuIcon.ToSqlParameter(nameof(permissionMenuIcon),SqlDbType.NVarChar),
                    permissionMenuTitle.ToSqlParameter(nameof(permissionMenuTitle),SqlDbType.NVarChar),
                    permissionMenuTitleEng.ToSqlParameter(nameof(permissionMenuTitleEng),SqlDbType.NVarChar),
                    permissionSortIndex.ToSqlParameter(nameof(permissionSortIndex),SqlDbType.Int)
                ]
             );

            await sqb.ExecuteCommand();
            permissionID = sqb.GetNextOutputParameterValue<int?>();
            return permissionID;
        }
        #endregion
    }
}