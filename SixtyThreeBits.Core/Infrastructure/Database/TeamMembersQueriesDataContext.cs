using SixtyThreeBits.Core.Infrastructure.Database.Core;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DBQueriesDataContext
    {
        #region TeamMembersGetSingleByID        
        public async Task<string> TeamMembersGetSingleByID(int? teamMemberID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(TeamMembersGetSingleByID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
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
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(TeamMembersList),
                itemType: typeof(TeamMembersListEntity)
            );
            var result = sqb.ExecuteQuery<TeamMembersListEntity>();
            return result;
        }
        #endregion
    }
}