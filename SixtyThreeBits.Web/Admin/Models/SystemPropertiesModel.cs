using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
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
            ViewModel.SMTPAddress = DBItem.SMTPAddress;
            ViewModel.SMTPPort = DBItem.SMTPPort;
            ViewModel.SMTPUsername = DBItem.SMTPUsername;
            ViewModel.SMTPPassword = DBItem.SMTPPassword;
            ViewModel.SMTPUseSSL = DBItem.SMTPUseSSL;
            ViewModel.SMTPFrom = DBItem.SMTPFrom;
            ViewModel.ScriptsHeader = DBItem.ScriptsHeader;
            ViewModel.ScriptsBodyStart = DBItem.ScriptsBodyStart;
            ViewModel.ScriptsBodyEnd = DBItem.ScriptsBodyEnd;
            ViewModel.UrlTestSmtp = Url.RouteUrl(ControllerActionRouteNames.Admin.SystemProperties.TestSmtp);
            return ViewModel;
        }

        public async Task<AjaxResponse> TestSmtp(SmtpTestModel SubmitModel)
        {
            var AR = new AjaxResponse();

            var M = new SMTP(
                SMTPAddress: SubmitModel.SMTPAddress,
                SMTPPort: SubmitModel.SMTPPort,
                SMTPUsername: SubmitModel.SMTPUsername,
                SMTPPassword: SubmitModel.SMTPPassword,
                SMTPUseSSL: SubmitModel.SMTPUseSSL,
                SMTPFromName: SubmitModel.SMTPFrom
            );
            var Result = await M.Send(To: SubmitModel.EmailTo, Subject: "TEST", Body: "Test email text");
            AR.IsSuccess = Result.IsSent;
            AR.Data = M.ErrorMessage;

            return AR;
        }

        public async Task<PageViewModel> UpdateSystemProperties(PageViewModel SubmitModel)
        {
            if (SubmitModel.GoogleMapsIFrame != null && SubmitModel.GoogleMapsIFrame.Contains("<iframe") && !SubmitModel.GoogleMapsIFrame.Contains("width=\"100%\""))
            {
                SubmitModel.GoogleMapsIFrame = Regex.Replace(SubmitModel.GoogleMapsIFrame, "width=\"\\d+\"", "width=\"100%\"").Trim();
            }

            var SP = new SystemProperties();
            SP.ContactEmail = SubmitModel.ContactEmail ?? Constants.NullValueFor.String;
            SP.ContactPhone = SubmitModel.ContactPhone ?? Constants.NullValueFor.String;
            SP.ContactAddress = SubmitModel.ContactAddress ?? Constants.NullValueFor.String;
            SP.FacebookUrl = SubmitModel.FacebookUrl ?? Constants.NullValueFor.String;
            SP.TwitterUrl = SubmitModel.TwitterUrl ?? Constants.NullValueFor.String;
            SP.InstagramUrl = SubmitModel.InstagramUrl ?? Constants.NullValueFor.String;
            SP.YoutubeUrl = SubmitModel.YoutubeUrl ?? Constants.NullValueFor.String;
            SP.LinkedInUrl = SubmitModel.LinkedInUrl ?? Constants.NullValueFor.String;
            SP.GoogleMapsIFrame = SubmitModel.GoogleMapsIFrame ?? Constants.NullValueFor.String;
            SP.SMTPAddress = SubmitModel.SMTPAddress ?? Constants.NullValueFor.String;
            SP.SMTPPort = SubmitModel.SMTPPort ?? Constants.NullValueFor.Int;
            SP.SMTPUsername = SubmitModel.SMTPUsername ?? Constants.NullValueFor.String;
            SP.SMTPPassword = SubmitModel.SMTPPassword ?? Constants.NullValueFor.String;
            SP.SMTPUseSSL = SubmitModel.SMTPUseSSL;
            SP.SMTPFrom = SubmitModel.SMTPFrom ?? Constants.NullValueFor.String;
            SP.ScriptsHeader = SubmitModel.ScriptsHeader ?? Constants.NullValueFor.String;
            SP.ScriptsBodyStart = SubmitModel.ScriptsBodyStart ?? Constants.NullValueFor.String;
            SP.ScriptsBodyEnd = SubmitModel.ScriptsBodyEnd ?? Constants.NullValueFor.String;
            await DataAccessFactory.SystemProperties.UpdateSystemProperties(SP);
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
            public string SMTPAddress { get; set; }
            public int? SMTPPort { get; set; }
            public string SMTPUsername { get; set; }
            public string SMTPPassword { get; set; }
            public bool SMTPUseSSL { get; set; }            
            public string SMTPFrom { get; set; }
            public string ScriptsHeader { get; set; }
            public string ScriptsBodyStart { get; set; }
            public string ScriptsBodyEnd { get; set; }

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
