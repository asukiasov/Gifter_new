using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Infrastructure.Base;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Database.Core;
using SixtyThreeBits.Core.Infrastructure.DTO;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class TeamMembersRepository : RepositoryBase
    {
        #region Constructors
        public TeamMembersRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.TeamMembersListEntity, TeamMemberDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<TeamMemberDTO> TeamMembersGetSingleByID(int? TeamMemberID)
        {
            return await TryToReturnAsyncTask($"{nameof(TeamMembersGetSingleByID)}({nameof(TeamMemberID)} = {TeamMemberID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.TeamMembersGetSingleByID(TeamMemberID);
                    return Result?.DeserializeJsonTo<TeamMemberDTO>();
                }
            });
        }

        public async Task<int?> TeamMembersIUD(Enums.DatabaseActions databaseAction, int? teamMemberID = null, string teamMemberFirstname = null, string teamMemberLastName = null, string teamMemberPosition = null, string teamMemberShortDescription = null, string teamMemberLongDescription = null, string teamMemberImageFilename = null, bool? teamMemberIsPublished = null, int? teamMemberCategoryID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(TeamMembersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(teamMemberID)} = {teamMemberID}, {nameof(teamMemberFirstname)} = {teamMemberFirstname}, {nameof(teamMemberLastName)} = {teamMemberLastName}, {nameof(teamMemberPosition)} = {teamMemberPosition}), {nameof(teamMemberShortDescription)} = {teamMemberShortDescription}, {nameof(teamMemberLongDescription)} = {teamMemberLongDescription},{nameof(teamMemberImageFilename)} = {teamMemberImageFilename},{nameof(teamMemberIsPublished)} = {teamMemberIsPublished}, {nameof(teamMemberCategoryID)} = {teamMemberCategoryID}", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    teamMemberID = await db.TeamMembersIUD(databaseAction, teamMemberID, teamMemberFirstname, teamMemberLastName, teamMemberPosition, teamMemberShortDescription, teamMemberLongDescription, teamMemberImageFilename, teamMemberIsPublished, teamMemberCategoryID);
                    return teamMemberID;
                }
            });
        }

        public async Task<List<TeamMemberDTO>> TeamMembersList()
        {
            return await TryToReturnAsyncTask($"{nameof(TeamMembersList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.TeamMembersList().OrderByDescending(item => item.TeamMemberDateCreated).ToListAsync())?.Select(item => _mapper.Map<TeamMemberDTO>(item)).ToList();
                }
            });
        }

        public async Task TeamMembersSyncSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var SortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(TeamMembersSyncSortIndexes)}({nameof(sortIndexes)} = {SortIndexesJson})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.TeamMembersSyncSortIndexes(SortIndexesJson);
                }
            });
        }
        #endregion
    }        
}
