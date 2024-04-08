using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class DictionariesRepository : RepositoryBase
    {
        #region Contructors
        public DictionariesRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {            
        }
        #endregion

        #region Methods        
        public async Task DictionariesDeleteRecursive(int? dictionaryID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(DictionariesDeleteRecursive)}({nameof(dictionaryID)} = {dictionaryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesDeleteRecursive),
                            sqlParameters:
                            [
                                dictionaryID.ToSqlParameter(nameof(dictionaryID),SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
        }

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions databaseAction, int? dictionaryID, DictionarieIudDTO dictionary)
        {
            var dictionaryJson = dictionary.ToJson();

            dictionaryID = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(dictionaryID)} = {dictionaryID}, {nameof(dictionary)} = {dictionaryJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                dictionaryID.ToSqlOutputParameter(nameof(dictionaryID),SqlDbType.Int),
                                dictionaryJson.ToSqlParameter(nameof(dictionaryJson),SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        dictionaryID = sqb.GetNextOutputParameterValue<int?>();
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<DictionariesDTO>();
                        resultQueryable = resultQueryable
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption).OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<DictionariesDTO>> DictionariesListByLevelCodeIsVisible(int? dictionaryLevel, int? dictionaryCode, bool? dictionaryIsVisible = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesListByLevelCodeIsVisible)}({nameof(dictionaryLevel)} = {dictionaryLevel}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(DictionariesListByLevelCodeIsVisible),
                            sqlParameters:
                            [
                                dictionaryLevel.ToSqlParameter(nameof(dictionaryLevel), SqlDbType.Int),
                                dictionaryCode.ToSqlParameter(nameof(dictionaryCode), SqlDbType.Int),
                                dictionaryIsVisible.ToSqlParameter(nameof(dictionaryIsVisible), SqlDbType.Bit)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<DictionariesDTO>();
                        resultQueryable = resultQueryable
                            .OrderByDescending(item => item.DictionaryIsDefault)
                            .ThenBy(item => item.DictionarySortIndex)
                            .ThenBy(item => item.DictionaryCaption);
                        var result = await resultQueryable.ToListAsync();                        

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<KeyValueTuple<int?, string>>> DictionariesListAsKeyValueTuple(int? dictionaryCode, bool isDictionaryIntCodeAsKey = false)
        {
            var result = (await DictionariesListByLevelCodeIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode))
                ?.Select(item => new KeyValueTuple<int?, string>
                {
                    Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                    Value = item.DictionaryCaption
                }).ToList();
            return result;
        }

        public async Task<List<KeyValueSelectedTuple<int?, string>>> DictionariesListAsKeyValueSelectedTuple(int? dictionaryCode, int? selectedValue, bool isDictionaryIntCodeAsKey = false)
        {
            var result = (await DictionariesListByLevelCodeIsVisible(dictionaryLevel: 1, dictionaryCode: dictionaryCode))
                ?.Select(item => new KeyValueSelectedTuple<int?, string>
                {
                    Key = isDictionaryIntCodeAsKey ? item.DictionaryIntCode : item.DictionaryID,
                    Value = item.DictionaryCaption,
                    IsSelected = isDictionaryIntCodeAsKey ? (item.DictionaryIntCode == selectedValue) : (item.DictionaryID == selectedValue)
                }).ToList();            
            return result;
        }        
        #endregion
    }
}