using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
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
        public TeamMembersRepository(DbContextFactory connectionFactory) : base(connectionFactory)
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

        public async Task<int?> TeamMembersIUD(Enums.DatabaseActions databaseAction, int? teamMemberID = null, string teamMemberFirstname = null, string teamMemberLastName = null, string teamMemberPosition = null, string teamMemberShortDescription = null, string teamMemberLongDescription = null, string teamMemberImageFilename = null, bool? teamMemberIsPublished = null, int? teamMemberCategoryID = null)
        {
            teamMemberID = await TryToReturnAsyncTask(
                logString: $"{nameof(TeamMembersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(teamMemberID)} = {teamMemberID}, {nameof(teamMemberFirstname)} = {teamMemberFirstname}, {nameof(teamMemberLastName)} = {teamMemberLastName}, {nameof(teamMemberPosition)} = {teamMemberPosition}), {nameof(teamMemberShortDescription)} = {teamMemberShortDescription}, {nameof(teamMemberLongDescription)} = {teamMemberLongDescription},{nameof(teamMemberImageFilename)} = {teamMemberImageFilename},{nameof(teamMemberIsPublished)} = {teamMemberIsPublished}, {nameof(teamMemberCategoryID)} = {teamMemberCategoryID}", 
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
                                teamMemberFirstname.ToSqlParameter(nameof(teamMemberFirstname), SqlDbType.NVarChar),
                                teamMemberLastName.ToSqlParameter(nameof(teamMemberLastName), SqlDbType.NVarChar),
                                teamMemberPosition.ToSqlParameter(nameof(teamMemberPosition), SqlDbType.NVarChar),
                                teamMemberShortDescription.ToSqlParameter(nameof(teamMemberShortDescription), SqlDbType.NVarChar),
                                teamMemberLongDescription.ToSqlParameter(nameof(teamMemberLongDescription), SqlDbType.NVarChar),
                                teamMemberImageFilename.ToSqlParameter(nameof(teamMemberImageFilename), SqlDbType.NVarChar),
                                teamMemberIsPublished.ToSqlParameter(nameof(teamMemberIsPublished), SqlDbType.Bit),
                                teamMemberCategoryID.ToSqlParameter(nameof(teamMemberCategoryID), SqlDbType.Int),
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

        public async Task<List<TeamMemberDTO>> TeamMembersList()
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

                        var resultQueryable = sqb.ExecuteTableValuedFunction<TeamMemberDTO>();
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
