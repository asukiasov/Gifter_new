using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables.Core;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class SystemPropertiesModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            var DBItem = await DataAccessFactory.SystemProperties.GetSystemProperties();
            ViewModel.ContactEmail = DBItem.ContactEmail;
            ViewModel.ContactPhone = DBItem.ContactPhone;            
            ViewModel.ContactAddress = DBItem.ContactAddress;
            ViewModel.FacebookUrl = DBItem.FacebookUrl;
            ViewModel.TwitterUrl = DBItem.TwitterUrl;
            ViewModel.InstagramUrl = DBItem.InstagramUrl;
            ViewModel.YoutubeUrl = DBItem.YoutubeUrl;            
            ViewModel.LinkedInUrl = DBItem.LinkedInUrl;
            ViewModel.GoogleMapsIFrame = DBItem.GoogleMapsIFrame;
            return ViewModel;
        }

        public async Task<PageViewModel> UpdateSystemProperties(PageViewModel SubmitModel)
        {
            if (SubmitModel.GoogleMapsIFrame != null && SubmitModel.GoogleMapsIFrame.Contains("<iframe") && !SubmitModel.GoogleMapsIFrame.Contains("width=\"100%\""))
            {
                SubmitModel.GoogleMapsIFrame = Regex.Replace(SubmitModel.GoogleMapsIFrame, "width=\"\\d+\"", "width=\"100%\"").Trim();
            }

            await DataAccessFactory.SystemProperties.UpdateSystemProperties(
                ContactEmail: SubmitModel.ContactEmail ?? Constants.NullValueFor.String,
                ContactPhone: SubmitModel.ContactPhone ?? Constants.NullValueFor.String,
                ContactAddress: SubmitModel.ContactAddress ?? Constants.NullValueFor.String,
                FacebookUrl: SubmitModel.FacebookUrl ?? Constants.NullValueFor.String,
                TwitterUrl: SubmitModel.TwitterUrl ?? Constants.NullValueFor.String,
                InstagramUrl: SubmitModel.InstagramUrl ?? Constants.NullValueFor.String,
                YoutubeUrl: SubmitModel.YoutubeUrl ?? Constants.NullValueFor.String,
                LinkedInUrl: SubmitModel.LinkedInUrl ?? Constants.NullValueFor.String,
                GoogleMapsIFrame:SubmitModel.GoogleMapsIFrame ?? Constants.NullValueFor.String
            );
            var ViewModel = SubmitModel;
            ViewModel.IsSaved = !DataAccessFactory.SystemProperties.IsError;
            return ViewModel;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel : FormViewModelBase
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
        #endregion
    }
}
