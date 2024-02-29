using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task<int?> TeamMembersIUD(Enums.DatabaseActions databaseAction, int? teamMemberID, string teamMemberFirstName, string teamMemberLastName, string teamMemberPosition, string teamMemberShortDescription, string teamMemberLongDescription, string teamMemberImageFilename, bool? teamMemberIsPublished, int? teamMemberCategoryID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(TeamMembersIUD),
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction), SqlDbType.TinyInt),
                    teamMemberID.ToSqlOutputParameter(nameof(teamMemberID), SqlDbType.Int),
                    teamMemberFirstName.ToSqlParameter(nameof(teamMemberFirstName), SqlDbType.NVarChar),
                    teamMemberLastName.ToSqlParameter(nameof(teamMemberLastName), SqlDbType.NVarChar),
                    teamMemberPosition.ToSqlParameter(nameof(teamMemberPosition), SqlDbType.NVarChar),
                    teamMemberShortDescription.ToSqlParameter(nameof(teamMemberShortDescription), SqlDbType.NVarChar),
                    teamMemberLongDescription.ToSqlParameter(nameof(teamMemberLongDescription), SqlDbType.NVarChar),
                    teamMemberImageFilename.ToSqlParameter(nameof(teamMemberImageFilename), SqlDbType.NVarChar),
                    teamMemberIsPublished.ToSqlParameter(nameof(teamMemberIsPublished), SqlDbType.Bit),
                    teamMemberCategoryID.ToSqlParameter(nameof(teamMemberCategoryID), SqlDbType.Int),
                ]
            );

            await sqb.ExecuteCommand();
            teamMemberID = sqb.GetNextOutputParameterValue<int?>();
            return teamMemberID;
        }

        public async Task TeamMembersSyncSortIndexes(string teamMembersSyncSortIndexesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(TeamMembersSyncSortIndexes),
                sqlParameters:
                [
                    teamMembersSyncSortIndexesJson.ToSqlParameter(nameof(teamMembersSyncSortIndexesJson), SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
        }
        #endregion
    }
}