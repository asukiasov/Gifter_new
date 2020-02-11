using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.DB.Tables;
using SixtyThreeBits.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class DictionariesDataAccess : DataAccessBase
    {
        #region Contructors
        public DictionariesDataAccess(DBCoreDataContext db) : base(db) { }
        #endregion

        #region Methods
        public async Task DeleteRecursive(int? DictionaryID)
        {
            await TryExecuteAsyncTask($"{nameof(DeleteRecursive)}({nameof(DictionaryID)} = {DictionaryID})", async () =>
            {
                await db.DictionariesDeleteRecursive(DictionaryID);
            });
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions DatabaseAction, int? DictionaryID = null, int? DictionaryParentID = null, string DictionaryCaption = null, string DictionaryCaptionEng = null, string DictionaryCaptionRus = null, string DictionaryStringCode = null, int? DictionaryIntCode = null, decimal? DictionaryDecimalValue = null, int? DictionaryCode = null, bool? DictionaryIsDefault = null, bool? DictionaryIsVisible = null, int? DictionarySortIndex = null)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(DictionaryID)} = {DictionaryID}, {nameof(DictionaryParentID)} = {DictionaryParentID}, {nameof(DictionaryCaption)} = {DictionaryCaption}, {nameof(DictionaryCaptionEng)} = {DictionaryCaptionEng}, {nameof(DictionaryCaptionRus)} = {DictionaryCaptionRus}, {nameof(DictionaryStringCode)} = {DictionaryStringCode}, {nameof(DictionaryIntCode)} = {DictionaryIntCode}, {nameof(DictionaryDecimalValue)} = {DictionaryDecimalValue}, {nameof(DictionaryCode)} = {DictionaryCode}, {nameof(DictionaryIsDefault)} = {DictionaryIsDefault}, {nameof(DictionaryIsVisible)} = {DictionaryIsVisible}, {nameof(DictionarySortIndex)} = {DictionarySortIndex})", async () =>
            {
                if (DatabaseAction == Enums.DatabaseActions.CREATE)
                {
                    var DBItem = new Dictionaries();
                    DBItem.DictionaryParentID = DictionaryParentID == Constants.NullValueFor.Int ? null : DictionaryParentID;
                    DBItem.DictionaryCaption = DictionaryCaption;
                    DBItem.DictionaryCaptionEng = DictionaryCaptionEng;
                    DBItem.DictionaryCaptionRus = DictionaryCaptionRus;
                    DBItem.DictionaryStringCode = DictionaryStringCode;
                    DBItem.DictionaryIntCode = DictionaryIntCode;
                    DBItem.DictionaryDecimalValue = DictionaryDecimalValue;
                    DBItem.DictionaryCode = DictionaryCode;
                    DBItem.DictionaryIsDefault = DictionaryIsDefault ?? false;
                    DBItem.DictionaryIsVisible = DictionaryIsVisible ?? false;
                    DBItem.DictionarySortIndex = DictionarySortIndex;
                    await db.Dictionaries.AddAsync(DBItem);
                    await db.SaveChangesAsync();

                    DictionaryID = DBItem.DictionaryID;
                }
                else if (DatabaseAction == Enums.DatabaseActions.UPDATE)
                {
                    var DBItem = await db.Dictionaries.FirstOrDefaultAsync(Item => Item.DictionaryID == DictionaryID);
                    if (DBItem != null)
                    {
                        DBItem.DictionaryParentID = DictionaryParentID == Constants.NullValueFor.Int ? null : DictionaryParentID ?? DBItem.DictionaryParentID;
                        DBItem.DictionaryCaption = DictionaryCaption == Constants.NullValueFor.String ? null : DictionaryCaption ?? DBItem.DictionaryCaption;
                        DBItem.DictionaryCaptionEng = DictionaryCaptionEng == Constants.NullValueFor.String ? null : DictionaryCaptionEng ?? DBItem.DictionaryCaptionEng;
                        DBItem.DictionaryCaptionRus = DictionaryCaptionRus == Constants.NullValueFor.String ? null : DictionaryCaptionRus ?? DBItem.DictionaryCaptionRus;
                        DBItem.DictionaryStringCode = DictionaryStringCode == Constants.NullValueFor.String ? null : DictionaryStringCode ?? DBItem.DictionaryStringCode;
                        DBItem.DictionaryIntCode = DictionaryIntCode == Constants.NullValueFor.Int ? null : DictionaryIntCode ?? DBItem.DictionaryIntCode;
                        DBItem.DictionaryDecimalValue = DictionaryDecimalValue == Constants.NullValueFor.Int ? null : DictionaryDecimalValue ?? DBItem.DictionaryDecimalValue;
                        DBItem.DictionaryCode = DictionaryCode == Constants.NullValueFor.Int ? null : DictionaryCode ?? DBItem.DictionaryCode;
                        DBItem.DictionaryIsDefault = DictionaryIsDefault ?? DBItem.DictionaryIsDefault;
                        DBItem.DictionaryIsVisible = DictionaryIsVisible ?? DBItem.DictionaryIsVisible;
                        DBItem.DictionarySortIndex = DictionarySortIndex == Constants.NullValueFor.Int ? null : DictionarySortIndex ?? DBItem.DictionarySortIndex;
                        db.Dictionaries.Update(DBItem);
                        await db.SaveChangesAsync();
                    }

                }

                return DictionaryID;
            });            
        }

        public async Task<List<Dictionaries>> ListDictionaries()
        {
            return await TryToReturnAsyncTask($"{nameof(ListDictionaries)}()", async () =>
            {
                return await db.Dictionaries.OrderBy(Item => Item.DictionarySortIndex).ThenBy(Item => Item.DictionaryCaption).ToListAsync();
            });
        }
        #endregion
    }
}
