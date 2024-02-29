using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
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
                cfg.CreateMap<DbContextQueries.TeamMembersListEntity, TeamMemberDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task<TeamMemberDTO> TeamMembersGetSingleByID(int? TeamMemberID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(TeamMembersGetSingleByID)}({nameof(TeamMemberID)} = {TeamMemberID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.TeamMembersGetSingleByID(TeamMemberID);
                        var result = resultJson?.DeserializeJsonTo<TeamMemberDTO>();
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
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        teamMemberID = await db.TeamMembersIUD(databaseAction, teamMemberID, teamMemberFirstname, teamMemberLastName, teamMemberPosition, teamMemberShortDescription, teamMemberLongDescription, teamMemberImageFilename, teamMemberIsPublished, teamMemberCategoryID);
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.TeamMembersList().OrderByDescending(item => item.TeamMemberDateCreated).ToListAsync())?.Select(item => _mapper.Map<TeamMemberDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task TeamMembersSyncSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var SortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(TeamMembersSyncSortIndexes)}({nameof(sortIndexes)} = {SortIndexesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.TeamMembersSyncSortIndexes(SortIndexesJson);
                    }
                }
            );
        }
        #endregion
    }        
}
