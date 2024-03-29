using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class DictionariesRepository : RepositoryBase
    {
        #region Contructors
        public DictionariesRepository(DbContextFactory connectionFactory) : base(connectionFactory)
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

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions databaseAction, int? dictionaryID = null, string dictionaryCaption = null, string dictionaryCaptionEng = null, int? dictionaryParentID = null, string dictionaryStringCode = null, int? dictionaryIntCode = null, decimal? dictionaryDecimalValue = null, int? dictionaryCode = null, bool? dictionaryIsDefault = null, bool? dictionaryIsVisible = null, int? dictionarySortIndex = null)
        {
            dictionaryID = await TryToReturnAsyncTask(
                logString: $"{nameof(DictionariesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(dictionaryID)} = {dictionaryID}, {nameof(dictionaryCaption)} = {dictionaryCaption}, {nameof(dictionaryCaptionEng)} = {dictionaryCaptionEng}, {nameof(dictionaryParentID)} = {dictionaryParentID}, {nameof(dictionaryStringCode)} = {dictionaryStringCode}, {nameof(dictionaryIntCode)} = {dictionaryIntCode}, {nameof(dictionaryDecimalValue)} = {dictionaryDecimalValue}, {nameof(dictionaryCode)} = {dictionaryCode}, {nameof(dictionaryIsDefault)} = {dictionaryIsDefault}, {nameof(dictionaryIsVisible)} = {dictionaryIsVisible}, {nameof(dictionarySortIndex)} = {dictionarySortIndex})", 
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