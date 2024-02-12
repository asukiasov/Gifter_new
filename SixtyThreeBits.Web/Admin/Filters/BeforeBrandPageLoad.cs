using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeBrandPageLoad: IAsyncActionFilter
    {
        #region Properties
        BrandsModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = LocalUtilities.GetModelFromController<BrandsModelBase>(filterContext.Controller);
            var brandID = filterContext.RouteData.Values[Constants.RouteValues.BrandID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetBrandsRepository();
            _model.DBItem = await repository.BrandsGetSingleByID(brandID);
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
            _model.PageTitle.Set(_model.DBItem.BrandName);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.BrandName);
        } 
        #endregion
    }
}
