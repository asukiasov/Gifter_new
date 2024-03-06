using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region UsersGetSingleByID        
        public async Task<string> UsersGetSingleByID(int? userID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(UsersGetSingleByID),
                sqlParameters:
                [
                    userID.ToSqlParameter(nameof(userID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region UsersGetSingleByEmailAndPassword        
        public async Task<string> UsersGetSingleByEmailAndPassword(string userEmail, string userPassword)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(UsersGetSingleByEmailAndPassword),
                sqlParameters:
                [
                    userEmail.ToSqlParameter(nameof(userEmail), SqlDbType.VarChar),
                    userPassword.ToSqlParameter(nameof(userPassword), SqlDbType.NVarChar)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region UsersList
        public record UsersListEntity
        (
            int? UserID,
            int? RoleID,
            string UserEmail,
            string UserPassword,
            string UserFirstname,
            string UserLastname,
            string UserFullname,
            DateTime? UserBirthdate,
            string UserPhoneNumberMobile,
            string UserPersonalNumber,
            string UserAvatarFilename,
            bool UserIsActive,
            DateTime? UserDateCreated
        );
        public IQueryable<UsersListEntity> UsersList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(UsersList)
            );
            var result = sqb.ExecuteQuery<UsersListEntity>();
            return result;
        }
        #endregion

        #region UsersIsEmailUnique        
        public async Task<bool> UsersIsEmailUnique(string userEmail, int? userID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(UsersIsEmailUnique),
                sqlParameters:
                [
                    userEmail.ToSqlParameter(nameof(userEmail), SqlDbType.NVarChar),
                    userID.ToSqlParameter(nameof(userID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<bool>();
            return result;
        }
        #endregion
    }
}