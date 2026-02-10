using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class DashboardModel : ModelBase
    {
        #region Properties
        public bool ShowWelcomeModal => User?.UserIsFirstLogin ?? false;
        public List<GiftListsListDTO> Wishlists { get; set; } = new List<GiftListsListDTO>();
        #endregion

        #region Methods
        public async Task<bool> CompleteOnboardingAsync()
        {
            if (User?.UserID == null)
            {
                return false;
            }

            var usersRepository = RepositoriesFactory.CreateUsersRepository();
            await usersRepository.UsersCompleteOnboarding(User.UserID.Value);

            // Refresh the user session with updated data
            var updatedUser = await usersRepository.UsersGetSingleByID(User.UserID.Value);
            if (updatedUser != null)
            {
                SessionAssistance.Set(WebConstants.SessionKeys.User, updatedUser);
            }

            return true;
        }

        public async Task LoadWishlistsAsync()
        {
            if (User?.UserID == null)
            {
                return;
            }

            try
            {
                var giftListsRepository = RepositoriesFactory.CreateGiftListsRepository();
                Wishlists = await giftListsRepository.GiftListsListByUserID(User.UserID.Value) ?? new List<GiftListsListDTO>();
            }
            catch
            {
                Wishlists = new List<GiftListsListDTO>();
            }
        }

        public async Task<int?> CreateWishlistAsync(string title, string occasionType)
        {
            if (User?.UserID == null)
            {
                return null;
            }

            var giftListsRepository = RepositoriesFactory.CreateGiftListsRepository();
            var newWishlist = new GiftListIudDTO
            {
                GiftListUserID = User.UserID.Value,
                GiftListTitle = title,
                GiftListOccasionType = occasionType,
                GiftListIsSecret = false,
                GiftListIsPublished = false
            };

            var giftListID = await giftListsRepository.GiftListsIUD(
                databaseAction: Enums.DatabaseActions.INSERT,
                giftListID: null,
                giftList: newWishlist
            );

            return giftListID;
        }
        #endregion
    }
}
