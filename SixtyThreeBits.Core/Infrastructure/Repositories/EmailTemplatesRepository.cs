using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class EmailTemplatesRepository : RepositoryBase
    {
        #region Constructors
        public EmailTemplatesRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods        
        public async Task<EmailTemplateDTO> EmailTemplatesGetSingleByID(int? emailTemplateID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesGetSingleByID)}({nameof(emailTemplateID)} = {emailTemplateID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(EmailTemplatesGetSingleByID),
                            sqlParameters:
                            [
                                emailTemplateID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<EmailTemplateDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> EmailTemplatesIUD(Enums.DatabaseActions databaseAction, int? emailTemplateID, EmailTemplateIudDTO emailTemplate)
        {
            var emailTemplateJson = emailTemplate.ToJson();

            emailTemplateID = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(emailTemplateID)} = {emailTemplateID}, {nameof(emailTemplate)} = {emailTemplateJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(EmailTemplatesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                emailTemplateID.ToSqlParameterOutput(SqlDbType.Int),
                                emailTemplateJson.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        emailTemplateID = sqb.GetNextOutputParameterValue<int?>();
                        return emailTemplateID;
                    }
                }
            );
            return emailTemplateID;
        }

        public async Task<List<EmailTemplatesListDTO>> EmailTemplatesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(EmailTemplatesList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<EmailTemplatesListDTO>();
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<string> EmailTemplatesWrapInLayout(string websiteHttpPath, string languageCultureCode, string bodyText, string urlUnsubscribe = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesWrapInLayout)}({nameof(websiteHttpPath)} = {websiteHttpPath}, {nameof(languageCultureCode)} = {languageCultureCode}, {nameof(bodyText)} = {bodyText}, {nameof(urlUnsubscribe)} = {urlUnsubscribe})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(EmailTemplatesWrapInLayout),
                            sqlParameters:
                            [
                                websiteHttpPath.ToSqlParameter(SqlDbType.NVarChar),
                                languageCultureCode.ToSqlParameter(SqlDbType.VarChar),
                                bodyText.ToSqlParameter(SqlDbType.NVarChar),
                                urlUnsubscribe.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<string>();                                                
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}