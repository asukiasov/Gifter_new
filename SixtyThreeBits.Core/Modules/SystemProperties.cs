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
            return await TryToReturnAsyncTask($"{nameof(GetSystemProperties)}()", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var DBResult = await db.SystemPropertiesGet();
                    var Result = DBResult.Value?.DeserializeTo<SystemProperties>();
                    return Result ?? new SystemProperties();
                }
            });
        }

        public async Task UpdateSystemProperties(string ContactEmail, string ContactPhone, string ContactAddress, string FacebookUrl, string InstagramUrl, string TwitterUrl, string YoutubeUrl, string LinkedInUrl, string GoogleMapsIFrame)
        {
            await TryExecuteAsyncTask($"{nameof(UpdateSystemProperties)}({nameof(ContactEmail)} = {ContactEmail}, {nameof(ContactPhone)} = {ContactPhone}, {nameof(ContactAddress)} = {ContactAddress}, {nameof(FacebookUrl)} = {FacebookUrl}, {nameof(InstagramUrl)} = {InstagramUrl}, {nameof(TwitterUrl)} = {TwitterUrl}, {nameof(YoutubeUrl)} = {YoutubeUrl}, {nameof(LinkedInUrl)} = {LinkedInUrl}, {nameof(GoogleMapsIFrame)} = {GoogleMapsIFrame})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.SystemPropertiesUpdate(ContactEmail, ContactPhone, ContactAddress, FacebookUrl, InstagramUrl, TwitterUrl, YoutubeUrl, LinkedInUrl, GoogleMapsIFrame);
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
        #endregion
    }
}
