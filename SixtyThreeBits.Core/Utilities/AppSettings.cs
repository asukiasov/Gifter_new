using Microsoft.Extensions.Configuration;
using System.IO;
using System.Runtime.CompilerServices;

namespace SixtyThreeBits.Core.Utilities
{
    public class AppSettingsCollection
    {
        #region Properties        
        IConfiguration _configuration;

        public readonly ConnectionStringSettings ConnectionStrings;
        public readonly string ContentRootPath;
        public readonly string DownloadFolderPhysicalPath;                        
        public readonly string UploadFolderPhysicalPath;
        public readonly string UploadFolderHttpPath = "/upload/";
        public readonly string WebRootPath;
        #endregion

        #region Constructors
        public AppSettingsCollection(string contentRootPath, string webRootPath, IConfiguration configuration)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            DownloadFolderPhysicalPath = $"{WebRootPath}{Path.DirectorySeparatorChar}download";
            UploadFolderPhysicalPath = $"{WebRootPath}{Path.DirectorySeparatorChar}upload";

            _configuration = configuration;
            ConnectionStrings = new ConnectionStringSettings(configuration);
        }
        #endregion

        #region Private Methods
        string GetConfigValue([CallerMemberName] string key = "")
        {
            return _configuration[key];
        }

        #region Nested Classes
        public class ConnectionStringSettings
        {
            #region Properties
            IConfiguration _configuration { get; set; }
            public string DbConnectionString => GetDBConnectionString();            
            #endregion

            #region Constructors
            public ConnectionStringSettings(IConfiguration configuration)
            {
                _configuration = configuration;
            }
            #endregion

            #region Methods
            string GetDBConnectionString([CallerMemberName] string key = "")
            {
                return _configuration.GetConnectionString(key);
            }
            #endregion
        }
        #endregion

        #endregion
    }
}
