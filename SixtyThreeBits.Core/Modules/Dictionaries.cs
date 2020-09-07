using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB.Tables;
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

        public async Task<List<Dictionaries>> ListDictionaries()
        {
            return await TryToReturnAsyncTask($"{nameof(ListDictionaries)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.Dictionaries.OrderBy(Item => Item.DictionarySortIndex).ThenBy(Item => Item.DictionaryCaption).ToListAsync();
                }
            });
        }

        public async Task<List<SimpleKeyValue<int?, string>>> ListDictionariesAsSimpleKeyValue(int? DictionaryCode, int? SelectedValue = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ListDictionariesAsSimpleKeyValue)}({nameof(DictionaryCode)} = {DictionaryCode})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.Dictionaries.Where(Item => Item.DictionaryLevel == 1 && Item.DictionaryCode == DictionaryCode).OrderBy(Item => Item.DictionarySortIndex).ThenBy(Item => Item.DictionaryCaption).ToListAsync();
                    return Result.Select(Item => new SimpleKeyValue<int?, string>
                    {
                        Key = Item.DictionaryID,
                        Value = Item.DictionaryCaption,
                        IsSelected = Item.DictionaryID == SelectedValue
                    }).ToList();
                }
            });
        }
        #endregion
    }
}
