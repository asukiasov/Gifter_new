using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BlogPostsRepository : RepositoryBase
    {
        #region Constructors
        public BlogPostsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
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

        public async Task<int?> BlogIUD(Enums.DatabaseActions databaseAction, int? blogPostID = null, string blogPostSlug = null, string blogPostTitle = null, string blogPostShortText = null, string blogPostText = null, string blogPostAuthorName = null, string blogPostImageFilename = null, DateTime? blogPostDate = null, bool? blogPostIsPublished = null)
        {
            blogPostID = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(blogPostID)} = {blogPostID}, {nameof(blogPostSlug)} = {blogPostSlug}, {nameof(blogPostTitle)} = {blogPostTitle}, {nameof(blogPostShortText)} = {blogPostShortText}, {nameof(blogPostText)} = {blogPostText}, {nameof(blogPostAuthorName)} = {blogPostAuthorName}, {nameof(blogPostImageFilename)} = {blogPostImageFilename}, {nameof(blogPostDate)} = {blogPostDate}, {nameof(blogPostIsPublished)} = {blogPostIsPublished})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogIUD),
                            sqlParameters:
                            [
                                 databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                 blogPostID.ToSqlOutputParameter(nameof(blogPostID),SqlDbType.Int),
                                 blogPostSlug.ToSqlParameter(nameof(blogPostSlug),SqlDbType.NVarChar),
                                 blogPostTitle.ToSqlParameter(nameof(blogPostTitle),SqlDbType.NVarChar),
                                 blogPostShortText.ToSqlParameter(nameof(blogPostShortText),SqlDbType.NVarChar),
                                 blogPostText.ToSqlParameter(nameof(blogPostText),SqlDbType.NVarChar),
                                 blogPostAuthorName.ToSqlParameter(nameof(blogPostAuthorName),SqlDbType.NVarChar),
                                 blogPostImageFilename.ToSqlParameter(nameof(blogPostImageFilename),SqlDbType.NVarChar),
                                 blogPostDate.ToSqlParameter(nameof(blogPostDate),SqlDbType.Date),
                                 blogPostIsPublished.ToSqlParameter(nameof(blogPostIsPublished), SqlDbType.Bit)
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

        public async Task<List<BlogPostDTO>> BlogPostList()
        {
            var result = await TryToReturn(
                logString: $"{nameof(BlogPostList)}()", 
                funcToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(BlogPostList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<BlogPostDTO>();
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