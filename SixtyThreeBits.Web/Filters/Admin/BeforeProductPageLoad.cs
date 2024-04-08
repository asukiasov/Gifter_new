using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
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
            var productID = filterContext.RouteData.Values[WebConstants.RouteValues.ProductID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetProductsRepository();
            _model.DBItem = await repository.ProductsGetSingleByID(productID);
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