using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
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
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(EmailTemplatesList),
                itemType: typeof(EmailTemplatesListEntity)
            );
            var result = sqb.ExecuteQuery<EmailTemplatesListEntity>();
            return result;
        }
        #endregion

        #region EmailTemplatesGetSingleByID        
        public async Task<string> EmailTemplatesGetSingleByID(int? emailTemplatesID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(EmailTemplatesGetSingleByID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    emailTemplatesID.ToSqlParameter(nameof(emailTemplatesID), SqlDbType.Int)
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
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(EmailTemplatesWrapInLayout),
                itemType: typeof(ScalarFunctionResultEntity<string>),
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