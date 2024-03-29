using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PermissionsRepository : RepositoryBase
    {
        #region Contructors
        public PermissionsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
        {            
        }
        #endregion

        #region Methods
        public async Task PermissionsDeleteRecursive(int? permissionID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(PermissionsDeleteRecursive)}({nameof(permissionID)} = {permissionID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsDeleteRecursive),
                            sqlParameters:
                            [
                                permissionID.ToSqlParameter(nameof(permissionID),SqlDbType.Int)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();                        
                    }
                }
            );
        }

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions databaseAction, int? permissionID = null, int? permissionParentID = null, string permissionCaption = null, string permissionCaptionEng = null, string permissionPagePath = null, string permissionCodeName = null, string permissionCode = null, int? permissionSortIndex = null, bool? permissionIsMenuItem = null, string permissionMenuIcon = null, string permissionMenuTitle = null, string permissionMenuTitleEng = null)
        {
            permissionID = await TryToReturnAsyncTask(
                logString: $"{nameof(PermissionsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(permissionID)} = {permissionID}, {nameof(permissionParentID)} = {permissionParentID}, {nameof(permissionCaption)} = {permissionCaption}, {nameof(permissionCaptionEng)} = {permissionCaptionEng}, {nameof(permissionPagePath)} = {permissionPagePath}, {nameof(permissionCodeName)} = {permissionCodeName}, {nameof(permissionCode)} = {permissionCode}, {nameof(permissionSortIndex)} = {permissionSortIndex}, {nameof(permissionIsMenuItem)} = {permissionIsMenuItem}, {nameof(permissionMenuIcon)} = {permissionMenuIcon}, {nameof(permissionMenuTitle)} = {permissionMenuTitle}, {nameof(permissionMenuTitleEng)} = {permissionMenuTitleEng})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
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

                        await sqb.ExecuteStoredProcedure();
                        permissionID = sqb.GetNextOutputParameterValue<int?>();
                        return permissionID;                        
                    }
                }
            );
            return permissionID;
        }

        public async Task<List<PermissionDTO>> PermissionsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PermissionsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PermissionDTO>();
                        resultQueryable = resultQueryable.OrderBy(P => P.PermissionSortIndex);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<PermissionsListByRoleIDDTO>> PermissionsListByRoleID(int? roleID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(PermissionsListByRoleID)}({nameof(roleID)} = {roleID}", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(PermissionsListByRoleID),
                            sqlParameters:
                            [
                                roleID.ToSqlParameter(nameof(roleID), SqlDbType.Int)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PermissionsListByRoleIDDTO>();
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }        
}