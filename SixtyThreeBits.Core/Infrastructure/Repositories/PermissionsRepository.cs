using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PermissionsRepository : RepositoryBase
    {
        #region Contructors
        public PermissionsRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
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

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions databaseAction, int? permissionID, PermissionIudDTO permission)
        {
            var permissionJson = permission.ToJson();

            permissionID = await TryToReturnAsyncTask(
                logString: $"{nameof(PermissionsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(permissionID)} = {permissionID}, {nameof(permission)} = {permissionJson})", 
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
                                permissionJson.ToSqlParameter(nameof(permissionJson),SqlDbType.NVarChar)                                
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

        public async Task<List<PermissionsListDTO>> PermissionsList()
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

                        var resultQueryable = sqb.ExecuteTableValuedFunction<PermissionsListDTO>();
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