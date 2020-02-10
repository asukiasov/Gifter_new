using SixtyThreeBits.Libraries;
using SixtyThreeBits.Core.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Utilities;

namespace SixtyThreeBits.Core.Modules
{
    public class UsersDataAccess : DataAccessBase
    {
        #region Contructors
        public UsersDataAccess(DBCoreDataContext db) : base(db) { }
        #endregion

        #region Methods        
        public async Task<User> GetSingleUserByID(int? UserID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleUserByID)}({nameof(UserID)} = {UserID})", async () =>
            {
                var Result = await db.UsersGetSingleUserByUserID(UserID);
                return Result.DeserializeTo<User>();
            });
        }

        public async Task<User> GetSingleUserByEmailAndPassword(string Email, string Password)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleUserByEmailAndPassword)}({nameof(Email)} = {Email}, {nameof(Password)} = {Password})", async () =>
            {
                 var Result = await db.UsersGetSingleUserByEmailAndPassword(Email, Password);
                return Result.DeserializeTo<User>();
            });
        }

        public async Task<bool> IsUserEmailUniq(string UserEmail, int? UserID = null)
        {
            return await TryToReturn($"{nameof(IsUserEmailUniq)}({nameof(UserEmail)} = {UserEmail}, {nameof(UserID)} = {UserID})", async () =>
            {
                return await db.UsersIsEmailUnique(UserEmail, UserID);                
            });
        }

        public async Task<List<DB.Tables.Users>> ListUsers()
        {
            return await TryToReturnAsyncTask($"{nameof(ListUsers)}()", async () =>
            {
                return await db.Users.OrderByDescending(Item => Item.CRTime).ToListAsync();
            });
        }

        public async Task<int?> UsersIUD(byte DatabaseAction, int? UserID = null, string UserEmail = null, string UserPassword = null, string UserFirstname = null, string UserLastname = null, int? UserRoleID = null, DateTime? UserBirthdate = null, string UserPhoneNumberMobile = null, string UserPersonalNumber = null, string UserAvatarFilename = null, bool? UserIsActive = null)
        {            
            return await TryToReturnAsyncTask($"{nameof(UsersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(UserID)} = {UserID}, {nameof(UserEmail)} = {UserEmail}, {nameof(UserPassword)} = {UserPassword}, {nameof(UserFirstname)} = {UserFirstname}, {nameof(UserLastname)} = {UserLastname}, {nameof(UserRoleID)} = {UserRoleID}, {nameof(UserBirthdate)} = {UserBirthdate}, {nameof(UserPhoneNumberMobile)} = {UserPhoneNumberMobile}, {nameof(UserPersonalNumber)} = {UserPersonalNumber}, {nameof(UserAvatarFilename)} = {UserAvatarFilename}, {nameof(UserIsActive)} = {UserIsActive})", async () =>
            {
                //await db.UsersIUD(DatabaseAction, UserID, UserEmail, UserPassword, UserFirstname, UserLastname, UserRoleID, UserBirthdate, UserPhoneNumberMobile, UserPersonalNumber, UserAvatarFilename, UserIsActive);

                if (DatabaseAction == Enums.DatabaseActions.CREATE)
                {                    
                    UserID = await db.UsersIUD(DatabaseAction, UserID, UserEmail, UserPassword, UserFirstname, UserLastname, UserRoleID, UserBirthdate, UserPhoneNumberMobile, UserPersonalNumber, UserAvatarFilename, UserIsActive);
                    var User = new DB.Tables.Users
                    {
                        UserEmail = UserEmail,
                        UserPassword = UserPassword.MD5(),
                        UserFirstname = UserFirstname,
                        UserLastname = UserLastname,
                        UserRoleID = UserRoleID,
                        UserBirthdate = UserBirthdate,
                        UserPhoneNumberMobile = UserPhoneNumberMobile,
                        UserPersonalNumber = UserPersonalNumber,
                        UserAvatarFilename = UserAvatarFilename,
                        UserIsActive = UserIsActive ?? false
                    };
                    await db.Users.AddAsync(User);
                    await db.SaveChangesAsync();
                    UserID = User.UserID;
                }
                else if (DatabaseAction == Enums.DatabaseActions.UPDATE)
                {
                    var User = await db.Users.FirstOrDefaultAsync(Item => Item.UserID == UserID);
                    if (User != null)
                    {
                        User.UserEmail = UserEmail ?? User.UserEmail;
                        User.UserPassword = UserPassword.MD5() ?? User.UserPassword;
                        User.UserFirstname = UserFirstname ?? User.UserFirstname;
                        User.UserLastname = UserLastname ?? User.UserLastname;
                        User.UserRoleID = UserRoleID == Constants.NullValueFor.Int ? null : UserRoleID ?? User.UserRoleID;
                        User.UserBirthdate = UserBirthdate == Constants.NullValueFor.Date ? null : UserBirthdate ?? User.UserBirthdate;
                        User.UserPhoneNumberMobile = UserPhoneNumberMobile == Constants.NullValueFor.String ? null : UserPhoneNumberMobile ?? User.UserPhoneNumberMobile;
                        User.UserPersonalNumber = UserPersonalNumber == Constants.NullValueFor.String ? null : UserPersonalNumber ?? User.UserPersonalNumber;
                        User.UserAvatarFilename = UserAvatarFilename == Constants.NullValueFor.String ? null : UserAvatarFilename ?? User.UserAvatarFilename;
                        User.UserIsActive = UserIsActive ?? User.UserIsActive;
                        db.Users.Update(User);                        
                        await db.SaveChangesAsync();
                    }
                }
                else if (DatabaseAction == Enums.DatabaseActions.DELETE)
                {
                    var User = await db.Users.FirstOrDefaultAsync(Item => Item.UserID == UserID);
                    if (User != null)
                    {
                        db.Users.Remove(User);
                        await db.SaveChangesAsync();
                    }
                }

                return UserID;
            });
        }
        #endregion Methods
    }

    [Serializable]
    public class User
    {
        #region Properties
        public int? UserID { get; set; }
        public string UserFullname { get; set; }
        public string UserFirstname { get; set; }
        public string UserLastname { get; set; }
        public DateTime? UserBirthdate { get; set; }        
        public string UserEmail { get; set; }        
        public string UserPassword { get; set; }
        public string UserPhoneNumberMobile { get; set; }
        public bool UserIsActive { get; set; }
        public bool UserIsAdmin { get; set; }
        public int? UserRoleID { get; set; }        
        public string UserAvatarFilename { get; set; }        
        public DateTime? CRTime { get; set; }
        public List<Permission> Permissions { get; set; }        
        #endregion Properties

        #region Methods
        public List<Permission> GetChildPermissionsByParent(string ParentPermission)
        {
            List<Permission> ChildPermissions = null;
            var ParentPermissionItem = GetPermission(ParentPermission);

            if(ParentPermissionItem!=null)
            {
                ChildPermissions = Permissions.Where(Item => Item.PermissionParentID == ParentPermissionItem.PermissionID).ToList();
            }                

            return ChildPermissions;
        }

        public Permission GetPermission(string Permission)
        {
            if (string.IsNullOrWhiteSpace(Permission))
            {
                return null;
            }
            else
            {
                return Permissions?.Where(P => P.PermissionCodeName == Permission || P.PermissionCode == Permission || (!string.IsNullOrWhiteSpace(P.PermissionPagePath) && Regex.IsMatch(Permission, $"^{P.PermissionPagePath}*$"))).LastOrDefault();
            }
        }      

        public string GetPermissionNameByPagePath(string PagePath)
        {
            return GetPermission(PagePath)?.PermissionCaption;
        }

        public bool HasPermission(string Permission)
        {
            if (UserIsAdmin || string.IsNullOrWhiteSpace(Permission))
            {
                return true;
            }
            else
            {
                var P = GetPermission(Permission);
                return P != null;
            }
        }
        #endregion Methods        
    }    
}