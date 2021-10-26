using Microsoft.AspNetCore.Mvc.Filters;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Filters
{
    public class BeforeTeamMemberPageLoad : IAsyncActionFilter
    {
        #region Constructors
        #endregion
        public BeforeTeamMemberPageLoad()
        {
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext FilterContext, ActionExecutionDelegate next)
        {
            var Model = LocalUtilities.GetModelFromController<TeamMembersModelBase>(FilterContext.Controller);
            var TeamMemberID = FilterContext.RouteData.Values["TeamMemberID"].ToString().ToInt();

            Model.DBItemTeamMember = await Model.DataAccessFactory.TeamMembers.GetSingleTeamMemberID(TeamMemberID);
            if (Model.DBItemTeamMember == null)
            {
                FilterContext.Result = Model.GetNotFoundAdminViewResult();
            }
            else
            {
                await next();
            }
        }
    }
}
