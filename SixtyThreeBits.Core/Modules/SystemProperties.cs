using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Libraries;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class SystemPropertiesAccess : DataAccessBase
    {
        #region Contructors
        public SystemPropertiesAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {
            
        }
        #endregion

        #region Methods
        public async Task<SystemProperties> GetSystemProperties()
        {
            var Result = await TryToReturnAsyncTask($"{nameof(GetSystemProperties)}()", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var DBResult = await db.SystemPropertiesGet();
                    return DBResult.Value?.DeserializeJsonTo<SystemProperties>();
                }
            });
            return Result ?? new SystemProperties();
        }

        public async Task UpdateSystemProperties(SystemProperties SystemProperties)
        {
            var SystemPropertiesJson = SystemProperties.ToJson();
            await TryExecuteAsyncTask($"{nameof(UpdateSystemProperties)}({nameof(SystemProperties)} = {SystemPropertiesJson})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.SystemPropertiesUpdate(SystemPropertiesJson);
                }
            });
        }
        #endregion
    }

    public class SystemProperties
    {
        #region Properties
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string YoutubeUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GoogleMapsIFrame { get; set; }
        public string SMTPAddress { get; set; }
        public int? SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }        
        public string SMTPFrom { get; set; }
        public string ScriptsHeader { get; set; }
        public string ScriptsBodyStart { get; set; }
        public string ScriptsBodyEnd { get; set; }

        public bool? IsMailgunEnabled { get; set; }
        public string MailgunBaseUrl { get; set; }
        public string MailgunApiKey { get; set; }
        public string MailgunDomain { get; set; }
        public string MailgunFrom { get; set; }
        public string MailgunWebhookWebhookSigningKey { get; set; }
        #endregion
    }
}
