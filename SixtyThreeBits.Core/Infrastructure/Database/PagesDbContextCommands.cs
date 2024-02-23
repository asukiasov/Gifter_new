using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task PagesDeleteRecursive(int? pageID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(PagesDeleteRecursive),
                itemType: null,
                sqlParameters:
                [
                    pageID.ToSqlParameter(nameof(pageID),SqlDbType.Int)
                ]
           );

            await sqb.ExecuteCommand();
        }

        public async Task<int?> PagesIUD(Enums.DatabaseActions databaseAction, int? pageID, int? pageParentID, string pageSlug, string pageTitle, string pageTitleEng, string pageText, string pageTextEng, string pageTextHeaderHtml, string pageTextHeaderHtmlEng, string pageTextFooterHtml, string pageTextFooterHtmlEng, string pageData, string pageDataEng, string pageShortDescription, string pageShortDescriptionEng, string pageImageFilename, bool? pageIsPublished, int? pageSortIndex, bool? pageIsMenuItem, bool? pageIsFooterItem, bool? pageIsExternalUrl, string pageExternalUrl)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(PagesIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    pageID.ToSqlOutputParameter(nameof(pageID),SqlDbType.Int),
                    pageParentID.ToSqlParameter(nameof(pageParentID),SqlDbType.Int),
                    pageSlug.ToSqlParameter(nameof(pageSlug),SqlDbType.VarChar),
                    pageTitle.ToSqlParameter(nameof(pageTitle),SqlDbType.NVarChar),
                    pageTitleEng.ToSqlParameter(nameof(pageTitleEng),SqlDbType.NVarChar),
                    pageText.ToSqlParameter(nameof(pageText),SqlDbType.NVarChar),
                    pageTextEng.ToSqlParameter(nameof(pageTextEng),SqlDbType.NVarChar),
                    pageTextHeaderHtml.ToSqlParameter(nameof(pageTextHeaderHtml),SqlDbType.NVarChar),
                    pageTextHeaderHtmlEng.ToSqlParameter(nameof(pageTextHeaderHtmlEng),SqlDbType.NVarChar),
                    pageTextFooterHtml.ToSqlParameter(nameof(pageTextFooterHtml),SqlDbType.NVarChar),
                    pageTextFooterHtmlEng.ToSqlParameter(nameof(pageTextFooterHtmlEng),SqlDbType.NVarChar),
                    pageData.ToSqlParameter(nameof(pageData),SqlDbType.NVarChar),
                    pageDataEng.ToSqlParameter(nameof(pageDataEng),SqlDbType.NVarChar),
                    pageShortDescription.ToSqlParameter(nameof(pageShortDescription),SqlDbType.NVarChar),
                    pageShortDescriptionEng.ToSqlParameter(nameof(pageShortDescriptionEng),SqlDbType.NVarChar),
                    pageImageFilename.ToSqlParameter(nameof(pageImageFilename),SqlDbType.NVarChar),
                    pageIsPublished.ToSqlParameter(nameof(pageIsPublished),SqlDbType.Bit),
                    pageSortIndex.ToSqlParameter(nameof(pageSortIndex),SqlDbType.Int),
                    pageIsMenuItem.ToSqlParameter(nameof(pageIsMenuItem),SqlDbType.Bit),
                    pageIsFooterItem.ToSqlParameter(nameof(pageIsFooterItem),SqlDbType.Bit),
                    pageIsExternalUrl.ToSqlParameter(nameof(pageIsExternalUrl),SqlDbType.Bit),
                    pageExternalUrl.ToSqlParameter(nameof(pageExternalUrl),SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
            pageID = sqb.GetNextOutputParameterValue<int?>();
            return pageID;
        }

        public async Task PagesSyncParentsAndSortIndexes(string SortIndexesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(PagesSyncParentsAndSortIndexes),
                itemType: null,
                sqlParameters:
                [
                    SortIndexesJson.ToSqlParameter(nameof(SortIndexesJson),SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
        }
        #endregion
    }
}