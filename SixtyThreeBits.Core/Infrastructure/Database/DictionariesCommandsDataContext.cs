using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        public async Task DictionariesDeleteRecursive(int? dictionaryID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(DictionariesDeleteRecursive),
                itemType: null,
                sqlParameters:
                [
                    dictionaryID.ToSqlParameter(nameof(dictionaryID),SqlDbType.Int)
                ]
           );

            await sqb.ExecuteCommand();
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions databaseAction, int? dictionaryID, string dictionaryCaption, string dictionaryCaptionEng, int? dictionaryParentID, string dictionaryStringCode, int? dictionaryIntCode, decimal? dictionaryDecimalValue, int? dictionaryCode, bool? dictionaryIsDefault, bool? dictionaryIsVisible, int? dictionarySortIndex)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(DictionariesIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    dictionaryID.ToSqlParameter(nameof(dictionaryID),SqlDbType.Int, true),
                    dictionaryCaption.ToSqlParameter(nameof(dictionaryCaption),SqlDbType.NVarChar),
                    dictionaryCaptionEng.ToSqlParameter(nameof(dictionaryCaptionEng),SqlDbType.NVarChar),
                    dictionaryParentID.ToSqlParameter(nameof(dictionaryParentID),SqlDbType.Int),
                    dictionaryStringCode.ToSqlParameter(nameof(dictionaryStringCode),SqlDbType.NVarChar),
                    dictionaryIntCode.ToSqlParameter(nameof(dictionaryIntCode),SqlDbType.Int),
                    dictionaryDecimalValue.ToSqlParameter(nameof(dictionaryDecimalValue),SqlDbType.Money),
                    dictionaryCode.ToSqlParameter(nameof(dictionaryCode),SqlDbType.Int),
                    dictionaryIsDefault.ToSqlParameter(nameof(dictionaryIsDefault),SqlDbType.Bit),
                    dictionaryIsVisible.ToSqlParameter(nameof(dictionaryIsVisible),SqlDbType.Bit),
                    dictionarySortIndex.ToSqlParameter(nameof(dictionarySortIndex),SqlDbType.Int),
                ]
            );

            await sqb.ExecuteCommand();
            dictionaryID = sqb.GetNextOutputParameterValue<int?>();
            return dictionaryID;
        }
    }
}