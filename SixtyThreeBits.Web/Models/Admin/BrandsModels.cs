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
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web.Models.Admin
{
    public class BrandsModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);

            viewModel.Grid = new PageViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGrid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.BrandsGridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Brands.BrandsGridDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetBrandsRepository();
            var brands = (await repository.BrandsList())?.Select(item => new PageViewModel.GridModel.GridItem
            {
                BrandID = item.BrandID,
                BrandName = item.BrandName,
                BrandNameEng = item.BrandNameEng,
                UrlBrandProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.Properties, new { brandID = item.BrandID })
            }).ToList();
            return brands;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? brandID, PageViewModel.GridModel.GridItem submitModel)
        {
            var repository = RepositoriesFactory.GetBrandsRepository();

            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var dbItem = await repository.BrandsGetSingleByID(brandID);
                await DeleteUploadedFile(dbItem.BrandImageFilename, folderPath: null);
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
                    var grid = GetGridWithStartupValues<GridItem>(html: html, keyFieldName: nameof(GridItem.BrandID));

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
        public PageViewModel GetPageViewModel(PageViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
                viewModel.BrandName = DBItem.BrandName;
                viewModel.BrandNameEng = DBItem.BrandNameEng;
            }
            viewModel.BrandImageFilename = DBItem.BrandImageFilename;
            viewModel.BrandImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.BrandImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Brands.Brand.DeleteCoverImage, new { brandID = DBItem.BrandID });

            return viewModel;
        }

        public async Task Save(PageViewModel viewModel)
        {
            var repository = RepositoriesFactory.GetBrandsRepository();

            var hasBrandImage = viewModel.BrandImageFile?.Length > 0;
            var brandImageFilename = hasBrandImage ? GetFilenameFromUploadedFile(viewModel.BrandImageFile) : null;

            if (hasBrandImage)
            {
                await DeleteUploadedFile(viewModel.BrandImageFilename, folderPath: null);
            }

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

            if (!repository.IsError)
            {
                viewModel.IsSaved = true;
                if (hasBrandImage)
                {
                    await SaveUploadedFile(viewModel.BrandImageFile, brandImageFilename, folderPath: null);
                }
            }
        }

        public void ValidatePageViewModel(PageViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.BrandName)), valueToValidate: viewModel.BrandName));
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetBrandsRepository();

            await DeleteUploadedFile(DBItem.BrandImageFilename, folderPath: null);

            await repository.BrandsIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                brandID: null,
                brand: new BrandIudDTO
                {
                    BrandID = DBItem.BrandID,
                    BrandImageFilename = Constants.NullValueFor.String
                }                
            );

            viewModel.IsSuccess = !repository.IsError;

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class PageViewModel : FormViewModelBase
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
