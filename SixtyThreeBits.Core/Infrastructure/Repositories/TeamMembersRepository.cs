using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class TeamMembersRepository : RepositoryBase
    {
        #region Constructors
        public TeamMembersRepository(DbContextFactory dbContextFactory) : base(dbContextFactory)
        {            
        }
        #endregion

        #region Methods
        public async Task<TeamMemberDTO> TeamMembersGetSingleByID(int? teamMemberID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(TeamMembersGetSingleByID)}({nameof(teamMemberID)} = {teamMemberID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(TeamMembersGetSingleByID),
                            sqlParameters:
                            [
                                teamMemberID.ToSqlParameter(nameof(teamMemberID), SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<TeamMemberDTO>();
                        return result;                        
                    }
                }
            );
            return result;
        }

        public async Task<int?> TeamMembersIUD(Enums.DatabaseActions databaseAction, int? teamMemberID, TeamMemberIudDTO teamMember)
        {
            var teamMemberJson = teamMember.ToJson();

            teamMemberID = await TryToReturnAsyncTask(
                logString: $"{nameof(TeamMembersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(teamMemberID)} = {teamMemberID}, {nameof(teamMemberJson)} = {teamMemberJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(TeamMembersIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction), SqlDbType.TinyInt),
                                teamMemberID.ToSqlOutputParameter(nameof(teamMemberID), SqlDbType.Int),
                                teamMemberJson.ToSqlParameter(nameof(teamMemberJson), SqlDbType.NVarChar)                                
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        teamMemberID = sqb.GetNextOutputParameterValue<int?>();
                        return teamMemberID;
                    }
                }
            );
            return teamMemberID;
        }

        public async Task<List<TeamMembersListDTO>> TeamMembersList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(TeamMembersList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(TeamMembersList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<TeamMembersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.TeamMemberDateCreated);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task TeamMembersSyncSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(TeamMembersSyncSortIndexes)}({nameof(sortIndexes)} = {sortIndexesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(TeamMembersSyncSortIndexes),
                            sqlParameters:
                            [
                                sortIndexesJson.ToSqlParameter(nameof(sortIndexesJson), SqlDbType.NVarChar)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
        }
        #endregion
    }        
}
