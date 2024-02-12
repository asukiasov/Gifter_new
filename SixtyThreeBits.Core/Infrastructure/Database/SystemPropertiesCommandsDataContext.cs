using SixtyThreeBits.Core.Infrastructure.Database.Core;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task SystemPropertiesUpdate(string systemPropertiesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(SystemPropertiesUpdate),
                itemType: null,
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