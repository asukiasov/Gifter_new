using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class UsersRepository : RepositoryBase
    {
        #region Contructors
        public UsersRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DbContextQueries.UsersListEntity, UsersListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods                
        public async Task<UserDTO> UsersGetSingleUserByUserID(int? userID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleUserByUserID)}({nameof(userID)} = {userID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.UsersGetSingleUserByUserID(userID);
                        var result = resultJson?.DeserializeJsonTo<UserDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<UserDTO> UsersGetSingleUserByEmailAndPassword(string userEmail, string userPassword)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersGetSingleUserByEmailAndPassword)}({nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.UsersGetSingleUserByEmailAndPassword(userEmail, userPassword);
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = await db.UsersIsEmailUnique(userEmail, userID);
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> UsersIUD(Enums.DatabaseActions databaseAction, int? roleID = null, int? userID = null, string userEmail = null, string userPassword = null, string userFirstname = null, string userLastname = null, DateTime? userBirthdate = null, string userPhoneNumberMobile = null, string userPersonalNumber = null, string userAvatarFilename = null, bool? userIsActive = null)
        {
            userID = await TryToReturnAsyncTask(
                logString: $"{nameof(UsersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(roleID)} = {roleID}, {nameof(userID)} = {userID}, {nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword}, {nameof(userFirstname)} = {userFirstname}, {nameof(userLastname)} = {userLastname}, {nameof(userBirthdate)} = {userBirthdate}, {nameof(userPhoneNumberMobile)} = {userPhoneNumberMobile}, {nameof(userPersonalNumber)} = {userPersonalNumber}, {nameof(userAvatarFilename)} = {userAvatarFilename}, {nameof(userIsActive)} = {userIsActive})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        userID = await db.UsersIUD(databaseAction, userID, roleID, userEmail, userPassword, userFirstname, userLastname, userBirthdate, userPhoneNumberMobile, userPersonalNumber, userAvatarFilename, userIsActive);
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
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (await db.UsersList().OrderByDescending(item => item.UserDateCreated).ToListAsync())?.Select(item => _mapper.Map<UsersListDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }
        #endregion Methods
    }    
}