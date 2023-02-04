using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeProductCategoryPageLoad: IAsyncActionFilter
    {
        public BeforeProductCategoryPageLoad()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<CategoriesModelBase>(FilterContext.Controller);
            var ProductCategoryID = FilterContext.RouteData.Values[Constants.RouteValues.ProductCategoryID].ToString().ToInt();

            Model.DBItem = await Model.DataAccessFactory.Products.ProductCategoriesGetSingleByID(ProductCategoryID);
            if (Model.DBItem == null)
            {
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {
                InitPageTitle(Model);                
                ReinitBreadCrumbs(Model);                
                await next();
            }
        }

        void InitPageTitle(CategoriesModelBase Model)
        {
            Model.PageTitle.Set(Model.DBItem.ProductCategoryName);
        }

        void ReinitBreadCrumbs(CategoriesModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
            Model.Breadcrumbs.RenameLastItem(Model.DBItem.ProductCategoryName);
        }
        
    }
}
