using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
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

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.Grid);
            viewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridAdd);
            viewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridUpdate);
            viewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandsController.GridDelete);
            viewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridAdd);
            viewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridUpdate);
            viewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.BrandsController.GridDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateBrandsRepository();

            var brands = (await repository.BrandsList());

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : brands.Select(item => new ViewModel.GridModel.GridItem
            {
                BrandID = item.BrandID,
                BrandName = item.BrandName,
                BrandNameEng = item.BrandNameEng,
                UrlBrandProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.BrandPropertiesController.Properties, new { brandID = item.BrandID })
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? brandID, ViewModel.GridModel.GridItem submitModel)
        {
            var viewModel = new AjaxResponse();

            await iudProcessBrandImageFilename(databaseAction: databaseAction, brandID: brandID);

            var repository = RepositoriesFactory.CreateBrandsRepository();
            await repository.BrandsIUD(
                databaseAction: databaseAction,
                brandID: brandID,
                brand: new BrandIudDTO
                {
                    BrandName = submitModel.BrandName,
                    BrandNameEng = submitModel.BrandNameEng ?? Constants.NullValueFor.String
                }
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }
        async Task iudProcessBrandImageFilename(Enums.DatabaseActions databaseAction, int? brandID)
        {
            if (databaseAction == Enums.DatabaseActions.DELETE)
            {
                var repository = RepositoriesFactory.CreateBrandsRepository();
                var brand = await repository.BrandsGetSingleByID(brandID);
                await FileStorage.DeleteFile(brand.BrandImageFilename);
            }
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.BrandID));

                    grid
                      .ID("BrandsGrid")
                      .OnInitialized("model.onGridInit")
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
}
