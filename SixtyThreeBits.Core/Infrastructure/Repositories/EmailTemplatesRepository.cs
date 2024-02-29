using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class EmailTemplatesRepository : RepositoryBase
    {
        #region Constructors
        public EmailTemplatesRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbContextQueries.EmailTemplatesListEntity, EmailTemplateDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods        
        public async Task<EmailTemplateDTO> EmailTemplatesGetSingleByID(int? emailTemplateID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesGetSingleByID)}({nameof(emailTemplateID)} = {emailTemplateID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.EmailTemplatesGetSingleByID(emailTemplateID)).DeserializeJsonTo<EmailTemplateDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> EmailTemplatesIUD(Enums.DatabaseActions databaseAction, int? emailTemplateID = null, string emailTemplateName = null, string emailTemplateSubject = null, string emailTemplateSubjectEng = null, string emailTemplateBody = null, string emailTemplateBodyEng = null)
        {
            emailTemplateID = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(emailTemplateID)} = {emailTemplateID}, {nameof(emailTemplateName)} = {emailTemplateName}, {nameof(emailTemplateSubject)} = {emailTemplateSubject}, {nameof(emailTemplateSubjectEng)} = {emailTemplateSubjectEng}, {nameof(emailTemplateBody)} = {emailTemplateBody}, {nameof(emailTemplateBodyEng)} = {emailTemplateBodyEng})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        emailTemplateID = await db.EmailTemplatesIUD(databaseAction, emailTemplateID, emailTemplateName, emailTemplateSubject, emailTemplateSubjectEng, emailTemplateBody, emailTemplateBodyEng);
                        return emailTemplateID;
                    }
                }
            );
            return emailTemplateID;
        }

        public async Task<List<EmailTemplateDTO>> EmailTemplatesList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(EmailTemplatesList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.EmailTemplatesList().ToListAsync())?.Select(item => _mapper.Map<EmailTemplateDTO>(item)).ToList();
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = await db.EmailTemplatesWrapInLayout(websiteHttpPath, languageCultureCode, bodyText, urlUnsubscribe);
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}