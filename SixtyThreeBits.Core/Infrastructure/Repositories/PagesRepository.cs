using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class PagesRepository : RepositoryBase
    {
        #region Contructors
        public PagesRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbContextQueries.PagesListEntity, PagesListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task PagesDeleteRecursive(int? pageID)
        {
            await TryExecuteAsyncTask($"{nameof(PagesDeleteRecursive)}({nameof(pageID)} = {pageID})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextCommands())
                {
                    await db.PagesDeleteRecursive(pageID);
                }
            });
        }

        public async Task<PageDTO> PagesGetSingleByID(int? pageID, bool? pageIsPublished = null)
        {
            var result = await TryToReturnAsyncTask($"{nameof(PagesGetSingleByID)}({nameof(pageID)} = {pageID}, {nameof(pageIsPublished)} = {pageIsPublished})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    var resultJson = await db.PagesGetSingleByID(pageID, pageIsPublished);
                    var result = resultJson?.DeserializeJsonTo<PageDTO>();
                    return result;
                }
            });
            return result;
        }

        public async Task<PageDTO> PagesGetSingleBySlugHierarchy(string pageSlug, bool? pageIsPublished = null)
        {
            var result = await TryToReturnAsyncTask($"{nameof(PagesGetSingleBySlugHierarchy)}({nameof(pageSlug)} = {pageSlug}, {nameof(pageIsPublished)} = {pageIsPublished})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    var resultJson = await db.PagesGetSingleBySlugHierarchy(pageSlug, pageIsPublished);
                    var result = resultJson?.DeserializeJsonTo<PageDTO>();
                    return result;
                }
            });
            return result;
        }

        public async Task<int?> PagesIUD(Enums.DatabaseActions databaseAction, int? pageID = null, int? pageParentID = null, string pageSlug = null, string pageTitle = null, string pageTitleEng = null, string pageText = null, string pageTextEng = null, string pageTextHeaderHtml = null, string pageTextHeaderHtmlEng = null, string pageTextFooterHtml = null, string pageTextFooterHtmlEng = null, string pageData = null, string pageDataEng = null, string pageShortDescription = null, string pageShortDescriptionEng = null, string pageImageFilename = null, bool? pageIsPublished = null, int? pageSortIndex = null, bool? pageIsMenuItem = null, bool? pageIsFooterItem = null, bool? pageIsExternalUrl = null, string pageExternalUrl = null)
        {
            pageID = await TryToReturnAsyncTask($"{nameof(PagesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(pageID)} = {pageID}, {nameof(pageParentID)} = {pageParentID}, {nameof(pageSlug)} = {pageSlug}, {nameof(pageTitle)} = {pageTitle}, {nameof(pageTitleEng)} = {pageTitleEng}, {nameof(pageText)} = {pageText}, {nameof(pageTextEng)} = {pageTextEng}, {nameof(pageTextHeaderHtml)} = {pageTextHeaderHtml}, {nameof(pageTextHeaderHtmlEng)} = {pageTextHeaderHtmlEng}, {nameof(pageTextFooterHtml)} = {pageTextFooterHtml}, {nameof(pageTextFooterHtmlEng)} = {pageTextFooterHtmlEng}, {nameof(pageData)} = {pageData}, {nameof(pageDataEng)} = {pageDataEng}, {nameof(pageShortDescription)} = {pageShortDescription}, {nameof(pageShortDescriptionEng)} = {pageShortDescriptionEng}, {nameof(pageImageFilename)} = {pageImageFilename}, {nameof(pageIsPublished)} = {pageIsPublished}, {nameof(pageSortIndex)} = {pageSortIndex}, {nameof(pageIsMenuItem)} = {pageIsMenuItem}, {nameof(pageIsFooterItem)} = {pageIsFooterItem}, {nameof(pageIsExternalUrl)} = {pageIsExternalUrl}, {nameof(pageExternalUrl)} = {pageExternalUrl})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextCommands())
                {
                    pageID = await db.PagesIUD(databaseAction, pageID, pageParentID, pageSlug, pageTitle, pageTitleEng, pageText, pageTextEng, pageTextHeaderHtml, pageTextHeaderHtmlEng, pageTextFooterHtml, pageTextFooterHtmlEng, pageData, pageDataEng, pageShortDescription, pageShortDescriptionEng, pageImageFilename, pageIsPublished, pageSortIndex, pageIsMenuItem, pageIsFooterItem, pageIsExternalUrl, pageExternalUrl);
                    return pageID;
                }
            });
            return pageID;
        }

        public async Task<List<PagesListDTO>> PagesList(bool? pageIsPublished = null, bool? pageIsMenuItem = null)
        {
            var result = await TryToReturnAsyncTask($"{nameof(PagesList)}({nameof(pageIsPublished)} = {pageIsPublished}, {nameof(pageIsMenuItem)} = {pageIsMenuItem})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextQueries())
                {
                    var result = (await db.PagesList(pageIsPublished, pageIsMenuItem).OrderBy(item => item.PageSortIndex).ToListAsync())?.Select(item => _mapper.Map<PagesListDTO>(item)).ToList();
                    return result;
                }
            });
            return result;
        }

        public async Task PagesSyncParentsAndSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(PagesSyncParentsAndSortIndexes)}({nameof(sortIndexes)} = {sortIndexesJson})", async () =>
            {
                using (var db = _connectionFactory.GetDbContextCommands())
                {
                    await db.PagesSyncParentsAndSortIndexes(sortIndexesJson);
                }
            });
        }
        #endregion
    }    
}