using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
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
                    var Result = DBResult?.DeserializeTo<SystemProperties>();
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
                    var DBItem = await db.SystemProperties.FirstOrDefaultAsync();
                    if (DBItem != null)
                    {
                        DBItem.ContactEmail = ContactEmail == Constants.NullValueFor.String ? null : ContactEmail ?? DBItem.ContactEmail;
                        DBItem.ContactPhone = ContactPhone == Constants.NullValueFor.String ? null : ContactPhone ?? DBItem.ContactPhone;
                        DBItem.ContactAddress = ContactPhone == Constants.NullValueFor.String ? null : ContactAddress ?? DBItem.ContactAddress;
                        DBItem.FacebookUrl = FacebookUrl == Constants.NullValueFor.String ? null : FacebookUrl ?? DBItem.FacebookUrl;
                        DBItem.InstagramUrl = InstagramUrl == Constants.NullValueFor.String ? null : InstagramUrl ?? DBItem.InstagramUrl;
                        DBItem.TwitterUrl = TwitterUrl == Constants.NullValueFor.String ? null : TwitterUrl ?? DBItem.TwitterUrl;
                        DBItem.YoutubeUrl = YoutubeUrl == Constants.NullValueFor.String ? null : YoutubeUrl ?? DBItem.YoutubeUrl;
                        DBItem.LinkedInUrl = LinkedInUrl == Constants.NullValueFor.String ? null : LinkedInUrl ?? DBItem.LinkedInUrl;                        
                        DBItem.GoogleMapsIFrame = GoogleMapsIFrame == Constants.NullValueFor.String ? null : GoogleMapsIFrame ?? DBItem.GoogleMapsIFrame;
                        db.SystemProperties.Update(DBItem);
                        await db.SaveChangesAsync();
                    }
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
