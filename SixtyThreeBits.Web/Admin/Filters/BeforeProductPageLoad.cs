using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeProductPageLoad : IAsyncActionFilter
    {        

        public BeforeProductPageLoad()
        {            
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<ProductsModelBase>(FilterContext.Controller);
            var ProductID = FilterContext.RouteData.Values["ProductID"].ToString().ToInt();

            Model.DBItemProduct = await Model.DataAccessFactory.Products.GetSingleProductByID(ProductID);
            if (Model.DBItemProduct == null)
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

        void InitPageTitle(ProductsModelBase Model)
        {
            Model.PageTitle.Set(Model.DBItemProduct.ProductName);
        }

        void ReinitBreadCrumbs(ProductsModelBase Model)
        {            
            Model.Breadcrumbs.RenameLastItem(Model.DBItemProduct.ProductName);
        }
    }
}