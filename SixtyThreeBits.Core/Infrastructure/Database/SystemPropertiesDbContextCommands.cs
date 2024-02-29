using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task SystemPropertiesUpdate(string systemPropertiesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(SystemPropertiesUpdate),
                sqlParameters:
                [
                    systemPropertiesJson.ToSqlParameter(nameof(systemPropertiesJson),SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
        }
        #endregion
    }
}