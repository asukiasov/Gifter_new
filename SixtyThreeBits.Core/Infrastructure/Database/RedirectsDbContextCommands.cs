using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task<int?> RedirectsIUD(Enums.DatabaseActions databaseAction, int? redirectID, string redirectFrom, string redirectTo)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(RedirectsIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    redirectID.ToSqlParameter(nameof(redirectID),SqlDbType.Int,true),
                    redirectFrom.ToSqlParameter(nameof(redirectFrom),SqlDbType.NVarChar),
                    redirectTo.ToSqlParameter(nameof(redirectTo),SqlDbType.NVarChar),
                ]
             );

            await sqb.ExecuteCommand();
            redirectID = sqb.GetNextOutputParameterValue<int?>();
            return redirectID;
        }
        #endregion
    }
}