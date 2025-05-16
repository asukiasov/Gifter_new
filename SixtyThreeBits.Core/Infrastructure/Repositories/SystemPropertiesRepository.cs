using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Libraries.Extensions;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class SystemPropertiesRepository : RepositoryBase
    {
        #region Contructors
        public SystemPropertiesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
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
