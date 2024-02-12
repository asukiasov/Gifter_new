using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBCommandsDataContext
    {
        #region Methods
        public async Task<int?> NewsIUD(Enums.DatabaseActions databaseAction, int? newsID, string newsSlug, string newsTitle, string newsTitleEng, string newsText, string newsTextEng, string newsShortDescription, string newsShortDescriptionEng, string newsImageFilename, DateTime? newsDatePublished, bool? newsIsPublished)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(NewsIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    newsID.ToSqlParameter(nameof(newsID),SqlDbType.Int,true),
                    newsSlug.ToSqlParameter(nameof(newsSlug),SqlDbType.NVarChar),
                    newsTitle.ToSqlParameter(nameof(newsTitle),SqlDbType.NVarChar),
                    newsTitleEng.ToSqlParameter(nameof(newsTitleEng),SqlDbType.NVarChar),
                    newsText.ToSqlParameter(nameof(newsText),SqlDbType.NVarChar),
                    newsTextEng.ToSqlParameter(nameof(newsTextEng),SqlDbType.NVarChar),
                    newsShortDescription.ToSqlParameter(nameof(newsShortDescription),SqlDbType.NVarChar),
                    newsShortDescriptionEng.ToSqlParameter(nameof(newsShortDescriptionEng),SqlDbType.NVarChar),
                    newsImageFilename.ToSqlParameter(nameof(newsImageFilename),SqlDbType.NVarChar),
                    newsDatePublished.ToSqlParameter(nameof(newsDatePublished),SqlDbType.DateTime),
                    newsIsPublished.ToSqlParameter(nameof(newsIsPublished),SqlDbType.Bit)
                ]
             );

            await sqb.ExecuteCommand();
            newsID = sqb.GetNextOutputParameterValue<int?>();
            return newsID;
        }
        #endregion
    }
}