using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task<int?> BrandsIUD(Enums.DatabaseActions databaseAction, int? brandID, string brandName, string brandNameEng, string brandImageFilename)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(BrandsIUD),
                itemType: null,
                sqlParameters:
                [
                     databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                     brandID.ToSqlParameter(nameof(brandID),SqlDbType.Int,true),
                     brandName.ToSqlParameter(nameof(brandName),SqlDbType.NVarChar),
                     brandNameEng.ToSqlParameter(nameof(brandNameEng),SqlDbType.NVarChar),
                     brandImageFilename.ToSqlParameter(nameof(brandImageFilename),SqlDbType.NVarChar)
                ]
            );

            var DBResult = await sqb.ExecuteCommand();
            brandID = sqb.GetNextOutputParameterValue<int?>();
            return brandID;
        }
        #endregion
    }
}