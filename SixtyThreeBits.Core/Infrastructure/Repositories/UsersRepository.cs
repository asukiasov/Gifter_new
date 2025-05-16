using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
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
                                userID.ToSqlParameter(nameof(userID), SqlDbType.Int)
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
                                userEmail.ToSqlParameter(nameof(userEmail), SqlDbType.VarChar),
                                userPassword.ToSqlParameter(nameof(userPassword), SqlDbType.NVarChar)
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
                                userEmail.ToSqlParameter(nameof(userEmail), SqlDbType.NVarChar),
                                userID.ToSqlParameter(nameof(userID), SqlDbType.Int)
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
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                userID.ToSqlParameterOutput(nameof(userID),SqlDbType.Int),
                                userJson.ToSqlParameter(nameof(userJson),SqlDbType.NVarChar),
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
        #endregion Methods
    }    
}