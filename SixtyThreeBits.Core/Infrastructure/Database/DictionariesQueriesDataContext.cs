using System;
using System.Data;
using System.Linq;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region DictionariesList
        public record DictionariesListEntity
        (
            int? DictionaryID,
            string DictionaryCaption,
            string DictionaryCaptionEng,
            int? DictionaryParentID,
            int? DictionaryLevel,
            string DictionaryStringCode,
            int? DictionaryIntCode,
            decimal? DictionaryDecimalValue,
            int? DictionaryCode,
            bool DictionaryIsDefault,
            bool DictionaryIsVisible,
            int? DictionarySortIndex,
            DateTime? DictionaryDateCreated
        );
        public IQueryable<DictionariesListEntity> DictionariesList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(DictionariesList),
                itemType: typeof(DictionariesListEntity)
            );
            var result = sqb.ExecuteQuery<DictionariesListEntity>();
            return result;
        }
        #endregion

        #region DictionariesListByLevelAndCodeAndIsVisible
        public record DictionariesListByLevelAndCodeAndIsVisibleEntity
        (
            int? DictionaryID,
            string DictionaryCaption,
            string DictionaryCaptionEng,
            int? DictionaryParentID,
            int? DictionaryLevel,
            string DictionaryStringCode,
            int? DictionaryIntCode,
            decimal? DictionaryDecimalValue,
            int? DictionaryCode,
            bool DictionaryIsDefault,
            bool DictionaryIsVisible,
            int? DictionarySortIndex,
            DateTime? DictionaryDateCreated
        );
        public IQueryable<DictionariesListByLevelAndCodeAndIsVisibleEntity> DictionariesListByLevelAndCodeAndIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(DictionariesList),
                itemType: typeof(DictionariesListByLevelAndCodeAndIsVisibleEntity),
                sqlParameters:
                [
                    dictionaryLevel.ToSqlParameter(nameof(dictionaryLevel), SqlDbType.Int),
                    dictionaryCode.ToSqlParameter(nameof(dictionaryCode), SqlDbType.Int),
                    dictionaryIsVisible.ToSqlParameter(nameof(dictionaryIsVisible), SqlDbType.Bit)
                ]
            );
            var result = sqb.ExecuteQuery<DictionariesListByLevelAndCodeAndIsVisibleEntity>();
            return result;
        }
        #endregion
    }
}