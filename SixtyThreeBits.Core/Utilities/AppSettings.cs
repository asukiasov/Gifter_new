using Microsoft.Extensions.Configuration;
using System.IO;
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
        public AppSettingsCollection(string contentRootPath, string webRootPath, IConfiguration configuration)
        {
            _configuration = configuration;
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;

            DownloadFolderPhysicalPath = $"{WebRootPath}{Path.DirectorySeparatorChar}{_downloadFolderName}";
            UploadFolderPhysicalPath = GetConfigValue(nameof(UploadFolderPhysicalPath));
            if (string.IsNullOrWhiteSpace(UploadFolderPhysicalPath))
            {
                UploadFolderPhysicalPath = $"{WebRootPath}{Path.DirectorySeparatorChar}{_uploadFolderName}";
            }
            if (string.IsNullOrWhiteSpace(UploadFolderPhysicalPath))
            {
                throw new System.Exception($"UploadFolderPhysicalPath: \"{UploadFolderPhysicalPath}\" is not found");
            }
            
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
