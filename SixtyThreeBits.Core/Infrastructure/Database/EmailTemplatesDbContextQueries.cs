using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region EmailTemplatesList
        public record EmailTemplatesListEntity
        (
            int? EmailTemplateID,
            string EmailTemplateName,
            string EmailTemplateBody
        );
        public IQueryable<EmailTemplatesListEntity> EmailTemplatesList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(EmailTemplatesList)
            );
            var result = sqb.ExecuteQuery<EmailTemplatesListEntity>();
            return result;
        }
        #endregion

        #region EmailTemplatesGetSingleByID        
        public async Task<string> EmailTemplatesGetSingleByID(int? emailTemplateID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(EmailTemplatesGetSingleByID),
                sqlParameters:
                [
                    emailTemplateID.ToSqlParameter(nameof(emailTemplateID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region EmailTemplatesWrapInLayout
        public async Task<string> EmailTemplatesWrapInLayout(string websiteHttpPath, string languageCultureCode, string bodyText, string urlUnsubscribe)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(EmailTemplatesWrapInLayout),
                sqlParameters:
                [
                    websiteHttpPath.ToSqlParameter(nameof(websiteHttpPath), SqlDbType.NVarChar),
                    languageCultureCode.ToSqlParameter(nameof(languageCultureCode), SqlDbType.VarChar),
                    bodyText.ToSqlParameter(nameof(bodyText), SqlDbType.NVarChar),
                    urlUnsubscribe.ToSqlParameter(nameof(urlUnsubscribe), SqlDbType.NVarChar)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion
    }
}