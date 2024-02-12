using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeBlogPageLoad : IAsyncActionFilter
    {
        #region Properties
        BlogModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<BlogModelBase>(filterContext.Controller);
            var blogPostID = filterContext.RouteData.Values[Constants.RouteValues.BlogPostID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetBlogRepository();
            _model.DBItem = await repository.BlogPostGetSingleByID(blogPostID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                reinitBreadCrumbs();
                await next();
            }
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.DeleteLastItem();
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.BlogPostTitle);
        }  
        #endregion
    }
}