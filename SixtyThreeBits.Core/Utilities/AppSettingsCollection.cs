using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SixtyThreeBits.Core.Utilities
{
    public class AppSettingsCollection
    {
        #region Properties        
        IConfiguration _configuration;
        const string _uploadFolderName = "upload";
        const string _downloadFolderName = "download";

        public readonly ConnectionStringSettings ConnectionStrings;
        public readonly string ContentRootPath;
        public readonly string DownloadFolderPhysicalPath;                        
        public readonly string UploadFolderPhysicalPath;
        public readonly string UploadFolderHttpPath = $"/{_uploadFolderName}/";
        public readonly string WebRootPath;
        #endregion

        #region Constructors
        public AppSettingsCollection(string contentRootPath, string webRootPath, IConfiguration configuration, bool isDevelopmentEnvironment)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            DownloadFolderPhysicalPath = $"{WebRootPath}{Path.DirectorySeparatorChar}{_downloadFolderName}";

            if (isDevelopmentEnvironment)
            {
                UploadFolderPhysicalPath = Path.GetFullPath(Path.Combine(webRootPath, _uploadFolderName));
            }
            else
            {
                string parentDir = Path.GetDirectoryName(contentRootPath);
                UploadFolderPhysicalPath = Path.Combine(parentDir ?? contentRootPath, _uploadFolderName);
            }

            if (!Directory.Exists(UploadFolderPhysicalPath))
            {
                Directory.CreateDirectory(UploadFolderPhysicalPath);
            }

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
