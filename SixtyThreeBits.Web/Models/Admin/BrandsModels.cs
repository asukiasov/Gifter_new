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
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web.Models.Admin
{
    public class BrandsModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridAdd);

            viewModel.Grid = new ViewModel.GridViewModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridDelete);

            return viewModel;
        }

        public async Task<List<ViewModel.GridViewModel.GridItem>> ListGridItems()
        {
            var repository = RepositoriesFactory.CreateBrandsRepository();
            var brands = (await repository.BrandsList())?.Select(item => new ViewModel.GridViewModel.GridItem
            {
                BrandID = item.BrandID,
                BrandName = item.BrandName,
                BrandNameEng = item.BrandNameEng,
                UrlBrandProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandPropertiesController.Properties, new { brandID = item.BrandID })
            }).ToList();
            return brands;
        }

        public async Task IUD(Enums.DatabaseActions DatabaseAction, int? brandID, ViewModel.GridViewModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.CreateBrandsRepository();

            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var dbItem = await repository.BrandsGetSingleByID(brandID);
                await DeleteUploadedFile(dbItem.BrandImageFilename);
            }

            await repository.BrandsIUD(
                databaseAction: DatabaseAction,
                brandID: brandID,
                brand: new BrandIudDTO
                {
                    BrandName = submitModel.BrandName,
                    BrandNameEng = submitModel.BrandNameEng ?? Constants.NullValueFor.String
                }
            );

            if (repository.IsError)
            {
                Form.AddError(repository.ErrorMessage);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridViewModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridViewModel : DevExtremeGridViewModelBase<GridViewModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.BrandID));

                    grid
                      .ID("BrandsGrid")
                      .OnInitialized("brandsModel.onGridInit")
                      .Columns(columns =>
                      {
                          columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlBrandProperties));
                          columns.AddFor(m => m.BrandName).Caption(Resources.TextName).Width(350).ValidationRules(Options =>
                          {
                              Options.AddRequired();
                          });
                          columns.AddFor(m => m.BrandNameEng).Caption(Resources.TextNameEng).Width(350);
                          columns.Add();
                      });

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? BrandID { get; set; }
                    public string BrandName { get; set; }
                    public string BrandNameEng { get; set; }
                    public string UrlBrandProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }

    public class BrandsModelBase : ModelBase
    {
        #region Properties
        public BrandDTO DBItem { get; set; }
        #endregion
    }

    public class BrandsPropertiesModel : BrandsModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
                viewModel.BrandName = DBItem.BrandName;
                viewModel.BrandNameEng = DBItem.BrandNameEng;
            }
            viewModel.BrandImageFilename = DBItem.BrandImageFilename;
            viewModel.BrandImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.BrandImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandPropertiesController.DeleteImage, new { brandID = DBItem.BrandID });

            return viewModel;
        }

        public void Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.BrandName)), valueToValidate: viewModel.BrandName));
        }

        public async Task Save(ViewModel viewModel)
        {            
            var brandImageFilename = default(string);

            var hasBrandImage = viewModel.BrandImageFile?.Length > 0;            
            if (hasBrandImage)
            {
                await DeleteUploadedFile(viewModel.BrandImageFilename);
                brandImageFilename = GetFilenameFromUploadedFile(viewModel.BrandImageFile);
            }

            var repository = RepositoriesFactory.CreateBrandsRepository();
            await repository.BrandsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                brandID: DBItem.BrandID,
                brand: new BrandIudDTO
                {
                    BrandName = viewModel.BrandName,
                    BrandNameEng = viewModel.BrandNameEng,
                    BrandImageFilename = brandImageFilename
                }                
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasBrandImage)
                {
                    await SaveUploadedFile(viewModel.BrandImageFile, brandImageFilename);
                }
            }
        }        

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateBrandsRepository();

            await DeleteUploadedFile(DBItem.BrandImageFilename);

            await repository.BrandsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                brandID: DBItem.BrandID,
                brand: new BrandIudDTO
                {
                    BrandImageFilename = Constants.NullValueFor.String
                }                
            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : FormViewModelBase
        {
            #region Properties           
            public string BrandName { get; set; }
            public string BrandNameEng { get; set; }
            public string BrandImageFilename { get; set; }
            public string BrandImageHttpPath { get; set; }
            public bool HasBrandImage => !string.IsNullOrWhiteSpace(BrandImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile BrandImageFile { get; set; }

            public readonly string TextName = Resources.TextName;
            public readonly string TextNameEng = Resources.TextNameEng;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            #endregion
        }
        #endregion
    }
}
