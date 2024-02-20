using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task<int?> PartnersIUD(Enums.DatabaseActions databaseAction, int? partnerID, string partnerName, string partnerNameEng, string partnerShortDescription, string partnerShortDescriptionEng, string partnerFullDescription, string partnerFullDescriptionEng, string partnerWebSite, string partnerImageFilename, bool? partnerIsPublished)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(PartnersIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    partnerID.ToSqlParameter(nameof(partnerID),SqlDbType.Int,true),
                    partnerName.ToSqlParameter(nameof(partnerName),SqlDbType.NVarChar),
                    partnerNameEng.ToSqlParameter(nameof(partnerNameEng),SqlDbType.NVarChar),
                    partnerShortDescription.ToSqlParameter(nameof(partnerShortDescription),SqlDbType.NVarChar),
                    partnerShortDescriptionEng.ToSqlParameter(nameof(partnerShortDescriptionEng),SqlDbType.NVarChar),
                    partnerFullDescription.ToSqlParameter(nameof(partnerFullDescription),SqlDbType.NVarChar),
                    partnerFullDescriptionEng.ToSqlParameter(nameof(partnerFullDescriptionEng),SqlDbType.NVarChar),
                    partnerWebSite.ToSqlParameter(nameof(partnerWebSite),SqlDbType.NVarChar),
                    partnerImageFilename.ToSqlParameter(nameof(partnerImageFilename),SqlDbType.NVarChar),
                    partnerIsPublished.ToSqlParameter(nameof(partnerIsPublished),SqlDbType.Bit),
                ]
            );

            await sqb.ExecuteCommand();
            partnerID = sqb.GetNextOutputParameterValue<int?>();
            return partnerID;
        }
        #endregion
    }
}