using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class DictionariesRepository : RepositoryBase
    {
        #region Contructors
        public DictionariesRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.DictionariesListEntity, DictionariesDTO>();
                cfg.CreateMap<DBQueriesDataContext.DictionariesListByLevelAndCodeAndIsVisibleEntity, DictionariesDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods        
        public async Task DictionariesDeleteRecursive(int? dictionaryID)
        {
            await TryExecuteAsyncTask($"{nameof(DictionariesDeleteRecursive)}({nameof(dictionaryID)} = {dictionaryID})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.DictionariesDeleteRecursive(dictionaryID);
                }
            });
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions databaseAction, int? dictionaryID = null, string dictionaryCaption = null, string dictionaryCaptionEng = null, int? dictionaryParentID = null, string dictionaryStringCode = null, int? dictionaryIntCode = null, decimal? dictionaryDecimalValue = null, int? dictionaryCode = null, bool? dictionaryIsDefault = null, bool? dictionaryIsVisible = null, int? dictionarySortIndex = null)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(dictionaryID)} = {dictionaryID}, {nameof(dictionaryCaption)} = {dictionaryCaption}, {nameof(dictionaryCaptionEng)} = {dictionaryCaptionEng}, {nameof(dictionaryParentID)} = {dictionaryParentID}, {nameof(dictionaryStringCode)} = {dictionaryStringCode}, {nameof(dictionaryIntCode)} = {dictionaryIntCode}, {nameof(dictionaryDecimalValue)} = {dictionaryDecimalValue}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsDefault)} = {dictionaryIsDefault}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible}, {nameof(dictionarySortIndex)} = {dictionarySortIndex})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    dictionaryID = await db.DictionariesIUD(databaseAction, dictionaryID, dictionaryCaption, dictionaryCaptionEng, dictionaryParentID, dictionaryStringCode, dictionaryIntCode, dictionaryDecimalValue, dictionaryCode, dictionaryIsDefault, dictionaryIsVisible, dictionarySortIndex);
                    return dictionaryID;
                }
            });
        }

        public async Task<List<DictionariesDTO>> DictionariesList()
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (
                    await db.DictionariesList()
                    .OrderByDescending(item => item.DictionaryIsDefault)
                    .ThenBy(item => item.DictionarySortIndex)
                    .ThenBy(item => item.DictionaryCaption)
                    .ToListAsync()
                    )
                    .Select(item=>_mapper.Map<DictionariesDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<DictionariesDTO>> DictionariesListByLevelAndCodeAndIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible = null)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (
                    await db.DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel, dictionaryCode, dictionaryIsVisible)
                    .OrderByDescending(item => item.DictionaryIsDefault)
                    .ThenBy(item => item.DictionarySortIndex)
                    .ThenBy(item => item.DictionaryCaption)
                    .ToListAsync()
                    )
                    .Select(item => _mapper.Map<DictionariesDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> DictionariesListAsKeyValueSelectedTuple(int? dictionaryCode, int? selectedValue, bool isDictionaryIntCodeAsKey = false)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesListAsKeyValueSelectedTuple)}({nameof(dictionaryCode)} = {dictionaryCode}, {nameof(isDictionaryIntCodeAsKey)} = {isDictionaryIntCodeAsKey})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode, dictionaryIsVisible: null);
                    return Result?.Select(item => new KeyValueSelectedTuple<int?, string>
                    {
                        Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                        Value = item.DictionaryCaption,
                        IsSelected = isDictionaryIntCodeAsKey ? (item.DictionaryIntCode == selectedValue) :(item.DictionaryID == selectedValue)
                    }).ToList();
                }
            });
        }

        public async Task<List<KeyValueTuple<int?, string>>> DictionariesListAsKeyValueTuple(int? dictionaryCode, bool dictionaryCodeAsKey = false)
        {
            return await TryToReturnAsyncTask($"{nameof(DictionariesListAsKeyValueTuple)}({nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryCodeAsKey)} = {dictionaryCodeAsKey})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode, dictionaryIsVisible: null);
                    return Result?.Select(item => new KeyValueTuple<int?, string>
                    {
                        Key = dictionaryCodeAsKey ? item.DictionaryCode : item.DictionaryID,
                        Value = item.DictionaryCaption
                    }).ToList();
                }
            });
        }
        #endregion
    }    
}
