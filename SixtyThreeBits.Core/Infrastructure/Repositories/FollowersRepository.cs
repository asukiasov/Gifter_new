using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class FollowersRepository : RepositoryBase
    {
        #region Constructors
        public FollowersRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {
        }
        #endregion

        #region Methods
        public async Task FollowersFollow(int followingUserID, int followedUserID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(FollowersFollow)}({nameof(followingUserID)} = {followingUserID}, {nameof(followedUserID)} = {followedUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(FollowersFollow),
                            sqlParameters:
                            [
                                followingUserID.ToSqlParameter(SqlDbType.Int),
                                followedUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
        }

        public async Task FollowersUnfollow(int followingUserID, int followedUserID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(FollowersUnfollow)}({nameof(followingUserID)} = {followingUserID}, {nameof(followedUserID)} = {followedUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(FollowersUnfollow),
                            sqlParameters:
                            [
                                followingUserID.ToSqlParameter(SqlDbType.Int),
                                followedUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                    }
                }
            );
        }

        public async Task<bool> FollowersIsFollowing(int followingUserID, int followedUserID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(FollowersIsFollowing)}({nameof(followingUserID)} = {followingUserID}, {nameof(followedUserID)} = {followedUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(FollowersIsFollowing),
                            sqlParameters:
                            [
                                followingUserID.ToSqlParameter(SqlDbType.Int),
                                followedUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<FollowersListDTO>> FollowersList(int followingUserID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(FollowersList)}({nameof(followingUserID)} = {followingUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(FollowersList),
                            sqlParameters:
                            [
                                followingUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<FollowersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.FollowerDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<FollowersMyFollowersDTO>> FollowersMyFollowers(int followedUserID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(FollowersMyFollowers)}({nameof(followedUserID)} = {followedUserID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(FollowersMyFollowers),
                            sqlParameters:
                            [
                                followedUserID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<FollowersMyFollowersDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.FollowerDateCreated);
                        var result = await resultQueryable.ToListAsync();

                        return result;
                    }
                }
            );
            return result;
        }
        #endregion
    }
}
