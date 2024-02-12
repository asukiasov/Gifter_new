using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Libraries.Extensions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class SystemPropertiesRepository : RepositoryBase
    {
        #region Contructors
        public SystemPropertiesRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {

        }
        #endregion

        #region Methods
        public async Task<SystemPropertiesDTO> SystemPropertiesGet()
        {
            var Result = await TryToReturnAsyncTask($"{nameof(SystemPropertiesGet)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var DBResult = await db.SystemPropertiesGet();
                    return DBResult?.DeserializeJsonTo<SystemPropertiesDTO>();
                }
            });
            return Result ?? new SystemPropertiesDTO();
        }

        public async Task SystemPropertiesUpdate(SystemPropertiesDTO systemProperties)
        {
            var SystemPropertiesJson = systemProperties.ToJson();
            await TryExecuteAsyncTask($"{nameof(SystemPropertiesUpdate)}({nameof(systemProperties)} = {SystemPropertiesJson})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.SystemPropertiesUpdate(SystemPropertiesJson);
                }
            });
        }
        #endregion
    }        
}
