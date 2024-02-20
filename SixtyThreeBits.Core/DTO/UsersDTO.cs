using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Core.DTO
{
    public class UserDTO
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
        public bool UserIsSuperAdmin { get; set; }
        public string UserAvatarFilename { get; set; }
        public DateTime? UserDateCreated { get; set; }
        public int? RoleID { get; set; }
        public int? RoleCode { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDTO> Permissions { get; set; }
        #endregion Properties

        #region Methods
        public List<PermissionDTO> GetChildPermissionsByParent(string parentPermission)
        {
            List<PermissionDTO> childPermissions = null;
            var parentPermissionItem = GetPermission(parentPermission);

            if (parentPermissionItem != null)
            {
                childPermissions = Permissions.Where(item => item.PermissionParentID == parentPermissionItem.PermissionID).ToList();
            }

            return childPermissions;
        }

        public PermissionDTO GetPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
            {
                return null;
            }
            else
            {
                return Permissions?.Where(item => item.PermissionCodeName == permission || item.PermissionCode == permission || !string.IsNullOrWhiteSpace(item.PermissionPagePath) && Regex.IsMatch(permission, $"^{item.PermissionPagePath}*$")).LastOrDefault();
            }
        }

        public string GetPermissionNameByPagePath(string pagePath)
        {
            return GetPermission(pagePath)?.PermissionCaption;
        }

        public bool HasPermission(string permission)
        {
            if (UserIsSuperAdmin || string.IsNullOrWhiteSpace(permission))
            {
                return true;
            }
            else
            {
                var p = GetPermission(permission);
                return p != null;
            }
        }
        #endregion Methods        
    }

    public record UsersListDTO
    {
        #region Properties
        public int? UserID { get; init; }
        public int? RoleID { get; init; }
        public string UserEmail { get; init; }
        public string UserPassword { get; init; }
        public string UserFirstname { get; init; }
        public string UserLastname { get; init; }
        public string UserFullname { get; init; }
        public DateTime? UserBirthdate { get; init; }
        public string UserPhoneNumberMobile { get; init; }
        public string UserPersonalNumber { get; init; }
        public string UserAvatarFilename { get; init; }
        public bool UserIsActive { get; init; }
        public DateTime? UserDateCreated { get; init; }
        #endregion
    }
}
