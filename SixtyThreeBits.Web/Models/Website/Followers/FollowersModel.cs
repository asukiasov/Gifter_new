using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;

namespace SixtyThreeBits.Web.Models.Website
{
    public class FollowersModel : ModelBase
    {
        #region Methods
        public async Task<AjaxResponse> Follow(int followedUserID)
        {
            var viewModel = new AjaxResponse();

            if (User?.UserID == null)
            {
                viewModel.Data = "Authentication required";
                return viewModel;
            }

            if (User.UserID.Value == followedUserID)
            {
                viewModel.Data = "You cannot follow yourself";
                return viewModel;
            }

            var repository = RepositoriesFactory.CreateFollowersRepository();
            await repository.FollowersFollow(followingUserID: User.UserID.Value, followedUserID: followedUserID);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            if (!repository.IsError)
            {
                var ordersRepository = RepositoriesFactory.CreateOrdersRepository();
                await ordersRepository.OrdersInsert(
                    orderUserID: User.UserID.Value,
                    orderType: 3,
                    orderTargetUserID: followedUserID
                );
            }

            return viewModel;
        }

        public async Task<AjaxResponse> Unfollow(int followedUserID)
        {
            var viewModel = new AjaxResponse();

            if (User?.UserID == null)
            {
                viewModel.Data = "Authentication required";
                return viewModel;
            }

            var repository = RepositoriesFactory.CreateFollowersRepository();
            await repository.FollowersUnfollow(followingUserID: User.UserID.Value, followedUserID: followedUserID);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> IsFollowing(int followedUserID)
        {
            var viewModel = new AjaxResponse();

            if (User?.UserID == null)
            {
                viewModel.Data = "Authentication required";
                return viewModel;
            }

            var repository = RepositoriesFactory.CreateFollowersRepository();
            var isFollowing = await repository.FollowersIsFollowing(followingUserID: User.UserID.Value, followedUserID: followedUserID);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = isFollowing;

            return viewModel;
        }

        public async Task<AjaxResponse> GetFollowing()
        {
            var viewModel = new AjaxResponse();

            if (User?.UserID == null)
            {
                viewModel.Data = "Authentication required";
                return viewModel;
            }

            var repository = RepositoriesFactory.CreateFollowersRepository();
            var list = await repository.FollowersList(followingUserID: User.UserID.Value);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : (object)list;

            return viewModel;
        }

        public async Task<AjaxResponse> GetMyFollowers()
        {
            var viewModel = new AjaxResponse();

            if (User?.UserID == null)
            {
                viewModel.Data = "Authentication required";
                return viewModel;
            }

            var repository = RepositoriesFactory.CreateFollowersRepository();
            var list = await repository.FollowersMyFollowers(followedUserID: User.UserID.Value);

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : (object)list;

            return viewModel;
        }
        #endregion
    }
}
