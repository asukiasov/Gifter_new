using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforePartnerPageLoad : IAsyncActionFilter
    {

        public BeforePartnerPageLoad()
        {
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<PartnersModelBase>(FilterContext.Controller);
            var PartnerID = FilterContext.RouteData.Values[Constants.RouteValues.PartnersID].ToString().ToInt();

            Model.DBItemPartner = await Model.DataAccessFactory.Partners.GetSinglePartnerByID(PartnerID);
            if (Model.DBItemPartner == null)
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

        void InitPageTitle(PartnersModelBase Model)
        {
            Model.PageTitle.Set(Model.DBItemPartner.PartnerName);
        }

        void ReinitBreadCrumbs(PartnersModelBase Model)
        {
            Model.Breadcrumbs.RenameLastItem(Model.DBItemPartner.PartnerName);
        }
    }
}