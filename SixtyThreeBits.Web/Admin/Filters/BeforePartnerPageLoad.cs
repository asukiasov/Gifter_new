using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforePartnerPageLoad : IAsyncActionFilter
    {
        #region Properties
        PartnersModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<PartnersModelBase>(filterContext.Controller);
            var partnerID = filterContext.RouteData.Values[Constants.RouteValues.PartnerID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetPartnersRepository();
            _model.DBItem = await repository.PartnersGetSingleByID(partnerID);
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
            _model.PageTitle.Set(_model.DBItem.PartnerName);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.PartnerName);
        } 
        #endregion
    }
}