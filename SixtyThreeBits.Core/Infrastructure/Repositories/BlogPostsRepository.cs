using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BlogPostsRepository : RepositoryBase
    {
        #region Constructors
        public BlogPostsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods
        public async Task<BlogPostDTO> BlogPostGetSingleByID(int? blogPostID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogPostGetSingleByID)}({nameof(blogPostID)} = {blogPostID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogPostGetSingleByID),
                            sqlParameters:
                            [
                                blogPostID.ToSqlParameter(nameof(blogPostID), SqlDbType.Int),
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<BlogPostDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<bool> BlogPostIsSlugUniq(string blogPostSlug, int? blogPostID = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogPostIsSlugUniq)}({nameof(blogPostSlug)} = {blogPostSlug}, {nameof(blogPostID)} = {blogPostID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogPostIsSlugUniq),
                            sqlParameters:
                            [
                                blogPostSlug.ToSqlParameter(nameof(blogPostSlug), SqlDbType.NVarChar),
                                blogPostID.ToSqlParameter(nameof(blogPostID), SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> BlogPostsIUD(Enums.DatabaseActions databaseAction, int? blogPostID, BlogPostIudDTO blogPost)
        {
            var blogPostJson = blogPost.ToJson();

            blogPostID = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogPostsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(blogPostID)} = {blogPostID}, {nameof(blogPost)} = {blogPostJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogPostsIUD),
                            sqlParameters:
                            [
                                 databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                 blogPostID.ToSqlParameterOutput(nameof(blogPostID),SqlDbType.Int),
                                 blogPostJson.ToSqlParameter(nameof(blogPostJson),SqlDbType.NVarChar)                                 
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();
                        blogPostID = sqb.GetNextOutputParameterValue<int?>();          
                        return blogPostID;
                    }
                }
            );
            return blogPostID;
        }

        public async Task<List<BlogPostListDTO>> BlogPostList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogPostList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogPostList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<BlogPostListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.BlogPostDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }        
}