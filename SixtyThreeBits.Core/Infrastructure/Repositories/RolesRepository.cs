using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class RolesRepository : RepositoryBase
    {
        #region Contructors
        public RolesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger) 
        {            
        }
        #endregion

        #region Methods
        public async Task<int?> RolesIUD(Enums.DatabaseActions databaseAction, int? roleID, RoleIudDTO role)
        {
            var roleJson = role.ToJson();

            roleID = await TryToReturnAsyncTask(
                logString: $"{nameof(RolesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(roleID)} = {roleID}, {nameof(role)} = {roleJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                roleID.ToSqlParameterOutput(SqlDbType.Int),
                                roleJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                         );

                        await sqb.ExecuteStoredProcedure();
                        roleID = sqb.GetNextOutputParameterValue<int?>();
                        return roleID;
                    }
                }
            );
            return roleID;
        }

        public async Task<List<RoleDTO>> RolesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(RolesList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<RoleDTO>();
                        resultQueryable = resultQueryable.OrderBy(item => item.RoleCode);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<KeyValueTuple<int?,string>>> RolesListAsKeyValueTuple(bool IsRoleCodeAsKey = false)
        {
            var result = (await RolesList())
                ?.Select(item => new KeyValueTuple<int?, string>
                {
                    Key = IsRoleCodeAsKey ? item.RoleCode : item.RoleID,
                    Value = item.RoleName
                }).ToList();            
            return result;
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> RolesListAsKeyValueSelectedTuple(int? SelectedValue, bool IsRoleCodeAsKey = false)
        {
            var result = (await RolesList())
                ?.Select(item => new KeyValueSelectedTuple<int?, string>
                {
                    Key = IsRoleCodeAsKey ? item.RoleCode : item.RoleID,
                    Value = item.RoleName,
                    IsSelected = (IsRoleCodeAsKey ? (item.RoleCode == SelectedValue) : (item.RoleID == SelectedValue))
                }).ToList();
            return result;                      
        }

        public async Task RolesPermissionsUpdate(int? roleID, List<int?> permissionIDs)
        {
            var permissionIDsJson = permissionIDs.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(RolesPermissionsUpdate)}({nameof(roleID)} = {roleID}, {nameof(permissionIDs)} = {permissionIDsJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(RolesPermissionsUpdate),
                            sqlParameters:
                            [
                                roleID.ToSqlParameter(SqlDbType.Int),
                                permissionIDsJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();                        
                    }
                }
            );
        }
        #endregion
    }    
}