using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Web.Domain.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class UrlFactory63
    {
        #region Properties     
        string _websiteDomain;
        string _protocol;
        IUrlHelper _url;
        #endregion

        #region Constructor
        public UrlFactory63(string websiteDomain, string protocol, IUrlHelper url)
        {
            _websiteDomain = websiteDomain;
            _protocol = protocol;
            _url = url;
        }
        #endregion

        #region Methods
        public string CreateFileManagerAdminUrl(FileManagerModules? fileManagerModule = null, string onSelectedFilesChooseClientCallback = null, string allowedExtensionsCsv = null, bool allowSelectMultiple = false)
        {
            var urlFileManager = _url.RouteUrl(routeName: ControllerActionRouteNames.Admin.FileManagerController.FileManager, values: null, protocol: _protocol);

            var queryStringValues = new List<string>(3);
            if(fileManagerModule != null)
            {
                queryStringValues.Add($"{WebConstants.QueryStringKeys.FileManagerModule}={fileManagerModule}");
            }
            if (!string.IsNullOrWhiteSpace(onSelectedFilesChooseClientCallback))
            {
                queryStringValues.Add($"{WebConstants.QueryStringKeys.FileManagerOnSelectedFilesChooseClientCallback}={onSelectedFilesChooseClientCallback}");
            }
            if (!string.IsNullOrWhiteSpace(allowedExtensionsCsv))
            {
                queryStringValues.Add($"{WebConstants.QueryStringKeys.FileManagerAllowedExtensions}={allowedExtensionsCsv}");
            }
            if (allowSelectMultiple)
            {
                queryStringValues.Add($"{WebConstants.QueryStringKeys.FileManagerAllowChooseMultiple}=true");
            }
            var queryString = queryStringValues.Any() ? $"?{string.Join("&", queryStringValues)}" : null;

            var result = $"{urlFileManager}{queryString}";

            return result;
        }
        #endregion
    }
}
