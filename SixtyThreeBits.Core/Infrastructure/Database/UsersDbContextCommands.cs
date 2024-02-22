using SixtyThreeBits.Core.Utilities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task<int?> UsersIUD(Enums.DatabaseActions databaseAction, int? userID, int? roleID, string userEmail, string userPassword, string userFirstname, string userLastname, DateTime? userBirthdate, string userPhoneNumberMobile, string userPersonalNumber, string userAvatarFilename, bool? userIsActive)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(UsersIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    roleID.ToSqlParameter(nameof(roleID),SqlDbType.Int),
                    userID.ToSqlParameter(nameof(userID),SqlDbType.Int,true),
                    userEmail.ToSqlParameter(nameof(userEmail),SqlDbType.VarChar),
                    userPassword.ToSqlParameter(nameof(userPassword),SqlDbType.NVarChar),
                    userFirstname.ToSqlParameter(nameof(userFirstname),SqlDbType.NVarChar),
                    userLastname.ToSqlParameter(nameof(userLastname),SqlDbType.NVarChar),
                    userBirthdate.ToSqlParameter(nameof(userBirthdate),SqlDbType.Date),
                    userPhoneNumberMobile.ToSqlParameter(nameof(userPhoneNumberMobile),SqlDbType.VarChar),
                    userPersonalNumber.ToSqlParameter(nameof(userPersonalNumber),SqlDbType.VarChar),
                    userAvatarFilename.ToSqlParameter(nameof(userAvatarFilename),SqlDbType.NVarChar),
                    userIsActive.ToSqlParameter(nameof(userIsActive),SqlDbType.Bit)
                ]
            );

            await sqb.ExecuteCommand();
            userID = sqb.GetNextOutputParameterValue<int?>();
            return userID;
        }
        #endregion
    }
}