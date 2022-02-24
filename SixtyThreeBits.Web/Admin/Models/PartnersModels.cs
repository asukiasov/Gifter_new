using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class PartnersModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridDelete);
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridDelete);
            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var Partners = (await DataAccessFactory.Partners.ListPartners())?.Select(Item => new PageViewModel.GridModel.GridItem
            {

                PartnerID = Item.PartnerID,
                PartnerName = Item.PartnerName,
                PartnerWebSite = Item.PartnerWebSite,
                UrlPartnerProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.Partner.Properties, new { PartnersID = Item.PartnerID })
            }).ToList();
            return Partners;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? PartnerID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if(DatabaseAction == Enums.DatabaseActions.DELETE)
            {
            var DBItem = await DataAccessFactory.Partners.GetSinglePartnerByID(PartnerID);
                if(DBItem != null)
                {
                    Utilities.DeleteUploadedFile(DBItem.PartnerImageFilename);
                }
            }
            await DataAccessFactory.Partners.PartnersIUD(
                DatabaseAction: DatabaseAction,
                PartnerID: PartnerID,
                PartnerName: SubmitModel.PartnerName,
                PartnerWebSite: SubmitModel.PartnerWebSite
            );
            if (DataAccessFactory.Partners.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Sub Clases
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.PartnerID));
                    Grid
                    .ID("PartnerGridID")
                    .Scrolling(Options =>
                    {
                        Options.Mode(GridScrollingMode.Standard);
                        Options.ShowScrollbar(ShowScrollbarMode.Always);
                    })
                    .OnInitialized("PartnersModel.OnPartnersGridInitialized")
                    .Columns(Columns =>
                    {
                        Columns.Add().Width(30).Caption(" ").CellTemplate(new JS("PartnersModel.GetDetailsButtonColumnCellHtml"));
                        Columns.AddFor(m => m.PartnerName).Caption("დასახელება").Width(350).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.PartnerWebSite).Caption("ვებ გვერდის URL").Width(350);
                        Columns.Add();
                    });


                    return Grid;

                }
                #endregion

                #region Sub Classes
                public class GridItem
                {
                    #region Properties
                    public int? PartnerID { get; set; }
                    public string PartnerName { get; set; }
                    public string PartnerWebSite { get; set; }
                    public string UrlPartnerProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion  
        }
        #endregion
    }

    public class PartnersModelBase : WebProjectModelBase
    {
        #region Properties
        public Partner DBItemPartner { get; set; }
        #endregion
    }

    public class PartnerPropertiesModel : PartnersModelBase
    {
        #region Methods
        public PartnerPropertiesViewModel GetPartnerPropertiesViewModel(PartnerPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new PartnerPropertiesViewModel();
                ViewModel.PartnerName = DBItemPartner.PartnerName;
                ViewModel.PartnerNameEng = DBItemPartner.PartnerNameEng;
                ViewModel.PartnernameRus = DBItemPartner.PartnerNameRus;
                ViewModel.PartnerShortDescription = DBItemPartner.PartnerShortDescription;
                ViewModel.PartnerShortDescriptionEng = DBItemPartner.PartnerShortDescriptionEng;
                ViewModel.PartnerShortDescriptionRus = DBItemPartner.PartnerShortDescriptionRus;
                ViewModel.PartnerFullDescription = DBItemPartner.PartnerFullDescription;
                ViewModel.PartnerFullDescriptionEng = DBItemPartner.PartnerFullDescriptionEng;
                ViewModel.PartnerFullDescriptionRus = DBItemPartner.PartnerFullDescriptionRus;
                ViewModel.PartnerWebSite = DBItemPartner.PartnerWebSite;
            }
            ViewModel.PartnerImageFilename = DBItemPartner.PartnerImageFilename;
            ViewModel.PartnerImageHttpPath = Utilities.GetUploadedFileHttpPath(ViewModel.PartnerImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersPartnerPropertiesDeleteImage, new { PartnersID = DBItemPartner.PartnerID });
            ViewModel.ShowPartnerImageDeleteButton = !string.IsNullOrWhiteSpace(ViewModel.PartnerImageFilename);
            return ViewModel;
        }
        public async Task<bool> SavePartnerProperties(PartnerPropertiesViewModel ViewModel)
        {
            var HasPartnerImage = ViewModel.PostedFile?.Length > 0;
            var PartnerImageFilename = HasPartnerImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;
            if (HasPartnerImage)
            {
                Utilities.DeleteUploadedFile(DBItemPartner.PartnerImageFilename);
            }
            await DataAccessFactory.Partners.PartnersIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PartnerID: DBItemPartner.PartnerID,
                PartnerName: ViewModel.PartnerName,
                PartnerNameEng: ViewModel.PartnerNameEng,
                PartnerNameRus: ViewModel.PartnernameRus,
                PartnerShortDescription: ViewModel.PartnerShortDescription,
                PartnerShortDescriptionEng: ViewModel.PartnerShortDescriptionEng,
                PartnerShortDescriptionRus: ViewModel.PartnerShortDescriptionRus,
                PartnerFullDescription: ViewModel.PartnerFullDescription,
                PartnerFullDescriptionEng: ViewModel.PartnerFullDescriptionEng,
                PartnerFullDescriptionRus: ViewModel.PartnerFullDescriptionRus,
                PartnerWebSite: ViewModel.PartnerWebSite,
                PartnerImageFilename: PartnerImageFilename
            );
            if (!DataAccessFactory.Partners.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasPartnerImage)
                {
                    await SaveUploadedFile(ViewModel.PostedFile, PartnerImageFilename);
                }
            }
            return ViewModel.IsSaved;
        }
        public async Task<AjaxResponse> DeleteImage(int? PartnerID)
        {
            var Partner = await DataAccessFactory.Partners.GetSinglePartnerByID(PartnerID);
            Utilities.DeleteUploadedFile(Partner.PartnerImageFilename);
            var AR = new AjaxResponse();
            await DataAccessFactory.Partners.PartnersIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                PartnerID: PartnerID,
                PartnerImageFilename: Constants.NullValueFor.String
            );
            AR.IsSuccess = !DataAccessFactory.Partners.IsError;
            return AR;
        }
        public void ValidatePartnerPropertiesViewModel(PartnerPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>()
            {
                Validation.ValidateRequired(ErrorKey : Validation.GetJQueryNameSelectorFor(nameof(ViewModel.PartnerName)),ViewModel.PartnerName),
            };
            ViewModel.Errors.RemoveAll(Item => Item == null);

        }
        #endregion

        #region SubClasses
        public class PartnerPropertiesViewModel : FormViewModelBase
        {
            #region Properties
            public int PartnerID { get; set; }
            public string PartnerName { get; set; }
            public string PartnerNameEng { get; set; }
            public string PartnernameRus { get; set; }
            public string PartnerShortDescription { get; set; }
            public string PartnerShortDescriptionEng { get; set; }
            public string PartnerShortDescriptionRus { get; set; }
            public string PartnerFullDescription { get; set; }
            public string PartnerFullDescriptionEng { get; set; }
            public string PartnerFullDescriptionRus { get; set; }
            public string PartnerWebSite { get; set; }
            public string PartnerImageFilename { get; set; }
            public string PartnerImageHttpPath { get; set; }
            public string UrlDeleteImage { get; set; }
            public string TextConfirmDelete { get; set; } = Resources.TextConfirmDeleteImage;
            public bool ShowPartnerImageDeleteButton { get; set; }
            public string UrlPartnerProperties { get; set; }
            public IFormFile PostedFile { get; set; }
            #endregion
        }
        #endregion
    }
}