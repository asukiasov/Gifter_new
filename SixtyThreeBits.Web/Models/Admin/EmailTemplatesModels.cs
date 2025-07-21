using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class EmailTemplatesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.Grid = new();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplatesController.Grid);
            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreateEmailTemplatesRepository();
            var viewModel = (await repository.EmailTemplatesList())?
            .Select(Item => new ViewModel.GridModel.GridItem
            {
                EmailTemplateID = Item.EmailTemplateID,
                EmailTemplateName = Item.EmailTemplateName,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplatePropertiesController.Properties, new { emailTemplateID = Item.EmailTemplateID })
            })
            .ToList();
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties            
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.EmailTemplateID));

                    grid
                    .ID("EmailTemplatesGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProperties));
                        columns.AddFor(m => m.EmailTemplateID).Caption("#").Width(60);
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
                    public string UrlProperties { get; set; }
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