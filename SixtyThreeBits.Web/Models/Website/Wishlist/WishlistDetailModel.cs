using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using SixtyThreeBits.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Website
{
    public class WishlistDetailModel : ModelBase
    {
        #region Nested Classes
        public class AddProductRequest
        {
            public string Title { get; set; }
            public decimal? Price { get; set; }
            public string Currency { get; set; }
            public string Url { get; set; }
            public string ImageUrl { get; set; }
        }

        public class AddProductResponse
        {
            public bool Success { get; set; }
            public int? GiftID { get; set; }
            public string Error { get; set; }
        }
        #endregion

        #region Properties
        public int GiftListID { get; set; }
        public GiftListsListDTO Wishlist { get; set; }
        public List<GiftsListDTO> Gifts { get; set; } = new List<GiftsListDTO>();
        public bool IsOwner => User?.UserID != null && Wishlist?.GiftListUserID == User.UserID;
        public bool NotFound { get; set; }
        #endregion

        #region Methods
        public async Task LoadAsync(int giftListID)
        {
            GiftListID = giftListID;

            try
            {
                var giftListsRepository = RepositoriesFactory.CreateGiftListsRepository();
                Wishlist = await giftListsRepository.GiftListsGetSingleByID(giftListID);

                if (Wishlist == null)
                {
                    NotFound = true;
                    return;
                }

                var giftsRepository = RepositoriesFactory.CreateGiftsRepository();
                Gifts = await giftsRepository.GiftsListByGiftListID(giftListID) ?? new List<GiftsListDTO>();
            }
            catch
            {
                Wishlist = null;
                Gifts = new List<GiftsListDTO>();
            }
        }

        public async Task<ScraperService.ScrapeResult> ScrapeProductAsync(string url)
        {
            var scraper = new ScraperService();
            return await scraper.ScrapeAsync(url);
        }

        public async Task<AddProductResponse> AddProductAsync(int giftListID, AddProductRequest request)
        {
            var response = new AddProductResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    response.Error = "Product title is required";
                    return response;
                }

                // Verify user owns this wishlist
                var giftListsRepository = RepositoriesFactory.CreateGiftListsRepository();
                var wishlist = await giftListsRepository.GiftListsGetSingleByID(giftListID);

                if (wishlist == null)
                {
                    response.Error = "Wishlist not found";
                    return response;
                }

                if (wishlist.GiftListUserID != User?.UserID)
                {
                    response.Error = "You don't have permission to add products to this wishlist";
                    return response;
                }

                var giftsRepository = RepositoriesFactory.CreateGiftsRepository();
                var giftId = await giftsRepository.GiftsIUD(
                    databaseAction: Enums.DatabaseActions.INSERT,
                    giftID: null,
                    gift: new GiftIudDTO
                    {
                        GiftGiftListID = giftListID,
                        GiftTitle = request.Title,
                        GiftPrice = request.Price,
                        GiftCurrency = request.Currency ?? "USD",
                        GiftUrl = request.Url,
                        GiftImageUrl = request.ImageUrl
                    }
                );

                response.Success = giftId.HasValue;
                response.GiftID = giftId;

                if (!response.Success)
                {
                    response.Error = "Failed to add product";
                }

                return response;
            }
            catch (System.Exception ex)
            {
                response.Error = $"An error occurred: {ex.Message}";
                return response;
            }
        }
        #endregion
    }
}
