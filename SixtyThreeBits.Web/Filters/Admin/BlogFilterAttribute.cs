using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BlogFilterAttribute : IAsyncActionFilter
    {
        #region Properties
        BlogModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<BlogModelBase>(filterContext.Controller);
            var blogPostID = filterContext.RouteData.Values[WebConstants.RouteValues.BlogPostID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.CreateBlogRepository();
            _model.DBItem = await repository.BlogPostGetSingleByID(blogPostID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                if (!_model.IsAjaxRequest)
                {
                    reinitBreadCrumbs();
                }
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