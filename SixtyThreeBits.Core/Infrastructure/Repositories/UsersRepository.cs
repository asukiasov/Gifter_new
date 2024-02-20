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
                cfg.CreateMap<DBQueriesDataContext.UsersListEntity, UsersListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods                
        public async Task<UserDTO> UsersGetSingleUserByUserID(int? userID)
        {
            return await TryToReturnAsyncTask($"{nameof(UsersGetSingleUserByUserID)}({nameof(userID)} = {userID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var result = await db.UsersGetSingleUserByUserID(userID);
                    return result?.DeserializeJsonTo<UserDTO>();
                }
            });
        }

        public async Task<UserDTO> UsersGetSingleUserByEmailAndPassword(string userEmail, string userPassword)
        {
            return await TryToReturnAsyncTask($"{nameof(UsersGetSingleUserByEmailAndPassword)}({nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var result = await db.UsersGetSingleUserByEmailAndPassword(userEmail, userPassword);
                    return result.DeserializeJsonTo<UserDTO>();
                }
            });
        }

        public async Task<bool> UsersIsEmailUnique(string userEmail, int? userID = null)
        {
            return await TryToReturn($"{nameof(UsersIsEmailUnique)}({nameof(userEmail)} = {userEmail}, {nameof(userID)} = {userID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return await db.UsersIsEmailUnique(userEmail, userID);
                }
            });
        }

        public async Task<int?> UsersIUD(Enums.DatabaseActions databaseAction, int? roleID = null, int? userID = null, string userEmail = null, string userPassword = null, string userFirstname = null, string userLastname = null, DateTime? userBirthdate = null, string userPhoneNumberMobile = null, string userPersonalNumber = null, string userAvatarFilename = null, bool? userIsActive = null)
        {
            return await TryToReturnAsyncTask($"{nameof(UsersIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(roleID)} = {roleID}, {nameof(userID)} = {userID}, {nameof(userEmail)} = {userEmail}, {nameof(userPassword)} = {userPassword}, {nameof(userFirstname)} = {userFirstname}, {nameof(userLastname)} = {userLastname}, {nameof(userBirthdate)} = {userBirthdate}, {nameof(userPhoneNumberMobile)} = {userPhoneNumberMobile}, {nameof(userPersonalNumber)} = {userPersonalNumber}, {nameof(userAvatarFilename)} = {userAvatarFilename}, {nameof(userIsActive)} = {userIsActive})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    userID = await db.UsersIUD(databaseAction, userID, roleID, userEmail, userPassword, userFirstname, userLastname, userBirthdate, userPhoneNumberMobile, userPersonalNumber, userAvatarFilename, userIsActive);
                    return userID;
                }
            });
        }

        public async Task<List<UsersListDTO>> UsersList()
        {
            return await TryToReturnAsyncTask($"{nameof(UsersList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.UsersList().OrderByDescending(item => item.UserDateCreated).ToListAsync())?.Select(item => _mapper.Map<UsersListDTO>(item)).ToList();
                }
            });
        }
        #endregion Methods
    }    
}