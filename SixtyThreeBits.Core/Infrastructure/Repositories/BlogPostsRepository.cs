using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BlogPostsRepository : RepositoryBase
    {
        #region Constructors
        public BlogPostsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbContextQueries.BlogPostListEntity, BlogPostDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<BlogPostDTO> BlogPostGetSingleByID(int? blogPostID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(BlogPostGetSingleByID)}({nameof(blogPostID)} = {blogPostID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.BlogPostGetSingleByID(blogPostID: blogPostID);
                        var result = resultJson?.DeserializeJsonTo<BlogPostDTO>();
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = await db.BlogPostIsSlugUniq(blogPostSlug: blogPostSlug, blogPostID: blogPostID);
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
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        blogPostID = await db.BlogIUD(
                            databaseAction: databaseAction, 
                            blogPostID: blogPostID,
                            blogPostSlug: blogPostSlug, 
                            blogPostTitle: blogPostTitle, 
                            blogPostShortText: blogPostShortText, 
                            blogPostText: blogPostText, 
                            blogPostAuthorName: blogPostAuthorName, 
                            blogPostImageFilename: blogPostImageFilename, 
                            blogPostDate: blogPostDate, 
                            blogPostIsPublished: blogPostIsPublished
                        );
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.BlogPostList()
                            .OrderByDescending(item => item.BlogPostDateCreated)
                            .ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<BlogPostDTO>(item))
                        .ToList();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }        
}
