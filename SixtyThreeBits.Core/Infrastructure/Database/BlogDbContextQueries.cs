using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region BlogsList
        public record BlogPostListEntity
        (
            int? BlogPostID,
            string BlogPostSlug,
            string BlogPostTitle,
            string BlogPostShortText,
            string BlogPostAuthorName,
            bool BlogPostIsPublished,
            DateTime? BlogPostDate,
            DateTime? BlogPostDateCreated
        );
        public IQueryable<BlogPostListEntity> BlogPostList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(BlogPostList)
            );
            var result = sqb.ExecuteQuery<BlogPostListEntity>();
            return result;
        }
        #endregion

        #region BlogsGetSingleByID
        public async Task<string> BlogPostGetSingleByID(int? blogPostID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(BlogPostGetSingleByID),
                sqlParameters:
                [
                    blogPostID.ToSqlParameter(nameof(blogPostID), SqlDbType.Int),
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region BlogsIsSlugUniq        
        public async Task<bool> BlogPostIsSlugUniq(string blogPostSlug, int? blogPostID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(BlogPostIsSlugUniq),
                sqlParameters:
                [
                    blogPostSlug.ToSqlParameter(nameof(blogPostSlug), SqlDbType.NVarChar),
                    blogPostID.ToSqlParameter(nameof(blogPostID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<bool>();
            return result;
        }
        #endregion        
    }
}