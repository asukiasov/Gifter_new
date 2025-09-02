using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Core.Infrastructure.Repositories.DTO
{
    public record UserDTO
    {
        #region Properties
        public int? UserID { get; init; }
        public string UserFullname { get; init; }
        public string UserFirstname { get; init; }
        public string UserLastname { get; init; }
        public DateTime? UserBirthdate { get; init; }
        public string UserEmail { get; init; }
        public string UserPassword { get; init; }
        public string UserPhoneNumberMobile { get; init; }
        public bool UserIsActive { get; init; } 
        public string UserAvatarFilename { get; init; }
        public DateTime? UserDateCreated { get; init; }
        public int? RoleID { get; init; }
        public int? RoleCode { get; init; }
        public string RoleName { get; init; }
        public List<PermissionDTO> Permissions { get; init; }
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
            if (string.IsNullOrWhiteSpace(permission))
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
}
