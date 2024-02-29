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
                cfg.CreateMap<DbContextQueries.DictionariesListEntity, DictionariesDTO>();
                cfg.CreateMap<DbContextQueries.DictionariesListByLevelAndCodeAndIsVisibleEntity, DictionariesDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods        
        public async Task DictionariesDeleteRecursive(int? dictionaryID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(DictionariesDeleteRecursive)}({nameof(dictionaryID)} = {dictionaryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.DictionariesDeleteRecursive(dictionaryID);                    
                    }
                }
            );
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions databaseAction, int? dictionaryID = null, string dictionaryCaption = null, string dictionaryCaptionEng = null, int? dictionaryParentID = null, string dictionaryStringCode = null, int? dictionaryIntCode = null, decimal? dictionaryDecimalValue = null, int? dictionaryCode = null, bool? dictionaryIsDefault = null, bool? dictionaryIsVisible = null, int? dictionarySortIndex = null)
        {
            dictionaryID = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(dictionaryID)} = {dictionaryID}, {nameof(dictionaryCaption)} = {dictionaryCaption}, {nameof(dictionaryCaptionEng)} = {dictionaryCaptionEng}, {nameof(dictionaryParentID)} = {dictionaryParentID}, {nameof(dictionaryStringCode)} = {dictionaryStringCode}, {nameof(dictionaryIntCode)} = {dictionaryIntCode}, {nameof(dictionaryDecimalValue)} = {dictionaryDecimalValue}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsDefault)} = {dictionaryIsDefault}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible}, {nameof(dictionarySortIndex)} = {dictionarySortIndex})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        dictionaryID = await db.DictionariesIUD(databaseAction, dictionaryID, dictionaryCaption, dictionaryCaptionEng, dictionaryParentID, dictionaryStringCode, dictionaryIntCode, dictionaryDecimalValue, dictionaryCode, dictionaryIsDefault, dictionaryIsVisible, dictionarySortIndex);
                        return dictionaryID;
                    }
                }
            );
            return dictionaryID;
        }

        public async Task<List<DictionariesDTO>> DictionariesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.DictionariesList()
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption)
                            .ToListAsync()
                        ).Select(item => _mapper.Map<DictionariesDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<DictionariesDTO>> DictionariesListByLevelAndCodeAndIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesListByLevelAndCodeAndIsVisible)}({nameof(dictionaryLevel)} = {dictionaryLevel}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible})",
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel, dictionaryCode, dictionaryIsVisible)
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption)
                            .ToListAsync()
                        ).Select(item => _mapper.Map<DictionariesDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> DictionariesListAsKeyValueSelectedTuple(int? dictionaryCode, int? selectedValue, bool isDictionaryIntCodeAsKey = false)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesListAsKeyValueSelectedTuple)}({nameof(dictionaryCode)} = {dictionaryCode}, {nameof(isDictionaryIntCodeAsKey)} = {isDictionaryIntCodeAsKey})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var dictionaries = (await DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode, dictionaryIsVisible: null));
                        var result = dictionaries?.Select(item => new KeyValueSelectedTuple<int?, string>
                        {
                            Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                            Value = item.DictionaryCaption,
                            IsSelected = isDictionaryIntCodeAsKey ? (item.DictionaryIntCode == selectedValue) : (item.DictionaryID == selectedValue)
                        }).ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<KeyValueTuple<int?, string>>> DictionariesListAsKeyValueTuple(int? dictionaryCode, bool dictionaryCodeAsKey = false)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesListAsKeyValueTuple)}({nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryCodeAsKey)} = {dictionaryCodeAsKey})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var dictionaries = await DictionariesListByLevelAndCodeAndIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode, dictionaryIsVisible: null);
                        var result = dictionaries?.Select(item => new KeyValueTuple<int?, string>
                        {
                            Key = dictionaryCodeAsKey ? item.DictionaryCode : item.DictionaryID,
                            Value = item.DictionaryCaption
                        }).ToList();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}