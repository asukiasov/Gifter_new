using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries.DevExtreme;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class UserFollowersModel : UserModelBase
    {
        #region Methods
        public async Task<ViewModel> GetViewModel()
        {
            var viewModel = new ViewModel();

            viewModel.Grid = new ViewModel.GridModel();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.UserFollowersController.FollowersGrid, new { userID = dbItem.UserID });

            return await Task.FromResult(viewModel);
        }

        public async Task<AjaxResponse> GetGridItems()
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateFollowersRepository();

            var followers = await repository.FollowersMyFollowers(dbItem.UserID.Value);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : followers.Select(item => new ViewModel.GridModel.GridItem
            {
                FollowerID = item.FollowerID,
                FollowingUserFullname = item.FollowingUserFullname,
                FollowingUserEmail = item.FollowingUserEmail,
                FollowerDateCreated = item.FollowerDateCreated
            }).ToList();

            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel : LayoutViewModelBase
        {
            #region Properties
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.FollowerID));

                    grid
                    .ID("FollowersGrid")
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.FollowerID).Caption("#").Width(60);
                        columns.AddFor(m => m.FollowingUserFullname).Caption("Follower").Width(250);
                        columns.AddFor(m => m.FollowingUserEmail).Caption("Email").Width(300);
                        columns.AddFor(m => m.FollowerDateCreated).Caption("Since").Width(180).InitDateColumn(format: DevExtremeExtensions63.DateColumnFormat.DateTime);
                        columns.Add();
                    })
                    .OnInitialized("model.onGridInit");

                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? FollowerID { get; set; }
                    public string FollowingUserFullname { get; set; }
                    public string FollowingUserEmail { get; set; }
                    public DateTime? FollowerDateCreated { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
