using DevExpress.Pdf.Native;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Admin;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductsCategoriesModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Add);
            viewModel.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Add);
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Delete);
            viewModel.UrlSort = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Sort);

            var repository = RepositoriesFactory.GetProductsRepository();
            viewModel.ProductCategories = (await repository.ProductCategoriesList())
            ?.Select(item => new TreeNodeItem
            {
                NodeID = item.ProductCategoryID.ToString(),
                ParentID = item.ProductCategoryParentID.HasValue ? item.ProductCategoryParentID.ToString() : null,
                Caption = item.ProductCategoryName,
                NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.Properties, new { productCategoryID = item.ProductCategoryID }),
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Add) && item.ProductCategoryParentID == null,
                ShowDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Delete)
            })
            .ToList();
            if (viewModel.HasCategories)
            {
                viewModel.ProductCategories.ToRecursive(IDPropertyName: nameof(TreeNodeItem.NodeID), nameof(TreeNodeItem.ParentID), nameof(TreeNodeItem.Children));
            }
            return viewModel;
        }

        public async Task<AjaxResponse> DeleteRecursive(ProductCategoryDeleteSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductCategoriesDeleteRecursive(submitModel.ProductCategoryID);
            if(repository.IsError)
            {
                viewModel.Data = repository.ErrorMessage;
            }
            else
            {
                viewModel.IsSuccess = true;
            }
            
            return viewModel;
        }

        public async Task<AjaxResponse> CreateProductCategory(ProductCategoryCreateSubmitModel submitModel)
        {
            TreeNodeItem node = null;

            var repository = RepositoriesFactory.GetProductsRepository();
            var productCategoryID = await repository.ProductCategoriesIUD(
                databaseAction: Enums.DatabaseActions.CREATE,
                productCategoryID: null,
                productCategory: new ProductCategoryIudDTO
                {
                    ProductCategoryParentID = submitModel.ProductCategoryParentID,
                    ProductCategoryName = submitModel.ProductCategoryName
                }                
            );

            if (productCategoryID > 0)
            {
                node = new TreeNodeItem();
                node.NodeID = productCategoryID.ToString();
                node.ParentID = submitModel.ProductCategoryParentID.HasValue ? submitModel.ProductCategoryParentID.ToString() : null;
                node.Caption = submitModel.ProductCategoryName;
                node.NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.Properties, new { productCategoryID = productCategoryID });
                node.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Add) && submitModel.ProductCategoryParentID is null;
                node.ShowDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Delete);
            }

            var viewModel = new AjaxResponse();

            if (node != null)
            {
                viewModel.IsSuccess = true;
                viewModel.Data = node;
            }

            return viewModel;
        }

        public async Task<AjaxResponse> SyncParentsAndSortIndexes(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductCategoriesSyncParentsAndSortIndexes(submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool HasCategories => ProductCategories != null && ProductCategories.Count > 0;
            public List<TreeNodeItem> ProductCategories { get; set; }
            public bool ShowAddNewButton { get; set; }
            public string UrlAddNew { get; set; }
            public string UrlDelete { get; set; }
            public string UrlSort { get; set; }
            public readonly string TextConfirmDeleteRecord = Resources.TextConfirmDelete;
            public readonly string TextConfirmDeleteRecursive = Resources.TextConfirmDeleteRecursive;
            public readonly string TextAdd = Resources.TextAdd;
            public readonly string TextCaption = Resources.TextCaption;
            #endregion
        }

        public class ProductCategoryCreateSubmitModel
        {
            #region Properties
            public int? ProductCategoryParentID { get; set; }
            public string ProductCategoryName { get; set; }
            #endregion
        }

        public class ProductCategoryDeleteSubmitModel
        {
            #region Properties
            public int? ProductCategoryID { get; set; }
            #endregion
        }
        #endregion
    }

    public class ProductsCategoriesModelBase : ModelBase
    {
        #region Properties
        public ProductCategoryDTO DBItem { get; set; }
        #endregion
    }

    public class ProductCategoryPropertiesModel : ProductsCategoriesModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();

                viewModel.ProductCategoryParentID = DBItem.ProductCategoryParentID;
                viewModel.ProductCategoryName = DBItem.ProductCategoryName;
                viewModel.ProductCategoryNameEng = DBItem.ProductCategoryNameEng;
                viewModel.ProductCategoryDescriptionShort = DBItem.ProductCategoryDescriptionShort;
                viewModel.ProductCategoryDescriptionShortEng = DBItem.ProductCategoryDescriptionShortEng;

            }
            viewModel.ProductCategoryImageFilename = DBItem.ProductCategoryImageFilename;
            viewModel.ProductCategoryImageHttpPath = FileStorage.GetUploadedFileHttpPath(DBItem.ProductCategoryImageFilename);
            viewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoryPropertiesController.DeleteImage, new { productCategoryID = DBItem.ProductCategoryID });

            return viewModel;
        }

        public async Task Save(ViewModel viewModel)
        {
            var hasCategoryImage = viewModel.ProductCategoryImageFile?.Length > 0;
            var categoryImageFilename = hasCategoryImage ? GetFilenameFromUploadedFile(viewModel.ProductCategoryImageFile) : null;

            if (hasCategoryImage)
            {
                await DeleteUploadedFile(viewModel.ProductCategoryImageFilename);
            }

            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductCategoriesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,                
                productCategoryID: DBItem.ProductCategoryID,
                productCategory: new ProductCategoryIudDTO
                {
                    ProductCategoryParentID = viewModel.ProductCategoryParentID,
                    ProductCategoryName = viewModel.ProductCategoryName,
                    ProductCategoryNameEng = viewModel.ProductCategoryNameEng ?? Constants.NullValueFor.String,
                    ProductCategoryImageFilename = categoryImageFilename,
                    ProductCategoryDescriptionShort = viewModel.ProductCategoryDescriptionShort ?? Constants.NullValueFor.String,
                    ProductCategoryDescriptionShortEng = viewModel.ProductCategoryDescriptionShortEng ?? Constants.NullValueFor.String
                }
            );

            if (repository.IsError)
            {
                viewModel.AddError(repository.ErrorMessage);
            }
            else
            {
                if (hasCategoryImage)
                {
                    await SaveUploadedFile(viewModel.ProductCategoryImageFile, categoryImageFilename);
                }
            }
        }

        public void Validate(ViewModel viewModel)
        {
            viewModel.AddError(Validation.ValidateRequired(errorKey: Validation.GetJQueryNameSelectorFor(nameof(viewModel.ProductCategoryName)), valueToValidate: viewModel.ProductCategoryName));
        }

        public async Task<AjaxResponse> DeleteImage()
        {
            var viewModel = new AjaxResponse();

            await DeleteUploadedFile(DBItem.ProductCategoryImageFilename);

            var repository = RepositoriesFactory.GetProductsRepository();
            await repository.ProductCategoriesIUD(
                databaseAction: Enums.DatabaseActions.UPDATE,
                productCategoryID: DBItem.ProductCategoryID,
                productCategory: new ProductCategoryIudDTO
                {
                    ProductCategoryImageFilename = Constants.NullValueFor.String
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
            public int? ProductCategoryID { get; set; }
            public int? ProductCategoryParentID { get; set; }
            public string ProductCategoryName { get; set; }
            public string ProductCategoryNameEng { get; set; }
            public string CategoryImageFilenameProduct { get; set; }
            public string ProductCategoryDescriptionShort { get; set; }
            public string ProductCategoryDescriptionShortEng { get; set; }
            public string ProductCategoryImageFilename { get; set; }
            public string ProductCategoryImageHttpPath { get; set; }
            public bool HasProductCategoryImage => !string.IsNullOrWhiteSpace(ProductCategoryImageFilename);
            public string UrlDeleteImage { get; set; }
            public IFormFile ProductCategoryImageFile { get; set; }

            public readonly string TextName = Resources.TextName;
            public readonly string TextNameEng = Resources.TextNameEng;
            public readonly string TextDescriptionShort = Resources.TextDescriptionShort;
            public readonly string TextDescriptionShortEng = Resources.TextDescriptionShortEng;
            public readonly string TextUploadImage = Resources.TextUploadImage;
            #endregion
        }
        #endregion
    }
}