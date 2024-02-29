using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region TeamMembersGetSingleByID        
        public async Task<string> TeamMembersGetSingleByID(int? teamMemberID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(TeamMembersGetSingleByID),
                sqlParameters:
                [
                    teamMemberID.ToSqlParameter(nameof(teamMemberID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region TeamMembersList
        public record TeamMembersListEntity
        (
            int? TeamMemberID,
            string TeamMemberFirstname,
            string TeamMemberLastname,
            string TeamMemberPosition,
            string TeamMemberLongDescription,
            string TeamMemberImageFilename,
            bool? TeamMemberIsPublished,
            int? TeamMemberCategoryID,
            int? TeamMemberSortIndex,
            DateTime? TeamMemberDateCreated
        );
        public IQueryable<TeamMembersListEntity> TeamMembersList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(TeamMembersList)
            );
            var result = sqb.ExecuteQuery<TeamMembersListEntity>();
            return result;
        }
        #endregion
    }
}