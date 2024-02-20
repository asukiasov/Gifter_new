using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task<int?> EmailTemplatesIUD(Enums.DatabaseActions databaseAction, int? emailTemplateID, string emailTemplateName, string emailTemplateSubject, string emailTemplateSubjectEng, string emailTemplateBody, string emailTemplateBodyEng)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(EmailTemplatesIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    emailTemplateID.ToSqlParameter(nameof(emailTemplateID),SqlDbType.Int, true),
                    emailTemplateName.ToSqlParameter(nameof(emailTemplateName),SqlDbType.NVarChar),
                    emailTemplateSubject.ToSqlParameter(nameof(emailTemplateSubject),SqlDbType.NVarChar),
                    emailTemplateSubjectEng.ToSqlParameter(nameof(emailTemplateSubjectEng),SqlDbType.NVarChar),
                    emailTemplateBody.ToSqlParameter(nameof(emailTemplateBody),SqlDbType.NVarChar),
                    emailTemplateBodyEng.ToSqlParameter(nameof(emailTemplateBodyEng),SqlDbType.NVarChar)
                ]
           );

            await sqb.ExecuteCommand();
            emailTemplateID = sqb.GetNextOutputParameterValue<int?>();
            return emailTemplateID;
        }
        #endregion
    }
}