using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Admin;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class ProductCategoriesModel : ModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.ProductCategoriesController.Add);
            viewModel.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Add);
            viewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Delete);
            viewModel.UrlSort = Url.RouteUrl(ControllerActionRouteNames.Admin.ProductCategoriesController.Sort);

            var repository = RepositoriesFactory.CreateProductsRepository();
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

        public async Task<AjaxResponse> Add(ProductCategoryCreateSubmitModel submitModel)
        {
            TreeNodeItem node = null;

            var repository = RepositoriesFactory.CreateProductsRepository();
            var productCategoryID = await repository.ProductCategoriesIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
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

        public async Task<AjaxResponse> Sort(SyncSortIndexesSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();
            await repository.ProductCategoriesSyncParentsAndSortIndexes(submitModel.SortIndexes);
            viewModel.IsSuccess = !repository.IsError;
            return viewModel;
        }

        public async Task<AjaxResponse> Delete(ProductCategoryDeleteSubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateProductsRepository();
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
}