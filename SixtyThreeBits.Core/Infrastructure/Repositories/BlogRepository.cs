using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class BlogRepository : RepositoryBase
    {
        #region Constructors
        public BlogRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.BlogPostListEntity, BlogPostDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<BlogPostDTO> BlogPostGetSingleByID(int? blogPostID)
        {
            return await TryToReturnAsyncTask($"{nameof(BlogPostGetSingleByID)}({nameof(blogPostID)} = {blogPostID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.BlogPostGetSingleByID(blogPostID);
                    return Result?.DeserializeJsonTo<BlogPostDTO>();
                }
            });
        }

        public async Task<bool> BlogPostIsSlugUniq(string blogPostSlug, int? blogPostID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(BlogPostIsSlugUniq)}({nameof(blogPostSlug)} = {blogPostSlug}, {nameof(blogPostID)} = {blogPostID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return await db.BlogPostIsSlugUniq(blogPostSlug, blogPostID);
                }
            });
        }

        public async Task<int?> BlogIUD(Enums.DatabaseActions databaseAction, int? blogPostID = null, string blogPostSlug = null, string blogPostTitle = null, string blogPostShortText = null, string blogPostText = null, string blogPostAuthorName = null, string blogPostImageFilename = null, DateTime? blogPostDate = null, bool? blogPostIsPublished = null)
        {
            return await TryToReturnAsyncTask($"{nameof(BlogIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(blogPostID)} = {blogPostID}, {nameof(blogPostSlug)} = {blogPostSlug}, {nameof(blogPostTitle)} = {blogPostTitle}, {nameof(blogPostShortText)} = {blogPostShortText}, {nameof(blogPostText)} = {blogPostText}, {nameof(blogPostAuthorName)} = {blogPostAuthorName}, {nameof(blogPostImageFilename)} = {blogPostImageFilename}, {nameof(blogPostDate)} = {blogPostDate}, {nameof(blogPostIsPublished)} = {blogPostIsPublished})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    blogPostID = await db.BlogIUD(databaseAction, blogPostID, blogPostSlug, blogPostTitle, blogPostShortText, blogPostText, blogPostAuthorName, blogPostImageFilename, blogPostDate, blogPostIsPublished);
                    return blogPostID;
                }
            });
        }

        public async Task<List<BlogPostDTO>> BlogPostList()
        {
            return await TryToReturn($"{nameof(BlogPostList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.BlogPostList().OrderByDescending(item => item.BlogPostDateCreated).ToListAsync())?.Select(item => _mapper.Map<BlogPostDTO>(item)).ToList();
                }
            });
        }
        #endregion
    }        
}
