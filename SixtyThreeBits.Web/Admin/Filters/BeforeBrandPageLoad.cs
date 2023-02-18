using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeBrandPageLoad: IAsyncActionFilter
    {
        public BeforeBrandPageLoad()
        {           
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<BrandsModelBase>(FilterContext.Controller);
            var BrandID = FilterContext.RouteData.Values[Constants.RouteValues.BrandID].ToString().ToInt();

            Model.DBItemBrands = await Model.DataAccessFactory.Brands.GetSingleBrandByID(BrandID);
            if (Model.DBItemBrands == null)
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

        void InitPageTitle(BrandsModelBase Model)
        {
            Model.PageTitle.Set(Model.DBItemBrands.BrandName);
        }

        void ReinitBreadCrumbs(BrandsModelBase Model)
        {
            Model.Breadcrumbs.DeleteLastItem();
            Model.Breadcrumbs.RenameLastItem(Model.DBItemBrands.BrandName);
        }
    }
}
