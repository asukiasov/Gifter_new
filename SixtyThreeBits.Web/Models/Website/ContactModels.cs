using SixtyThreeBits.Core.Infrastructure.Services;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class ContactModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ContactPhone = SystemProperties.ContactPhone;
            viewModel.ContactAddress = Utilities.GetValuesByLanguage(LanguageCultureCode, SystemProperties.ContactAddress, SystemProperties.ContactAddressEng);
            viewModel.ContactEmail = SystemProperties.ContactEmail;
            viewModel.RecaptchaSiteKey = SystemProperties.ReCaptchaSiteKey;
            return viewModel;
        }

        public async Task<ValidationResult> ValidateSubmitModel(SubmitModel submitModel)
        {
            var result = new ValidationResult();

            var error = Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(submitModel.Name)), submitModel.Name);
            result.AddError(error);

            error = await Validation.ValidateEmail(
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(submitModel.Email)),
                userEmail: submitModel.Email,
                validateRequired: true
            );
            result.AddError(error);

            error = Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(submitModel.Message)), submitModel.Message);
            result.AddError(error);

            error = await Validation.ValidateAsync(
                errorAction: async () =>
                {
                    var isError = false;
                    var service = new GoogleRecaptchaService(recaptchaSecretKey: SystemProperties.ReCaptchaSecretKey);
                    var recaptchaResult = await service.VerifyRecaptcha(submitModel.RecaptchaClientResponseToken);
                    isError = !recaptchaResult.IsSuccess;
                    return isError;
                },
                errorKey: Validation.GetJQueryNameSelectorFor(nameof(submitModel.RecaptchaClientResponseToken)),
                errorMessage: Resources.ValidationRecaptchaInvalid
            );
            result.AddError(error);

            return result;
        }

        public async Task<AjaxResponse> SendContactEmail(SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();

            await NotificationManager.SendContactUsEmailToAdmins(
                name: submitModel.Name,
                email: submitModel.Email,
                message: submitModel.Message,
                emailsTo: SystemProperties.AdminEmails
            );
            viewModel.IsSuccess = true;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string ContactAddress { get; set; }
            public string RecaptchaSiteKey { get; set; }

            public readonly string TextContactUs = Resources.TextContactUs;            
            public readonly string TextPhone = Resources.TextPhone;
            public readonly string TextEmail = Resources.TextEmail;
            public readonly string TextAddress = Resources.TextAddress;
            
            public readonly string TextName = Resources.TextName;
            public readonly string TextMessage = Resources.TextMessage;
            public readonly string TextSend = Resources.TextSend;
            public readonly string TextContactFormSuccess = Resources.TextContactFormSuccess;
            #endregion
        }

        public class SubmitModel
        {
            #region Properties
            public string Name { get; set; }
            public string Email { get; set; }
            public string Message { get; set; }
            public string RecaptchaClientResponseToken { get; set; }
            #endregion
        }
        #endregion
    }
}
