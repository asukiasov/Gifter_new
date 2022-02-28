using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class UsersDataAccess : DataAccessBase
    {
        #region Contructors
        public UsersDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory) { }
        #endregion

        #region Methods        
        public async Task<User> GetSingleUserByID(int? UserID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleUserByID)}({nameof(UserID)} = {UserID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.UsersGetSingleUserByUserID(UserID);
                    return Result?.DeserializeJsonTo<User>();
                }
            });
        }

        public async Task<User> GetSingleUserByEmailAndPassword(string Email, string Password)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleUserByEmailAndPassword)}({nameof(Email)} = {Email}, {nameof(Password)} = {Password})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Json = await db.UsersGetSingleUserByEmailAndPassword(Email, Password);
                    //var Result = Json.DeserializeJsonTo<User>();
                    var Result = System.Text.Json.JsonSerializer.Deserialize<User>(Json);
                    return Result;
                }
            });
        }

        public async Task<bool> IsUserEmailUniq(string UserEmail, int? UserID = null)
        {
            return await TryToReturn($"{nameof(IsUserEmailUniq)}({nameof(UserEmail)} = {UserEmail}, {nameof(UserID)} = {UserID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.UsersIsEmailUnique(UserEmail, UserID);
                }
            });
        }

        public async Task<List<DBCoreDataContext.UsersListResultItem>> ListUsers()
        {
            return await TryToReturnAsyncTask($"{nameof(ListUsers)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.UsersList().OrderByDescending(Item => Item.UserDateCreated).ToListAsync();
                }
            });
        }

        public async Task<int?> UsersIUD(Enums.DatabaseActions DatabaseAction, int? UserID = null, string UserEmail = null, string UserPassword = null, string UserFirstname = null, string UserLastname = null, int? UserRoleID = null, DateTime? UserBirthdate = null, string UserPhoneNumberMobile = null, string UserPersonalNumber = null, string UserAvatarFilename = null, bool? UserIsActive = null)
        {            
            return await TryToReturnAsyncTask($"{nameof(UsersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(UserID)} = {UserID}, {nameof(UserEmail)} = {UserEmail}, {nameof(UserPassword)} = {UserPassword}, {nameof(UserFirstname)} = {UserFirstname}, {nameof(UserLastname)} = {UserLastname}, {nameof(UserRoleID)} = {UserRoleID}, {nameof(UserBirthdate)} = {UserBirthdate}, {nameof(UserPhoneNumberMobile)} = {UserPhoneNumberMobile}, {nameof(UserPersonalNumber)} = {UserPersonalNumber}, {nameof(UserAvatarFilename)} = {UserAvatarFilename}, {nameof(UserIsActive)} = {UserIsActive})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    UserID = await db.UsersIUD(DatabaseAction, UserID, UserEmail, UserPassword, UserFirstname, UserLastname, UserRoleID, UserBirthdate, UserPhoneNumberMobile, UserPersonalNumber, UserAvatarFilename, UserIsActive);                    
                    return UserID;
                }
            });
        }
        #endregion Methods
    }
    
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
        public DateTime? UserDateCreated { get; set; }
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