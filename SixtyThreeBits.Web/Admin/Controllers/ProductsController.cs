using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/product-categories")]
    public class CategoriesController : AdminControllerBase<ProductCategoriesModel>
    {
        #region Constructors
        public CategoriesController()
        {
            Model = new ProductCategoriesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategories.Index)]
        public async Task<IActionResult> Categories()
        {
            Model.PluginsClient.EnableJQueryUI(EnableJs: true).EnableJQueryNestedSortable(true).EnableTemplate7(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.ProductCategories.Page, ViewModel);
        }

        [Route("add", Name = ControllerActionRouteNames.Admin.ProductCategories.Add)]
        public async Task<IActionResult> Create(int? ProductCategoryParentID, string ProductCategoryName)
        {
            var ViewModel = await Model.CreateProductCategory(ProductCategoryParentID, ProductCategoryName);
            return Json(ViewModel);
        }

        [Route("sort", Name = ControllerActionRouteNames.Admin.ProductCategories.Sort)]
        public async Task<IActionResult> Sort(SyncSortIndexesModel SubmitModel)
        {
            var ViewModel = await Model.SyncParentsAndSortIndexes(SubmitModel);
            return Json(ViewModel);
        }

        [Route("delete", Name = ControllerActionRouteNames.Admin.ProductCategories.Delete)]
        public async Task<IActionResult> Delete(int? ProductCategoryID)
        {
            var ViewModel = await Model.DeleteRecursive(ProductCategoryID);
            return Json(ViewModel);
        }
        #endregion
    }

    [Route("admin/product-categories/{ProductCategoryID:int}/properties")]
    [TypeFilter(typeof(BeforeProductCategoryPageLoad), Order = 2)]
    public class CategoriesPropertiesController : AdminControllerBase<CategoryPropertiesModel>
    {
        #region Constructors
        public CategoriesPropertiesController()
        {
            Model = new CategoryPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.Properties)]
        public IActionResult Properties(int? ProductCategoryID)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(ProductCategoryID, ViewModel: null);
            if (ViewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, ViewModel);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(CategoryPropertiesModel.ProductCategoryPropertiesViewModel SubmitModel, int? ProductCategoryID)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetPageViewModel(ProductCategoryID, SubmitModel);
            Model.ValidatePageViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveCategoryProperties(ProductCategoryID, ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.Properties, new { ProductCategoryID = ProductCategoryID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.ProductCategories.ProductCategoryProperties, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("image/delete", Name = ControllerActionRouteNames.Admin.ProductCategories.ProductCategory.ImageDelete)]
        public async Task<IActionResult> CategoryDeleteImage(int? ProductCategoryID)
        {
            var Result = await Model.DeleteImage(ProductCategoryID);
            return Json(Result);
        }
        #endregion
    }

    [Route("admin/products")]
    public class ProductsController : AdminControllerBase<ProductsModel>
    {
        #region Constructors
        public ProductsController()
        {
            Model = new ProductsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Products.Index)]
        public async Task<ActionResult> Products()
        {
            Model.PluginsClient.EnableDevextreme(true).Enable63BitsForms(true).EnableTemplate7(true);
            var ViewModel = await Model.GetPageViewModel();
            return View(ViewNames.Admin.Products.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Products.Grid)]
        public async Task<ActionResult> ProductsGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Products.GridAdd)]
        public async Task<ActionResult> ProductsGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<ProductsModel.PageViewModel.GridModel.GridItem>() ?? new ProductsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, ProductID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Products.GridUpdate)]
        public async Task<ActionResult> ProductsGridUpdate(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<ProductsModel.PageViewModel.GridModel.GridItem>() ?? new ProductsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, ProductID: key, SubmitModel: SubmitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Products.GridDelete)]
        public async Task<ActionResult> ProductsGridDelete(int? key)
        {
            
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, ProductID: key, SubmitModel: new ProductsModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [Route("excel/download", Name = ControllerActionRouteNames.Admin.Products.ExcelDownload)]
        public async Task<IActionResult> ExcelDownload()
        {
            var ExcelFileBytes = await Model.GetProductsSyncExcelFileBytes();
            return File(ExcelFileBytes, "application/force-download", "ProductsSync.xlsx");
        }

        [HttpPost]
        [Route("excel/upload", Name = ControllerActionRouteNames.Admin.Products.ExcelUpload)]
        public async Task<IActionResult> ExcelSync(byte[] ExcelFileBytes, string ExcelFilename)
        {
            var ViewModel = await Model.SyncExcel(ExcelFileBytes, ExcelFilename);
            return Json(ViewModel);
        }
        #endregion
    }

    [Route("admin/products/{ProductID:int}/properties")]
    [TypeFilter(typeof(BeforeProductPageLoad), Order = 2)]    
    public class ProductsPropertiesController : AdminControllerBase<ProductPropertiesModel>
    {
        #region Constructors
        public ProductsPropertiesController()
        {
            Model = new ProductPropertiesModel();                
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Products.Product.Properties)]
        public async Task<IActionResult> Properties(int? ProductID)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableJQueryUI(true);
            var ViewModel = await Model.GetPageViewModel(ProductID, ViewModel: null);
            if (ViewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.Products.Product, ViewModel);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Properties(int? ProductID, ProductPropertiesModel.ProductsPropertiesViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableJQueryNumericInput(true).EnableTemplate7(true).EnableJQueryUI(true);
            var ViewModel = await Model.GetPageViewModel(ProductID, SubmitModel);
            Model.ValidatePageViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveProductsProperties(ProductID, ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Products.Product.Properties, new { ProductID = ProductID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Products.Product, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Products.Product, ViewModel);
            }
            return Result;
        }        

        [HttpPost]
        [Route("images/upload", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesUpload)]
        public async Task<IActionResult> PropertiesImagesUpload(int? ProductID)
        {
            await Task.Delay(2000);
            var ViewModel = await Model.UploadImages(ProductID);
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("images/sort", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesSort)]
        public async Task<IActionResult> PropertiesImagesSort(int? ProductID, SyncSortIndexesModel SubmitModel)
        {
            var ViewModel = await Model.SortImages(ProductID, SubmitModel);
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("images/delete", Name = ControllerActionRouteNames.Admin.Products.Product.PropertiesImagesDelete)]
        public async Task<IActionResult> PropertiesImagesDelete(int? ProductID, int? ProductImageID)
        {
            var ViewModel = await Model.DeleteImage(ProductID, ProductImageID);
            return Json(ViewModel);
        }        
        #endregion      
    }    
}