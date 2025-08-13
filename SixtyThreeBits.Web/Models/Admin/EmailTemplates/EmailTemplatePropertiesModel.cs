using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Models.Base;
using System.Linq;
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
                viewModel.EmailTemplatePlaceHoldersJson = dbItem.EmailTemplatesPlaceHolders?.Any() == true ? dbItem.EmailTemplatesPlaceHolders.ToJson() : "[]";
            }

            return viewModel;
        }

        public void ValidateViewModel(ViewModel viewModel)
        {
            viewModel.AddError(Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateName)), viewModel.EmailTemplateName));
            viewModel.AddError(Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateSubject)), viewModel.EmailTemplateSubject));
            viewModel.AddError(Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateBody)), viewModel.EmailTemplateBody));
            viewModel.AddError(Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateSubjectEng)), viewModel.EmailTemplateSubjectEng));
            viewModel.AddError(Validation63.ValidateRequired(Validation63.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateBodyEng)), viewModel.EmailTemplateBodyEng));
        }

        public async Task Save(int? emailTemplateID, ViewModel viewModel)
        {
            var repository = RepositoriesFactory.CreateEmailTemplatesRepository();
            await repository.EmailTemplatesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                emailTemplateID: emailTemplateID,
                emailTemplate: new EmailTemplateIudDTO
                {
                    EmailTemplateName = viewModel.EmailTemplateName,
                    EmailTemplateSubject = viewModel.EmailTemplateSubject,
                    EmailTemplateSubjectEng = viewModel.EmailTemplateSubjectEng,
                    EmailTemplateBody = viewModel.EmailTemplateBody,
                    EmailTemplateBodyEng = viewModel.EmailTemplateBodyEng
                }                
            );
            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }            
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