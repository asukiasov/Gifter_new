using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region PagesGetSingleByID        
        public async Task<string> PagesGetSingleByID(int? pageID, bool? pageIsPublished)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PagesGetSingleByID),
                sqlParameters:
                [
                    pageID.ToSqlParameter(nameof(pageID), SqlDbType.Int),
                    pageIsPublished.ToSqlParameter(nameof(pageIsPublished), SqlDbType.Bit)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region PagesGetSingleBySlugHierarchy        
        public async Task<string> PagesGetSingleBySlugHierarchy(string pageSlug, bool? pageIsPublished)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PagesGetSingleBySlugHierarchy),
                sqlParameters:
                [
                    pageSlug.ToSqlParameter(nameof(pageSlug), SqlDbType.NVarChar),
                    pageIsPublished.ToSqlParameter(nameof(pageIsPublished), SqlDbType.Bit)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion        

        #region PagesList
        public record PagesListEntity
        (
            int? PageID,
            int? PageParentID,
            string PageSlugHierarchy,
            string PageTitle,
            string PageTitleEng,
            string PageShortDescription,
            string PageShortDescriptionEng,
            string PageImageFilename,
            bool PageIsPublished,
            int? PageSortIndex,
            bool PageIsMenuItem,
            bool PageIsFooterItem,
            bool PageIsExternalUrl,
            string PageExternalUrl,
            DateTime? PageDateCreated
        );
        public IQueryable<PagesListEntity> PagesList(bool? pageIsPublished, bool? pageIsMenuItem)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PagesList),
                sqlParameters:
                [
                    pageIsPublished.ToSqlParameter(nameof(pageIsPublished), SqlDbType.Bit),
                    pageIsMenuItem.ToSqlParameter(nameof(pageIsMenuItem), SqlDbType.Bit)
                ]
            );
            var result = sqb.ExecuteQuery<PagesListEntity>();
            return result;
        }
        #endregion

        #region PagesListForDeleteRecursive        
        public record PagesListForDeleteRecursiveEntity
        (
            int? PageID
        );
        public IQueryable<PagesListForDeleteRecursiveEntity> PagesListForDeleteRecursive(int? pageID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(PagesListForDeleteRecursive),
                sqlParameters:
                [
                    pageID.ToSqlParameter(nameof(pageID), SqlDbType.Int),
                ]
            );
            var result = sqb.ExecuteQuery<PagesListForDeleteRecursiveEntity>();
            return result;
        }
        #endregion
    }
}