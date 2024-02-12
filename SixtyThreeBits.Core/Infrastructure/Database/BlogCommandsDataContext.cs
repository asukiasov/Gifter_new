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
        public async Task<int?> BlogIUD(Enums.DatabaseActions iud, int? blogPostID, string blogPostlug, string blogPostTitle, string blogPostShortText, string blogPostText, string blogPostAuthorName, string blogPostImageFilename, DateTime? blogPostDate, bool? blogPostIsPublished)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(BlogIUD),
                itemType: null,
                sqlParameters:
                [
                     iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                     blogPostID.ToSqlParameter(nameof(blogPostID),SqlDbType.Int,true),
                     blogPostlug.ToSqlParameter(nameof(blogPostlug),SqlDbType.NVarChar),
                     blogPostTitle.ToSqlParameter(nameof(blogPostTitle),SqlDbType.NVarChar),
                     blogPostShortText.ToSqlParameter(nameof(blogPostShortText),SqlDbType.NVarChar),
                     blogPostText.ToSqlParameter(nameof(blogPostText),SqlDbType.NVarChar),
                     blogPostAuthorName.ToSqlParameter(nameof(blogPostAuthorName),SqlDbType.NVarChar),
                     blogPostImageFilename.ToSqlParameter(nameof(blogPostImageFilename),SqlDbType.NVarChar),
                     blogPostDate.ToSqlParameter(nameof(blogPostDate),SqlDbType.Date),
                     blogPostIsPublished.ToSqlParameter(nameof(blogPostIsPublished), SqlDbType.Bit)
                ]
            );
            await sqb.ExecuteCommand();
            blogPostID = sqb.GetNextOutputParameterValue<int?>();
            return blogPostID;
        }
        #endregion
    }
}