using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Linq;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PageRootModel : ModelBase
    {
        #region Methods
        public string GetRedirectUrl()
        {
            var redirectUrl = default(string);
            var permissionIDParent = User.Permissions.FindLast(Item => Item.PermissionCodeName == ControllerActionRouteNames.Admin.PagesManagementController.RedirectToChild)?.PermissionID;
            if (permissionIDParent.HasValue)
            {
                var firstPermission = User.Permissions.FirstOrDefault(item => item.PermissionParentID == permissionIDParent);
                redirectUrl = firstPermission.PermissionPagePath;
            }
            return redirectUrl;
        } 
        #endregion
    }    
}