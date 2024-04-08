using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class EmailTemplatesModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.Grid = new();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.Grid);
            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.GetEmailTemplatesRepository();
            var viewModel = (await repository.EmailTemplatesList())
            ?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                EmailTemplateID = Item.EmailTemplateID,
                EmailTemplateName = Item.EmailTemplateName,
                UrlEmailTemplate = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.EmailTemplate.Properties, new { Item.EmailTemplateID })
            })
            .ToList();
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties            
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.EmailTemplateID));

                    grid
                    .ID("EmailTemplatesGrid")
                    .OnInitialized("emailTemplatesModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlEmailTemplate));
                        columns.AddFor(m => m.EmailTemplateName).Caption(Resources.TextTemplate).Width(300);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? EmailTemplateID { get; set; }
                    public string EmailTemplateName { get; set; }
                    public string UrlEmailTemplate { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class EmailTemplatePropertiesModel : ModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel(int? emailTemplateID, PageViewModel viewModel = null)
        {
            var repository = RepositoriesFactory.GetEmailTemplatesRepository();
            var dbItem = await repository.EmailTemplatesGetSingleByID(emailTemplateID);
            if (dbItem == null)
            {
                viewModel = null;
            }
            else
            {
                if (viewModel == null)
                {
                    viewModel = new PageViewModel();
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

        public void ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateName)), viewModel.EmailTemplateName));
            viewModel.AddError(Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateSubject)), viewModel.EmailTemplateSubject));
            viewModel.AddError(Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateBody)), viewModel.EmailTemplateBody));
            viewModel.AddError(Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateSubjectEng)), viewModel.EmailTemplateSubjectEng));
            viewModel.AddError(Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(viewModel.EmailTemplateBodyEng)), viewModel.EmailTemplateBodyEng));
        }

        public async Task Save(int? emailTemplateID, PageViewModel viewModel)
        {
            var repository = RepositoriesFactory.GetEmailTemplatesRepository();
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
            else
            {
                viewModel.IsSaved = true;
            }
        }
        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
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