using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class DictionariesDataAccess : DataAccessBase
    {
        #region Contructors
        public DictionariesDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? DictionaryID)
        {
            await TryExecuteAsyncTask($"{nameof(DeleteRecursive)}({nameof(DictionaryID)} = {DictionaryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.DictionariesDeleteRecursive(DictionaryID);
                }
            });
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions DatabaseAction, int? DictionaryID = null, string DictionaryCaption = null, string DictionaryCaptionEng = null, string DictionaryCaptionRus = null, int? DictionaryParentID = null, string DictionaryStringCode = null, int? DictionaryIntCode = null, decimal? DictionaryDecimalValue = null, int? DictionaryCode = null, bool? DictionaryIsDefault = null, bool? DictionaryIsVisible = null, int? DictionarySortIndex = null)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(DictionaryID)} = {DictionaryID}, {nameof(DictionaryCaption)} = {DictionaryCaption}, {nameof(DictionaryCaptionEng)} = {DictionaryCaptionEng}, {nameof(DictionaryCaptionRus)} = {DictionaryCaptionRus}, {nameof(DictionaryParentID)} = {DictionaryParentID}, {nameof(DictionaryStringCode)} = {DictionaryStringCode}, {nameof(DictionaryIntCode)} = {DictionaryIntCode}, {nameof(DictionaryDecimalValue)} = {DictionaryDecimalValue}, {nameof(DictionaryCode)} = {DictionaryCode}, {nameof(DictionaryIsDefault)} = {DictionaryIsDefault}, {nameof(DictionaryIsVisible)} = {DictionaryIsVisible}, {nameof(DictionarySortIndex)} = {DictionarySortIndex})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {

                    DictionaryID = await db.DictionariesIUD(DatabaseAction, DictionaryID, DictionaryCaption, DictionaryCaptionEng, DictionaryCaptionRus, DictionaryParentID, DictionaryStringCode, DictionaryIntCode, DictionaryDecimalValue, DictionaryCode, DictionaryIsDefault, DictionaryIsVisible, DictionarySortIndex);

                    return DictionaryID;
                }
            });
        }

        public async Task<List<SimpleKeyValue<int?, string>>> CountriesListAsSimpleKeyValue(int? SelectedCountryID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(CountriesListAsSimpleKeyValue)}({nameof(SelectedCountryID)} = {SelectedCountryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.CountriesList().OrderBy(Item => Item.CountryName).ToListAsync()).Select(Item => new SimpleKeyValue<int?, string>
                    {
                        Key = Item.CountryID,
                        Value = Item.CountryName,
                        IsSelected = Item.CountryID == SelectedCountryID
                    }).ToList();
                }
            });
        }

        public async Task<List<DBCoreDataContext.DictionariesListResultItem>> ListDictionaries(int? DictionaryLevel = null, int? DictionaryCode = null, bool? DictionaryIsVisible = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ListDictionaries)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.DictionariesList(DictionaryLevel, DictionaryCode, DictionaryIsVisible)
                    .OrderByDescending(Item => Item.DictionaryIsDefault)
                    .ThenBy(Item => Item.DictionarySortIndex)
                    .ThenBy(Item => Item.DictionaryCaption)
                    .ToListAsync();
                }
            });
        }

        public async Task<List<SimpleKeyValue<int?, string>>> ListDictionariesAsSimpleKeyValue(int? DictionaryCode, int? SelectedValue = null, bool DictionaryCodeAsKey = false)
        {
            return await TryToReturnAsyncTask($"{nameof(ListDictionariesAsSimpleKeyValue)}({nameof(DictionaryCode)} = {DictionaryCode}, {nameof(DictionaryCodeAsKey)} = {DictionaryCodeAsKey})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await ListDictionaries(DictionaryLevel: 1, DictionaryCode: DictionaryCode, DictionaryIsVisible: null);
                    return Result?.Select(Item => new SimpleKeyValue<int?, string>
                    {
                        Key = DictionaryCodeAsKey ? Item.DictionaryCode : Item.DictionaryID,
                        Value = Item.DictionaryCaption,
                        IsSelected = Item.DictionaryID == SelectedValue
                    }).ToList();
                }
            });
        }
        #endregion
    }
}
