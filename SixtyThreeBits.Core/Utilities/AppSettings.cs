using Microsoft.Extensions.Configuration;
using SixtyThreeBits.Libraries;
using System.Runtime.CompilerServices;

namespace SixtyThreeBits.Core.Utilities
{
    public class AppSettingsCollection
    {
        #region Properties        
        IConfiguration Configuration { get; set; }
        public DBConnectionStringsModel DBConnectionStrings { get; set; }

        public readonly string OgImageDefaultHttpPath = "/img/og_image_default.jpg";

        public string UploadFolderPhysicalPath => GetConfigValue();
        public readonly string UploadFolderVirtualName = "upload";
        public string UploadFolderVirtualPath => $"/{UploadFolderVirtualName}/";
        public bool IsDevelopment { get; set; }

        #endregion

        #region Constructors
        public AppSettingsCollection(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
            DBConnectionStrings = new DBConnectionStringsModel(Configuration);
        } 
        #endregion

        #region Private Methods
        string GetConfigValue([CallerMemberName] string Key = "")
        {
            return Configuration[Key];
        }        

        #region Sub Classes
        public class DBConnectionStringsModel
        {
            #region Properties
            IConfiguration Configuration { get; set; }
            public string DBConnectionString  => GetDBConnectionString();
            #endregion

            #region Constructors
            public DBConnectionStringsModel(IConfiguration Configuration)
            {
                this.Configuration = Configuration;
            }
            #endregion

            #region Methods
            string GetDBConnectionString([CallerMemberName] string Key = "")
            {
                return Configuration.GetConnectionString(Key);
            } 
            #endregion
        }
        #endregion

        #endregion
    } 
}
