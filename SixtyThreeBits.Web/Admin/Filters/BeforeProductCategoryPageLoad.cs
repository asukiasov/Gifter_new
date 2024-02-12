using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeProductCategoryPageLoad: IAsyncActionFilter
    {
        #region Properties
        ProductsCategoriesModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = LocalUtilities.GetModelFromController<ProductsCategoriesModelBase>(filterContext.Controller);
            var productCategoryID = filterContext.RouteData.Values[Constants.RouteValues.ProductCategoryID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetProductsRepository();
            _model.DBItem = await repository.ProductCategoriesGetSingleByID(productCategoryID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                initPageTitle();
                reinitBreadCrumbs();
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
