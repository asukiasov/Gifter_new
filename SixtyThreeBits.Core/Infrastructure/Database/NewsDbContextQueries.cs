using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
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
                databaseObjectName: nameof(NewsList)
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
                databaseObjectName: nameof(NewsGetSingleByID),
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
                databaseObjectName: nameof(NewsIsSlugUniq),
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