using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Services;
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
            ViewModel.FooterScripts = DBItem.FooterScripts;
            ViewModel.SMTPAddress = DBItem.SMTPAddress;
            ViewModel.SMTPPort = DBItem.SMTPPort;
            ViewModel.SMTPUsername = DBItem.SMTPUsername;
            ViewModel.SMTPPassword = DBItem.SMTPPassword;
            ViewModel.SMTPUseSSL = DBItem.SMTPUseSSL;
            ViewModel.SMTPFrom = DBItem.SMTPFrom;
            ViewModel.UrlTestSmtp = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestSmtp);
            return ViewModel;
        }

        public AjaxResponse TestSmtp(SmtpTestModel SubmitModel)
        {
            var AR = new AjaxResponse();

            var M = new Email(
                SMTPAddress: SubmitModel.SMTPAddress,
                SMTPPort: SubmitModel.SMTPPort,
                SMTPUsername: SubmitModel.SMTPUsername,
                SMTPPassword: SubmitModel.SMTPPassword,
                SMTPUseSSL: SubmitModel.SMTPUseSSL,
                SMTPFrom: SubmitModel.SMTPFrom
            );
            AR.IsSuccess = M.Send(To: SubmitModel.EmailTo, Subject: "TEST", Body: "Test email text");
            AR.Data = M.ErrorMessage;

            return AR;
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
                GoogleMapsIFrame:SubmitModel.GoogleMapsIFrame ?? Constants.NullValueFor.String,
                FooterScripts: SubmitModel.FooterScripts ?? Constants.NullValueFor.String,
                SMTPAddress: SubmitModel.SMTPAddress ?? Constants.NullValueFor.String,
                SMTPPort: SubmitModel.SMTPPort ?? Constants.NullValueFor.Int,
                SMTPUsername: SubmitModel.SMTPUsername ?? Constants.NullValueFor.String,
                SMTPPassword: SubmitModel.SMTPPassword ?? Constants.NullValueFor.String,
                SMTPUseSSL: SubmitModel.SMTPUseSSL,
                SMTPFrom: SubmitModel.SMTPFrom ?? Constants.NullValueFor.String
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
            public string FooterScripts { get; set; }
            public string SMTPAddress { get; set; }
            public int? SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public bool SMTPUseSSL { get; set; }            
            public string SMTPFrom { get; set; }
            public string UrlTestSmtp { get; set; }
            public readonly string TextSuccess = Resources.TextSuccess;
            #endregion
        }

        public class SmtpTestModel
        {
            #region Properties
            public string EmailTo { get; set; }
            public string SMTPAddress { get; set; }
            public int? SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public bool SMTPUseSSL { get; set; }
            public string SMTPFrom { get; set; } 
            #endregion
        }
        #endregion
    }
}
