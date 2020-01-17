using SixtyThreeBits.Libraries;
using SixtyThreeBits.Core.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SixtyThreeBits.Core.Modules
{
    public class UsersDataAccess : DataAccessBase
    {
        #region Contructors
        public UsersDataAccess(DBCoreDataContext db) : base(db) { } 
        #endregion

        #region Methods
        //    public void DeleteUser(int? UserID)
        //    {
        //        TryExecute($"{nameof(DeleteUser)}({nameof(UserID)} = {UserID})", () =>
        //        {
        //            using (var db = ConnectionFactory.GetDBCoreDataContext())
        //            {
        //                db.UsersDelete(UserID);
        //            }
        //        });
        //    }

        //    public static User GetSingleUserByID(int? UserID)
        //    {
        //        return TryToReturnStatic($"{nameof(GetSingleUserByID)}({nameof(UserID)} = {UserID})", () =>
        //        {
        //            using (var db = ConnectionFactory.GetDBCoreDataContext())
        //            {
        //                return db.UsersGetSingleUserByUserID(UserID).DeserializeTo<User>();
        //            }
        //        });
        //    }        

        public async Task<User> GetSingleUserByEmailAndPassword(string Email, string Password)
        {
            return await TryToReturnAsync($"{nameof(GetSingleUserByEmailAndPassword)}({nameof(Email)} = {Email}, {nameof(Password)} = {Password})", async () =>
            {
                 var Result = await db.UsersGetSingleUserByEmailAndPassword(Email, Password);
                return Result.DeserializeTo<User>();
            });
        }

        //    public static bool IsEmailUniq(string Email,int? UserID = null)
        //    {
        //        return TryToReturnStatic($"{nameof(IsEmailUniq)}({nameof(Email)} = {Email}, {nameof(UserID)} = {UserID})", () =>
        //        {
        //            using (var db = ConnectionFactory.GetDBCoreDataContext())
        //            {
        //                return db.UsersIsEmailUnique(Email,UserID).Value;
        //            }
        //        });
        //    }

        public async Task<List<DB.Tables.Users>> ListUsers()
        {
            return await TryToReturnAsync($"{nameof(ListUsers)}()", async () =>
            {
                return await db.Users.OrderByDescending(Item => Item.CRTime).ToListAsync();
            });
        }

        //    public int? UsersIUD(byte? DatabaseAction, int? UserID = null, string UserEmail = null, string UserPassword = null, string UserFirstname = null, string UserLastname = null, string UserFullname = null, int? UserRoleID = null, DateTime? UserBirthdate = null, string UserMobile = null, string UserPersonalNumber = null, string UserAvatarFilename = null, bool? UserIsActive = null)
        //    {
        //        return TryToReturn($"{nameof(UsersIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(UserID)} = {UserID}, {nameof(UserEmail)} = {UserEmail}, {nameof(UserPassword)} = {UserPassword}, {nameof(UserFirstname)} = {UserFirstname}, {nameof(UserLastname)} = {UserLastname}, {nameof(UserFullname)} = {UserFullname}, {nameof(UserRoleID)} = {UserRoleID}, {nameof(UserBirthdate)} = {UserBirthdate}, {nameof(UserMobile)} = {UserMobile}, {nameof(UserPersonalNumber)} = {UserPersonalNumber}, {nameof(UserAvatarFilename)} = {UserAvatarFilename}, {nameof(UserIsActive)} = {UserIsActive})", () =>
        //        {
        //            using (var db = ConnectionFactory.GetDBCoreDataContext())
        //            {
        //                db.UsersIUD(DatabaseAction, ref UserID, UserEmail, UserPassword, UserFirstname, UserLastname, UserRoleID, UserBirthdate, UserMobile, UserPersonalNumber, UserAvatarFilename, UserIsActive);
        //                return UserID;
        //            }
        //        });
        //    }
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