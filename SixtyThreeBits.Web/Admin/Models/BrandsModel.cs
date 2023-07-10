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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web.Admin.Models
{
    public class BrandsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridDelete);
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var Brands = (await DataAccessFactory.Brands.ListBrands())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                BrandID = Item.BrandID,
                BrandName = Item.BrandName,
                BrandNameEng = Item.BrandNameEng,
                BrandNameRus = Item.BrandNameRus,
                UrlBrandProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.Properties, new { BrandID = Item.BrandID })
            }).ToList();
            return Brands;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? BrandID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.Brands.GetSingleBrandByID(BrandID);
                Utilities.DeleteUploadedFile(DBItem?.BrandImageFilename);
            }

            await DataAccessFactory.Brands.BrandsIUD(
                DatabaseAction: DatabaseAction,
                BrandID: BrandID,
                BrandName: SubmitModel.BrandName,
                BrandNameEng: SubmitModel.BrandNameEng ?? Constants.NullValueFor.String,
                BrandNameRus: SubmitModel.BrandNameRus ?? Constants.NullValueFor.String
            );

            if (DataAccessFactory.Brands.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Nested Classes
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
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.BrandID));

                    Grid
                      .ID("BrandsGrid")                      
                      .OnInitialized("BrandsModel.OnGridInit")
                      .Columns(Columns =>
                      {
                          Columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlBrandProperties));
                          Columns.AddFor(m => m.BrandName).Caption("დასახელება").Width(350).ValidationRules(Options =>
                          {
                              Options.AddRequired();
                          });
                          Columns.AddFor(m => m.BrandNameEng).Caption("დასახელება Eng").Width(350);
                          Columns.AddFor(m => m.BrandNameRus).Caption("დასახელება Rus").Width(350);
                          Columns.Add();
                      });

                    return Grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? BrandID { get; set; }
                    public string BrandName { get; set; }
                    public string BrandNameEng { get; set; }
                    public string BrandNameRus { get; set; }
                    public string UrlBrandProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion

    }

    public class BrandsModelBase : WebProjectModelBase
    {
        #region Properties
        public Brand DBItemBrands { get; set; }
        #endregion
    }

    public class BrandsPropertiesModel : BrandsModelBase
    {
        #region Methods
        public BrandsPropertiesViewModel GetPageViewModel(int? BrandID, BrandsPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new BrandsPropertiesViewModel();
                ViewModel.BrandName = DBItemBrands.BrandName;
                ViewModel.BrandNameEng = DBItemBrands.BrandNameEng;
                ViewModel.BrandNameRus = DBItemBrands.BrandNameRus;
            }
            ViewModel.BrandImageFilename = DBItemBrands.BrandImageFilename;
            ViewModel.BrandImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemBrands.BrandImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.DeleteCoverImage, new { BrandID = BrandID });

            return ViewModel;
        }

        public async Task<AjaxResponse> DeleteImage(int? BrandID)
        {

            Utilities.DeleteUploadedFile(DBItemBrands.BrandImageFilename);

            var AR = new AjaxResponse();

            await DataAccessFactory.Brands.BrandsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BrandID: BrandID,
                BrandImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Brands.IsError;

            return AR;
        }

        public async Task SaveBrandsProperties(int? BrandID, BrandsPropertiesViewModel ViewModel)
        {
            var HasBrandImage = ViewModel.PostedFile?.Length > 0;
            var BrandImageFilename = HasBrandImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;

            if (HasBrandImage)
            {
                Utilities.DeleteUploadedFile(ViewModel.BrandImageFilename);
            }

            await DataAccessFactory.Brands.BrandsIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                BrandID: BrandID,
                BrandName: ViewModel.BrandName,
                BrandNameEng: ViewModel.BrandNameEng,
                BrandNameRus: ViewModel.BrandNameRus,
                BrandImageFilename: BrandImageFilename
          
            );

            if (!DataAccessFactory.Brands.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasBrandImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: BrandImageFilename);
                }
            }
        }

        public void ValidatePageViewModel(BrandsPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {

                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.BrandName)}\"]", ValueToValidate:ViewModel.BrandName)

            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }
        #endregion

        #region Nested Classes
        public class BrandsPropertiesViewModel : FormViewModelBase
        {
            #region Properties           
            public string BrandName { get; set; }
            public string BrandNameEng { get; set; }
            public string BrandNameRus { get; set; }           
            public string BrandImageFilename { get; set; }
            public string BrandImageHttpPath { get; set; }
            public bool HasBrandImage => !string.IsNullOrWhiteSpace(BrandImageFilename);
            public string UrlDeleteImage { get; set; }            
            public IFormFile PostedFile { get; set; }

            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
            #endregion
        }
        #endregion
    }
}
