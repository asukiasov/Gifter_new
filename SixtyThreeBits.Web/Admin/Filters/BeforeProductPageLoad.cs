using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeProductPageLoad : IAsyncActionFilter
    {
        #region Properties
        ProductsModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<ProductsModelBase>(filterContext.Controller);
            var productID = filterContext.RouteData.Values[Constants.RouteValues.ProductID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetProductsRepository();
            _model.DBItem = await repository.ProductsGetSingleByID(productID);
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
            _model.PageTitle.Set(_model.DBItem.ProductName);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.RemoveAt(2);
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.ProductName);
        } 
        #endregion
    }
}