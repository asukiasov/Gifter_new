using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PartnersModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGrid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.PartnersGridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Partners.PartnersGridDelete);
            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetPartnersRepository();
            var viewModel = (await repository.PartnersList())
            ?.Select(item => new PageViewModel.GridModel.GridItem
            {
                PartnerID = item.PartnerID,
                PartnerName = item.PartnerName,
                PartnerWebSite = item.PartnerWebSite,
                PartnerIsPublished = item.PartnerIsPublished,
                UrlPartnerProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.Partner.Properties, new { partnerID = item.PartnerID })
            })
            .ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? partnerID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetPartnersRepository();

            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var dbItem = await repository.PartnersGetSingleByID(partnerID);
                await DeleteUploadedFile(dbItem.PartnerImageFilename, folderPath: null);
            }

            await repository.PartnersIUD(
                databaseAction: databaseAction,
                partnerID: partnerID,
                partnerName: submitModel.PartnerName,
                partnerWebSite: submitModel.PartnerWebSite,
                partnerIsPublished: submitModel.PartnerIsPublished
            );
            if (repository.IsError)
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

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.PartnerID));
                    grid
                    .ID("PartnerGridID")
                    .OnInitialized("partnersModel.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlPartnerProperties));
                        columns.AddFor(m => m.PartnerName).Caption(Resources.TextName).Width(350).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        columns.AddFor(m => m.PartnerWebSite).Caption(Resources.TextPageUrl).Width(350);
                        columns.AddFor(m => m.PartnerIsPublished).Caption(Resources.TextPublished).Width(150).InitCheckboxColumn();
                        columns.Add();
                    });


                    return grid;

                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? PartnerID { get; set; }
                    public string PartnerName { get; set; }
                    public string PartnerWebSite { get; set; }
                    public bool PartnerIsPublished { get; set; }
                    public string UrlPartnerProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion  
        }
        #endregion
    }

    public class PartnersModelBase : ModelBase
    {
        #region Properties
        public PartnerDTO DBItem { get; set; }
        #endregion
    }

    public class PartnerPropertiesModel : PartnersModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel(PageViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.PartnerName = DBItem.PartnerName;
                viewModel.PartnerNameEng = DBItem.PartnerNameEng;
                viewModel.PartnerShortDescription = DBItem.PartnerShortDescription;
                viewModel.PartnerShortDescriptionEng = DBItem.PartnerShortDescriptionEng;
                viewModel.PartnerFullDescription = DBItem.PartnerFullDescription;
                viewModel.PartnerFullDescriptionEng = DBItem.PartnerFullDescriptionEng;
                viewModel.PartnerWebSite = DBItem.PartnerWebSite;
                viewModel.PartnerIsPublished = DBItem.PartnerIsPublished;
            }
            viewModel.PartnerImageFilename = DBItem.PartnerImageFilename;
            viewModel.PartnerImageHttpPath = FileStorage.GetUploadedFileHttpPath(viewModel.PartnerImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Partners.Partner.PropertiesDeleteImage, new { DBItem.PartnerID });
            viewModel.ShowPartnerImageDeleteButton = !string.IsNullOrWhiteSpace(viewModel.PartnerImageFilename);
            return viewModel;
        }

        public void ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.PartnerName)), viewModel.PartnerName));
        }

        public async Task Save(PageViewModel viewModel)
        {
            var hasPartnerImage = viewModel.PartnerImageFile?.Length > 0;
            var partnerImageFilename = hasPartnerImage ? GetFilenameFromUploadedFile(viewModel.PartnerImageFile) : null;
            if (hasPartnerImage)
            {
                await DeleteUploadedFile(DBItem.PartnerImageFilename, folderPath: null);
            }

            var repository = RepositoriesFactory.GetPartnersRepository();
            await repository.PartnersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                partnerID: DBItem.PartnerID,
                partnerName: viewModel.PartnerName,
                partnerNameEng: viewModel.PartnerNameEng,
                partnerShortDescription: viewModel.PartnerShortDescription,
                partnerShortDescriptionEng: viewModel.PartnerShortDescriptionEng,
                partnerFullDescription: viewModel.PartnerFullDescription,
                partnerFullDescriptionEng: viewModel.PartnerFullDescriptionEng,
                partnerWebSite: viewModel.PartnerWebSite,
                partnerImageFilename: partnerImageFilename,
                partnerIsPublished: viewModel.PartnerIsPublished
            );

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasPartnerImage)
                {
                    await SaveUploadedFile(viewModel.PartnerImageFile, partnerImageFilename, folderPath: null);
                }
            }
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            await DeleteUploadedFile(DBItem.PartnerImageFilename, folderPath: null);
            var repository = RepositoriesFactory.GetPartnersRepository();
            await repository.PartnersIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                partnerID: DBItem.PartnerID,
                partnerImageFilename: Constants.NullValueFor.String
            );
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region SubClasses
        public class PageViewModel : FormViewModelBase
        {
            #region Properties            
            public bool PartnerIsPublished { get; set; }
            public string PartnerName { get; set; }
            public string PartnerNameEng { get; set; }
            public string PartnerShortDescription { get; set; }
            public string PartnerShortDescriptionEng { get; set; }
            public string PartnerFullDescription { get; set; }
            public string PartnerFullDescriptionEng { get; set; }
            public string PartnerWebSite { get; set; }
            public string PartnerImageFilename { get; set; }
            public string PartnerImageHttpPath { get; set; }
            public string UrlDeleteImage { get; set; }
            public bool ShowPartnerImageDeleteButton { get; set; }
            public string UrlPartnerProperties { get; set; }
            public IFormFile PartnerImageFile { get; set; }

            public readonly string TextPublished = Resources.TextPublished;
            public readonly string TextName = Resources.TextName;
            public readonly string TextNameEng = Resources.TextNameEng;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextDescription = Resources.TextDescription;
            public readonly string TextDescriptionEng = Resources.TextDescriptionEng;
            public readonly string TextPageUrl = Resources.TextPageUrl;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            #endregion
        }
        #endregion
    }
}