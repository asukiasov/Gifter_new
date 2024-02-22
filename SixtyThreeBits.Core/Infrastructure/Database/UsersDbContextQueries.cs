using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region UsersGetSingleUserByUserID        
        public async Task<string> UsersGetSingleUserByUserID(int? userID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(UsersGetSingleUserByUserID),
                itemType: typeof(ScalarFunctionResultEntity<string>),
                sqlParameters:
                [
                    userID.ToSqlParameter(nameof(userID), SqlDbType.Int)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region UsersGetSingleUserByEmailAndPassword        
        public async Task<string> UsersGetSingleUserByEmailAndPassword(string userEmail, string userPassword)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(UsersGetSingleUserByEmailAndPassword),
                itemType: typeof(ScalarFunctionResultEntity<string>),
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
                databaseObjectType: DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                databaseObjectName: nameof(UsersList),
                itemType: typeof(UsersListEntity)
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
                databaseObjectType: DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                databaseObjectName: nameof(UsersIsEmailUnique),
                itemType: typeof(ScalarFunctionResultEntity<bool>),
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