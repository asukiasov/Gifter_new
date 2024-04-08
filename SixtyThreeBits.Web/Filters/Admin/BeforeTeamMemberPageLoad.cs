using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Filters.Admin
{
    public class BeforeTeamMemberPageLoad : IAsyncActionFilter
    {
        #region Properties
        TeamMembersModelBase _model;
        #endregion

        #region Methods
        public async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            _model = WebUtilities.GetModelFromController<TeamMembersModelBase>(filterContext.Controller);
            var teamMemberID = filterContext.RouteData.Values[WebConstants.RouteValues.TeamMemberID]?.ToString().ToInt();

            var repository = _model.RepositoriesFactory.GetTeamMembersRepository();
            _model.DBItem = await repository.TeamMembersGetSingleByID(teamMemberID);
            if (_model.DBItem == null)
            {
                filterContext.Result = _model.GetNotFoundAdminViewResult();
            }
            else
            {
                if (!_model.IsAjaxRequest)
                {
                    initPageTitle();
                    reinitBreadCrumbs();
                }
                await next();
            }
        }

        void initPageTitle()
        {
            _model.PageTitle.Set(_model.DBItem.TeamMemberFullname);
        }

        void reinitBreadCrumbs()
        {
            _model.Breadcrumbs.RemoveAt(2);
            _model.Breadcrumbs.RenameLastItem(_model.DBItem.TeamMemberFullname);
        }
        #endregion
    }
}
