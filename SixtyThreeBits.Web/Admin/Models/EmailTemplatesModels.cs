using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class EmailTemplatesModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.Grid = new PageViewModel.GridModel();            
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.Grid);
            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridModel()
        {            
            var ViewModel = (await DataAccessFactory.EmailTemplates.ListEmailTemplates())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                EmailTemplateID = Item.EmailTemplateID,
                EmailTemplateName = Item.EmailTemplateName,
                UrlEmailTemplate = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplates.EmailTemplate.Properties, new { EmailTemplateID = Item.EmailTemplateID })
            }).ToList();
            return ViewModel;
        }        
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties            
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.EmailTemplateID));

                    Grid                        
                    .ID("EmailTemplatesGrid")
                    .OnInitialized("EmailTemplatesModel.OnGridInit")
                    .Columns(Columns =>
                    {
                        Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlEmailTemplate));
                        Columns.AddFor(m => m.EmailTemplateName).Caption("Templates").Width(300);
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion

                #region Sub CLasses
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

    public class EmailTemplatePropertiesModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel(int? EmailTemplateID, PageViewModel ViewModel = null)
        {
            var DBItem = await DataAccessFactory.EmailTemplates.GetSingleEmailTemplateByID(EmailTemplateID);
            if (DBItem == null)
            {
                ViewModel = null;
            }
            else
            {
                if(ViewModel == null)
                {
                    ViewModel = new PageViewModel();
                    ViewModel.EmailTemplateName = DBItem.EmailTemplateName;
                    ViewModel.EmailTemplateSubject = DBItem.EmailTemplateSubject;
                    ViewModel.EmailTemplateSubjectEng = DBItem.EmailTemplateSubjectEng;
                    ViewModel.EmailTemplateBody = DBItem.EmailTemplateBody;
                    ViewModel.EmailTemplateBodyEng = DBItem.EmailTemplateBodyEng;
                }
                ViewModel.EmailTemplatePlaceHoldersJson = DBItem.EmailTemplatesPlaceHolders?.Any() == true ? DBItem.EmailTemplatesPlaceHolders.ToJson() : "[]";
            }

            return ViewModel;
        }

        public void ValidatePageViewModel(PageViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {
                Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(ViewModel.EmailTemplateName)),ViewModel.EmailTemplateName),
                Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(ViewModel.EmailTemplateSubject)),ViewModel.EmailTemplateSubject),
                //Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(ViewModel.EmailTemplateBody)),ViewModel.EmailTemplateBody),
                //Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(ViewModel.EmailTemplateSubjectEng)),ViewModel.EmailTemplateSubjectEng),
                //Validation.ValidateRequired(Validation.GetJQueryNameSelectorFor(nameof(ViewModel.EmailTemplateBodyEng)),ViewModel.EmailTemplateBodyEng)
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        public async Task Save(int? EmailTemplateID, PageViewModel ViewModel)
        {
            await DataAccessFactory.EmailTemplates.EmailTemplatesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                EmailTemplateID: EmailTemplateID,
                EmailTemplateName: ViewModel.EmailTemplateName,
                EmailTemplateSubject: ViewModel.EmailTemplateSubject,
                EmailTemplateSubjectEng: ViewModel.EmailTemplateSubjectEng,
                EmailTemplateBody: ViewModel.EmailTemplateBody,
                EmailTemplateBodyEng: ViewModel.EmailTemplateBodyEng
            );
            if (DataAccessFactory.EmailTemplates.IsError)
            {
                ViewModel.AddError(DataAccessFactory.EmailTemplates.ErrorMessage);
            }
            else
            {
                ViewModel.IsSaved = true;
            }
        }
        #endregion

        #region Sub Classes
        public class PageViewModel : FormViewModelBase
        {
            #region Properties
            public string EmailTemplateName { get; set; }
            public string EmailTemplateSubject { get; set; }
            public string EmailTemplateSubjectEng { get; set; }
            public string EmailTemplateBody { get; set; }
            public string EmailTemplateBodyEng { get; set; }
            public string EmailTemplatePlaceHoldersJson { get; set; }
            #endregion
        }
        #endregion
    }
}