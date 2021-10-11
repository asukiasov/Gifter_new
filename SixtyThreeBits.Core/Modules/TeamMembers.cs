using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    class TeamMembersDataAccess : DataAccessBase
    {
        #region Constructors
        public TeamMembersDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {

        }
        #endregion

        #region Methods

        public async Task<int?> TeamMembersIUD(Enums.DatabaseActions DatabaseAction, int? TeamMemberID = null, string TeamMemberFirstname = null, string TeamMemberLastName = null, string TeamMemberPosition = null, string TeamMemberShortDescription = null, string TeamMemberLongDescription =null, string TeamMemberImageFilename = null)
        {
            return await TryToReturnAsyncTask($"{nameof(TeamMembersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(TeamMemberID)} = {TeamMemberID}, {nameof(TeamMemberFirstname)} = {TeamMemberFirstname}, {nameof(TeamMemberLastName)} = {TeamMemberLastName}, {nameof(TeamMemberPosition)} = {TeamMemberPosition}), {nameof(TeamMemberShortDescription)} = {TeamMemberShortDescription}, {nameof(TeamMemberLongDescription)} = {TeamMemberLongDescription},{nameof(TeamMemberImageFilename)} = {TeamMemberImageFilename}", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    TeamMemberID = await db.TeamMembersIUD(DatabaseAction, TeamMemberID, TeamMemberFirstname, TeamMemberLastName, TeamMemberPosition, TeamMemberShortDescription, TeamMemberLongDescription, TeamMemberImageFilename);
                    return TeamMemberID;
                }
            });
        }

        public async Task<TeamMember> GetSingleTeamMemberID(int? TeamMemberId)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleTeamMemberID)}({nameof(TeamMemberId)} = {TeamMemberId})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.TeamMembersGetSingleByID(TeamMemberId);
                    return Result?.DeserializeTo<TeamMember>();
                }
            });
        }

        public async Task<List<DBCoreDataContext.TeamMembersListResultItem>> ListTeamMembers()
        {
            return await TryToReturnAsyncTask($"{nameof(ListTeamMembers)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.TeamMembersList().OrderByDescending(Item => Item.TeamMemberDateCreated).ToListAsync();
                }
            });
        }

        public class TeamMember
        {
            #region Properties
            public int? TeamMemberID { get; set; }
            public string TeamMemberFirstname { get; set; }
            public string TeamMemberLastname { get; set; }
            public string TeamMemberPosition { get; set; }
            public string TeamMemberShortDescription { get; set; }
            public string TeamMemberLongDescription { get; set; }
            public string TeamMemberImageFilename { get; set; }
            #endregion
        }
        #endregion
    }
}
