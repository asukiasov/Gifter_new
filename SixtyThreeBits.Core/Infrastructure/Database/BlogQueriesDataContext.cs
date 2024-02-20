using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
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
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(BlogPostList),
                itemType: typeof(BlogPostListEntity)
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
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(BlogPostGetSingleByID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
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
        public async Task<bool> BlogPostIsSlugUniq(string bBlogPostSlug, int? blogPostID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(BlogPostIsSlugUniq),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    bBlogPostSlug.ToSqlParameter(nameof(bBlogPostSlug), SqlDbType.NVarChar),
                    blogPostID.ToSqlParameter(nameof(blogPostID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<bool>();
            return result;
        }
        #endregion        
    }
}