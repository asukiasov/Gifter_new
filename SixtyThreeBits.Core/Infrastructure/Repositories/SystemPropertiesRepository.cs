using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Libraries.Extensions;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class SystemPropertiesRepository : RepositoryBase
    {
        #region Contructors
        public SystemPropertiesRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {
        }
        #endregion

        #region Methods
        public async Task<SystemPropertiesDTO> SystemPropertiesGet()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(SystemPropertiesGet)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(SystemPropertiesGet)
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<SystemPropertiesDTO>();

                        return result;
                    }
                }
            );
            return result ?? new SystemPropertiesDTO();
        }

        public async Task SystemPropertiesUpdate(SystemPropertiesIudDTO systemProperties)
        {
            var systemPropertiesJson = systemProperties.ToJson();

            await TryExecuteAsyncTask(
                logString: $"{nameof(SystemPropertiesUpdate)}({nameof(systemProperties)} = {systemPropertiesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(SystemPropertiesUpdate),
                            sqlParameters:
                            [
                                systemPropertiesJson.ToSqlParameter(nameof(systemPropertiesJson),SqlDbType.NVarChar)
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
