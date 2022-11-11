using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class EmailTemplatesDataAccess : DataAccessBase
    {
        #region Constructors
        public EmailTemplatesDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods
        public async Task<List<DBCoreDataContext.EmailTemplatesListResultItem>> ListEmailTemplates()
        {
            return await TryToReturnAsyncTask($"{nameof(ListEmailTemplates)}", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.EmailTemplatesList().ToListAsync();
                }
            });
        }
        public async Task<int?> EmailTemplatesIUD(Enums.DatabaseActions DatabaseAction, int? EmailTemplateID = null, string EmailTemplateName = null, string EmailTemplateSubject = null, string EmailTemplateSubjectEng = null, string EmailTemplateBody = null, string EmailTemplateBodyEng = null)
        {
            return await TryToReturnAsyncTask($"{nameof(EmailTemplatesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(EmailTemplateID)} = {EmailTemplateID}, {nameof(EmailTemplateName)} = {EmailTemplateName}, {nameof(EmailTemplateSubject)} = {EmailTemplateSubject}, {nameof(EmailTemplateSubjectEng)} = {EmailTemplateSubjectEng}, {nameof(EmailTemplateBody)} = {EmailTemplateBody}, {nameof(EmailTemplateBodyEng)} = {EmailTemplateBodyEng})", async () =>
            {
                 using (var db = ConnectionFactory.GetDBCoreDataContext())
                 {
                     await db.EmailTemplatesIUD(DatabaseAction, EmailTemplateID, EmailTemplateName, EmailTemplateSubject, EmailTemplateSubjectEng, EmailTemplateBody, EmailTemplateBodyEng);
                     return EmailTemplateID;
                 }
            });
        }
        public async Task<EmailTemplate> GetSingleEmailTemplateByID(int? EmailTemplateID)
        {
            return await TryToReturnStaticAsyncTask($"{nameof(GetSingleEmailTemplateByID)}({nameof(EmailTemplateID)} = {EmailTemplateID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.EmailTemplatesGetSingleByID(EmailTemplateID)).DeserializeJsonTo<EmailTemplate>();
                }
            });
        }
        #endregion
    }

    public class EmailTemplate
    {
        #region Properties
        public int? EmailTemplateID { get; set; }
        public string EmailTemplateName { get; set; }
        public string EmailTemplateSubject { get; set; }
        public string EmailTemplateBody { get; set; }
        public string EmailTemplateSubjectEng { get; set; }
        public string EmailTemplateBodyEng { get; set; }
        public List<SimpleKeyValue<string, string>> EmailTemplatesPlaceHolders { get; set; }
        #endregion
    }
}
