using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeNewsPageLoad : IAsyncActionFilter
    {
        #region Properties
        NewsModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = LocalUtilities.GetModelFromController<NewsModelBase>(filterContext.Controller);
            var newsID = filterContext.RouteData.Values[Constants.RouteValues.NewsID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetNewsRepository();
            _model.DBItem = await repository.NewsGetSingleByID(newsID);
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
            _model.PageTitle.Set(_model.DBItem.NewsTitle);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.NewsTitle);
        } 
        #endregion
    }
}