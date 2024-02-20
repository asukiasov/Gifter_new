using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region NewsList
        public record NewsListEntity
        (
            int? NewsID,
            string NewsSlug,
            string NewsTitle,
            string NewsTitleEng,
            string NewsText,
            string NewsTextEng,
            string NewsShortDescription,
            string NewsShortDescriptionEng,
            string NewsImageFilename,
            DateTime? NewsDatePublished,
            bool NewsIsPublished,
            DateTime? NewsDateCreated
        );
        public IQueryable<NewsListEntity> NewsList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(NewsList),
                itemType: typeof(NewsListEntity)
            );
            var result = sqb.ExecuteQuery<NewsListEntity>();
            return result;
        }
        #endregion

        #region NewsGetSingleByID        
        public async Task<string> NewsGetSingleByID(int? newsID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(NewsGetSingleByID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    newsID.ToSqlParameter(nameof(newsID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region NewsIsSlugUniq        
        public async Task<bool> NewsIsSlugUniq(string newsSlug, int? newsID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(NewsIsSlugUniq),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    newsSlug.ToSqlParameter(nameof(newsSlug), SqlDbType.NVarChar),
                    newsID.ToSqlParameter(nameof(newsID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<bool>();
            return result;
        }
        #endregion
    }
}