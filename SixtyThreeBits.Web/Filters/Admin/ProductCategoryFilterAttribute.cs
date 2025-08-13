using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class ProductCategoryFilterAttribute : IAsyncActionFilter
    {
        #region Properties
        ProductCategoryModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<ProductCategoryModelBase>(filterContext.Controller);
            var productCategoryID = filterContext.RouteData.Values[WebConstants.RouteValues.ProductCategoryID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.CreateProductsRepository();
            _model.DBItem = await repository.ProductCategoriesGetSingleByID(productCategoryID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                if (!_model.IsAjaxRequest)
                {
                    initPageTitle();
                    reinitBreadCrumbs();
                }
                await next();
            }
        }

        void initPageTitle()
        {
            _model.PageTitle.Set(_model.DBItem.ProductCategoryName);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.ProductCategoryName);
        }
        #endregion
    }
}
