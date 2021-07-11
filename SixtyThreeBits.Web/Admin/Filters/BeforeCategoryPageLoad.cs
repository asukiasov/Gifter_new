using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeCategoryPageLoad: IAsyncActionFilter
    {
        public BeforeCategoryPageLoad()
        {

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<CategoriesModelBase>(FilterContext.Controller);
            var CategoryID = FilterContext.RouteData.Values["CategoryID"].ToString().ToInt();

            Model.DBItemCategories = await Model.DataAccessFactory.Categories.GetSingleCategoryByID(CategoryID);
            if (Model.DBItemCategories == null)
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
            Model.PageTitle.Set(Model.DBItemCategories.CategoryName);
        }

        void ReinitBreadCrumbs(CategoriesModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
            Model.Breadcrumbs.RenameLastItem(Model.DBItemCategories.CategoryName);
        }
        
    }
}
