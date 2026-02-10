using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class UsersRepository : RepositoryBase
    {
        #region Contructors
        public UsersRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
        {            
        }
        #endregion

        #region Methods                
        public async Task<UserDTO> UsersGetSingleByID(int? userID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleByID)}({nameof(userID)} = {userID})", 
                asyncFuncToTry: async () =>
                {                    
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByID),
                            sqlParameters:
                            [
                                userID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<UserDTO>();                                                
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<UserDTO> UsersGetSingleByEmailAndPassword(string userEmail, string userPassword)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleByEmailAndPassword)}({nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByEmailAndPassword),
                            sqlParameters:
                            [
                                userEmail.ToSqlParameter(SqlDbType.VarChar),
                                userPassword.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<UserDTO>();

                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<bool> UsersIsEmailUnique(string userEmail, int? userID = null)
        {
            var result = await TryToReturn(
                logString: $"{nameof(UsersIsEmailUnique)}({nameof(userEmail)} = {userEmail}, {nameof(userID)} = {userID})", 
                funcToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersIsEmailUnique),
                            sqlParameters:
                            [
                                userEmail.ToSqlParameter(SqlDbType.NVarChar),
                                userID.ToSqlParameter(SqlDbType.Int)
                            ]
                        );
                        var result = await sqb.ExecuteScalarValuedFunction<bool>();                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> UsersIUD(Enums.DatabaseActions databaseAction, int? userID, UserIudDTO user)
        {
            var userJson = user.ToJson();

            userID = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(userID)} = {user}, {nameof(userJson)} = {userJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                userID.ToSqlParameterOutput(SqlDbType.Int),
                                userJson.ToSqlParameter(SqlDbType.NVarChar),
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        userID = sqb.GetNextOutputParameterValue<int?>();
                        return userID;
                    }
                }
            );
            return userID;
        }

        public async Task<List<UsersListDTO>> UsersList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<UsersListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.UserDateCreated);
                        var result = await resultQueryable.ToListAsync();
                        
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<UserDTO> UsersGetSingleByGoogleID(string userGoogleID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleByGoogleID)}({nameof(userGoogleID)} = {userGoogleID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByGoogleID),
                            sqlParameters:
                            [
                                userGoogleID.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<UserDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<UserDTO> UsersGetSingleByEmail(string userEmail)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleByEmail)}({nameof(userEmail)} = {userEmail})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(UsersGetSingleByEmail),
                            sqlParameters:
                            [
                                userEmail.ToSqlParameter(SqlDbType.NVarChar)
                            ]
                        );
                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<UserDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> UsersUpdateGoogleID(int userID, string googleID, string avatarUrl)
        {
            return await UsersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                userID: userID,
                user: new UserIudDTO
                {
                    UserGoogleID = googleID,
                    UserAvatarFilename = avatarUrl
                }
            );
        }

        public async Task<int?> UsersUpdateAvatar(int userID, string avatarUrl)
        {
            return await UsersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                userID: userID,
                user: new UserIudDTO
                {
                    UserAvatarFilename = avatarUrl
                }
            );
        }

        public async Task<int?> UsersCreateFromGoogle(
            string googleID,
            string userEmail,
            string userFullname,
            string userFirstname,
            string userLastname,
            string avatarUrl)
        {
            return await UsersIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                userID: 0,
                user: new UserIudDTO
                {
                    UserGoogleID = googleID,
                    UserEmail = userEmail,
                    UserFullname = userFullname,
                    UserFirstname = userFirstname,
                    UserLastname = userLastname,
                    UserAvatarFilename = avatarUrl,
                    UserIsActive = true,
                    RoleID = 2 // Assuming 2 is 'User' role as per roadmap
                }
            );
        }

        public async Task<int?> UsersCompleteOnboarding(int userID)
        {
            return await UsersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                userID: userID,
                user: new UserIudDTO
                {
                    UserIsFirstLogin = false
                }
            );
        }
        #endregion Methods
    }    
}