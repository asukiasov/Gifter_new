using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Models.Base;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class EmailTemplatePropertiesModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel(int? emailTemplateID, ViewModel viewModel = null)
        {
            var repository = RepositoriesFactory.CreateEmailTemplatesRepository();
            var dbItem = await repository.EmailTemplatesGetSingleByID(emailTemplateID);
            if (dbItem == null)
            {
                viewModel = null;
            }
            else
            {
                if (viewModel == null)
                {
                    viewModel = new ViewModel();
                    viewModel.EmailTemplateName = dbItem.EmailTemplateName;
                    viewModel.EmailTemplateSubject = dbItem.EmailTemplateSubject;
                    viewModel.EmailTemplateSubjectEng = dbItem.EmailTemplateSubjectEng;
                    viewModel.EmailTemplateBody = dbItem.EmailTemplateBody;
                    viewModel.EmailTemplateBodyEng = dbItem.EmailTemplateBodyEng;
                }
            }

            return viewModel;
        }

        public async Task<ViewModel> Save(int? emailTemplateID, ViewModel submitModel)
        {
            var viewModel = await GetViewModel(emailTemplateID, submitModel);

            var validationResult = validateSubmitModel(submitModel);

            if (validationResult.HasErrors)
            {
                viewModel.AddFormErrors(validationResult.Errors);
            }
            else
            {
                var repository = RepositoriesFactory.CreateEmailTemplatesRepository();
                await repository.EmailTemplatesIUD(
                    databaseAction: Enums.DatabaseActions.UPDATE,
                    emailTemplateID: emailTemplateID,
                    emailTemplate: new EmailTemplateIudDTO
                    {
                        EmailTemplateName = submitModel.EmailTemplateName,
                        EmailTemplateSubject = submitModel.EmailTemplateSubject,
                        EmailTemplateSubjectEng = submitModel.EmailTemplateSubjectEng,
                        EmailTemplateBody = submitModel.EmailTemplateBody,
                        EmailTemplateBodyEng = submitModel.EmailTemplateBodyEng
                    }
                );
                if (repository.IsError)
                {
                    viewModel.AddToastError(repository.ErrorMessage);
                }
            }

            return viewModel;
        }

        ValidationResult63 validateSubmitModel(ViewModel submitModel)
        {
            var validationResult = new ValidationResult63();
            var error = default(Error63);

            error = Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(submitModel.EmailTemplateName)), submitModel.EmailTemplateName);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(submitModel.EmailTemplateSubject)), submitModel.EmailTemplateSubject);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(submitModel.EmailTemplateBody)), submitModel.EmailTemplateBody);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(submitModel.EmailTemplateSubjectEng)), submitModel.EmailTemplateSubjectEng);
            validationResult.AddError(error);

            error = Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(submitModel.EmailTemplateBodyEng)), submitModel.EmailTemplateBodyEng);
            validationResult.AddError(error);

            return validationResult;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties
            public string EmailTemplateName { get; set; }
            public string EmailTemplateSubject { get; set; }
            public string EmailTemplateSubjectEng { get; set; }
            public string EmailTemplateBody { get; set; }
            public string EmailTemplateBodyEng { get; set; }
            public string EmailTemplatePlaceHoldersJson { get; set; }

            public readonly string TextCaption = Resources.TextCaption;
            #endregion
        }
        #endregion
    }
}