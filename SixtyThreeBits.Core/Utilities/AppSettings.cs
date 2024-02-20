using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace SixtyThreeBits.Core.Utilities
{
    public class AppSettingsCollection
    {
        #region Properties        
        IConfiguration _configuration;

        public readonly string WebRootPath;

        public readonly ConnectionStringSettings ConnectionStrings;

        public readonly string OgImageDefaultHttpPath = "/img/og_image_default.jpg";

        public string DownloadFolderPhysicalPath => GetConfigValue();
        public string UploadFolderPhysicalPath => GetConfigValue();
        public string UploadFolderHttpPath => GetConfigValue();
        public bool IsDevelopment => GetConfigValue() == "true";
        #endregion

        #region Constructors
        public AppSettingsCollection(string webRootPath, IConfiguration configuration)
        {
            WebRootPath = webRootPath;
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
            public string CommandsConnectionString => GetDBConnectionString();
            public string QueriesConnectionString => GetDBConnectionString();
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
