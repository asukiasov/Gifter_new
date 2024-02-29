using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
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
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(SystemPropertiesGet)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.SystemPropertiesGet();
                        var result = resultJson?.DeserializeJsonTo<SystemPropertiesDTO>();
                        return result;
                    }
                }
            );
            return result ?? new SystemPropertiesDTO();
        }

        public async Task SystemPropertiesUpdate(SystemPropertiesDTO systemProperties)
        {
            var systemPropertiesJson = systemProperties.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(SystemPropertiesUpdate)}({nameof(systemProperties)} = {systemPropertiesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.SystemPropertiesUpdate(systemPropertiesJson);
                    }
                }
            );
        }
        #endregion
    }        
}
