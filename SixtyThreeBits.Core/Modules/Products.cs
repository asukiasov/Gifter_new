using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DB;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Modules
{
    public class ProductsDataAccess : DataAccessBase
    {
        #region Constructors
        public ProductsDataAccess(ConnectionFactory ConnectionFactory) : base(ConnectionFactory)
        {

        }
        #endregion

        #region Methods
        public async Task<ProductFilters> GetFilters(string Language, int? CategoryID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetFilters)}({nameof(Language)} = {Language}, {nameof(CategoryID)} = {CategoryID})", async () =>
            {
                using(var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductsFiltersGet(Language, CategoryID);
                    return Result?.DeserializeTo<ProductFilters>();
                }
            });
        }

        public async Task<Product> GetSingleProductByID(int? ProductID)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleProductByID)}({nameof(ProductID)} = {ProductID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductsGetSingleByID(ProductID);
                    return Result?.DeserializeTo<Product>();
                }
            });
        }

        public async Task<Product> GetSingleProductBySlug(string ProductSlug)
        {
            return await TryToReturnAsyncTask($"{nameof(GetSingleProductBySlug)}({nameof(ProductSlug)} = {ProductSlug})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductsGetsingleBySlug(ProductSlug);
                    return Result?.DeserializeTo<Product>();
                }
            });
        }

        public async Task InitListWithBrandIDAndCategoryID(List<Product.ProductSyncItem> ProductSyncItems, DataAccessFactory DataAccessFactory)
        {
            var CategoriesList = await DataAccessFactory.Categories.ListCategories();
            var BrandsList = await DataAccessFactory.Brands.ListBrands();
            var ProductsList = await ListProducts();

            foreach (var Item in ProductSyncItems)
            {
                Item.CategoryID = CategoriesList.FirstOrDefault(CategoryItem => CategoryItem.CategoryName == Item.ProductCategory)?.CategoryID;
            }

            foreach (var Item in ProductSyncItems)
            {
                Item.BrandID = BrandsList.FirstOrDefault(CategoryItem => CategoryItem.BrandName == Item.ProductBrand)?.BrandID;
            }

            foreach (var Item in ProductSyncItems)
            {
                Item.ProductID = ProductsList.FirstOrDefault(ProductItem => ProductItem.ProductName == Item.ProductName)?.ProductID;
            }

        }

        public async Task<bool> IsProductSlugUniq(string ProductSlug, int? ProductID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(IsProductSlugUniq)}({nameof(ProductSlug)} = {ProductSlug}, {nameof(ProductID)} = {ProductID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.ProductsIsSlugUniq(ProductSlug, ProductID);
                }
            });
        }

        public async Task<List<Product>> ListProducts(bool? ProductIsPubliShed = null, bool? ProductIsFeatured = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ListProducts)}({nameof(ProductIsPubliShed)} = {ProductIsPubliShed}, {nameof(ProductIsFeatured)} = {ProductIsFeatured})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.ProductsList(ProductIsPubliShed, ProductIsFeatured).OrderByDescending(Item => Item.ProductDateCreated).ToListAsync())
                    ?.Select(Item => new Product
                    {
                        ProductID = Item.ProductID,
                        BrandID = Item.BrandID,
                        CategoryID = Item.CategoryID,
                        CategorySlug = Item.CategorySlug,
                        CategorySlugParent = Item.CategorySlugParent,
                        ProductSlug = Item.ProductSlug,
                        ProductName = Item.ProductName,
                        ProductCode = Item.ProductCode,
                        ProductNameEng = Item.ProductNameEng,
                        ProductNameRus = Item.ProductNameRus,
                        ProductPrice = Item.ProductPrice,
                        ProductPriceOld = Item.ProductPriceOld,
                        ProductRemainder = Item.ProductRemainder,
                        ProductImageFilename = Item.ProductImageFilename,
                        ProductIsPublished = Item.ProductIsPublished,
                        ProductDescriptionShort = Item.ProductDescriptionShort,
                        ProductDescriptionShortEng = Item.ProductDescriptionShortEng,
                        ProductDescriptionShortRus = Item.ProductDescriptionShortRus,
                        ProductIsFeatured = Item.ProductIsFeatured,
                        ProductDateCreated = Item.ProductDateCreated
                    }).ToList();
                }
            });
        }

        public async Task<List<Product>> ListProductsPager(string Language, int? PageNumber, int? ItemsPerPage, int? SortType, string SearchPhrase, decimal? ProductPriceMin, decimal? ProductPriceMax, bool? IsInStock, bool? HasDiscount, List<string> CategorySlugs, List<string> BrandSlugs, List<string> ProducerCountryCodes)
        {
            return await TryToReturnAsyncTask($"{nameof(ListProductsPager)}({nameof(Language)} = {Language}, {nameof(PageNumber)} = {PageNumber}, {nameof(ItemsPerPage)} = {ItemsPerPage}, {nameof(SortType)} = {SortType}, {nameof(SearchPhrase)} = {SearchPhrase}, {nameof(ProductPriceMin)} = {ProductPriceMin}, {nameof(ProductPriceMax)} = {ProductPriceMax}, {nameof(IsInStock)} = {IsInStock}, {nameof(HasDiscount)} = {HasDiscount}, {nameof(CategorySlugs)} = {CategorySlugs.ToXml()}, {nameof(BrandSlugs)} = {BrandSlugs.ToXml()}, {nameof(ProducerCountryCodes)} = {ProducerCountryCodes.ToXml()})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.ProductsListPager(Language, PageNumber, ItemsPerPage, SortType, SearchPhrase, ProductPriceMin, ProductPriceMax, IsInStock, HasDiscount, CategorySlugs.ToXml(), BrandSlugs.ToXml(), ProducerCountryCodes.ToXml())
                    .ToListAsync())
                    ?.Select(Item => new Product
                    {
                        ProductID = Item.ProductID,
                        ProductSlug = Item.ProductSlug,
                        ProductName = Item.ProductName,
                        ProductNameEng = Item.ProductNameEng,
                        ProductNameRus = Item.ProductNameRus,
                        ProductPrice = Item.ProductPrice,
                        ProductPriceOld = Item.ProductPriceOld,
                        ProductRemainder = Item.ProductRemainder,
                        ProductImageFilename = Item.ProductImageFilename,
                        CategorySlug = Item.CategorySlug,
                        CategorySlugParent = Item.CategorySlugParent,
                    }).ToList();
                }
            });
        }

        public async Task<List<DBCoreDataContext.ProductsImagesListResultItem>> ListProductsImages(int? ProductID)
        {
            return await TryToReturnAsyncTask($"{nameof(ListProductsImages)}({nameof(ProductID)} = {ProductID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.ProductsImagesList(ProductID).OrderBy(Item => Item.ProductImageSortIndex).ToListAsync();
                }
            });
        }

        public async Task ProductsSync(List<Product.ProductSyncItem> ProductSyncItems)
        {
            await TryExecuteAsyncTask($"{nameof(ProductsSync)}({nameof(ProductSyncItems)} = {ProductSyncItems.ToXml()})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.ProductsSync(ProductSyncItems.ToXml());
                }
            });
        }

        public async Task<int?> ProductsImagesIUD(Enums.DatabaseActions DatabaseAction, int? ProductImageID = null, int? ProductID = null, string ProductImageFilename = null, int? ProductImageSyncSortIndex = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsImagesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProductImageID)} = {ProductImageID}, {nameof(ProductID)} = {ProductID}, {nameof(ProductImageFilename)} = {ProductImageFilename}, {nameof(ProductImageSyncSortIndex)} = {ProductImageSyncSortIndex})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    ProductImageID = await db.ProductsImagesIUD(DatabaseAction, ProductImageID, ProductID, ProductImageFilename, ProductImageSyncSortIndex);
                    return ProductImageID;
                }
            });
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions DatabaseAction, int? ProductID = null, int? BrandID = null, int? CategoryID = null, string ProductSlug = null, string ProductCode = null, string ProductName = null, string ProductNameEng = null, string ProductNameRus = null, decimal? ProductPrice = null, decimal? ProductPriceOld = null, decimal? ProductRemainder = null, string ProductImageFilename = null, bool? ProductIsPublished = null, string ProductDescriptionShort = null, string ProductDescriptionShortEng = null, string ProductDescriptionShortRus = null, string ProductDescription = null, string ProductDescriptionEng = null, string ProductDescriptionRus = null, bool? ProductIsFeatured = null, string ProductSKU = null, int? ProductProducerCountryID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProductID)} = {ProductID}, {nameof(BrandID)} = {BrandID}, {nameof(CategoryID)} = {CategoryID}, {nameof(ProductSlug)} = {ProductSlug}, {nameof(ProductCode)} = {ProductCode}, {nameof(ProductName)} = {ProductName}, {nameof(ProductNameEng)} = {ProductNameEng}, {nameof(ProductNameRus)} = {ProductNameRus}, {nameof(ProductPrice)} = {ProductPrice}, {nameof(ProductPriceOld)} = {ProductPriceOld}, {nameof(ProductRemainder)} = {ProductRemainder}, {nameof(ProductImageFilename)} = {ProductImageFilename}, {nameof(ProductIsPublished)} = {ProductIsPublished}, {nameof(ProductDescriptionShort)} = {ProductDescriptionShort}, {nameof(ProductDescriptionShortEng)} = {ProductDescriptionShortEng}, {nameof(ProductDescriptionShortRus)} = {ProductDescriptionShortRus}, {nameof(ProductDescription)} = {ProductDescription}, {nameof(ProductDescriptionEng)} = {ProductDescriptionEng}, {nameof(ProductDescriptionRus)} = {ProductDescriptionRus}, {nameof(ProductIsFeatured)} = {ProductIsFeatured}, {nameof(ProductSKU)} = {ProductSKU}, {nameof(ProductProducerCountryID)} = {ProductProducerCountryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    ProductID = await db.ProductsIUD(DatabaseAction, ProductID, BrandID, CategoryID, ProductCode, ProductName, ProductNameEng, ProductNameRus, ProductPrice, ProductPriceOld, ProductRemainder, ProductImageFilename, ProductIsPublished, ProductDescriptionShort, ProductDescriptionShortEng, ProductDescriptionShortRus, ProductDescription, ProductDescriptionEng, ProductDescriptionRus, ProductIsFeatured, ProductSKU, ProductProducerCountryID);
                    return ProductID;
                }
            });
        }

        public async Task ProductsImagesInsert(int? ProductID, List<Product.ProductImage> ProductImages)
        {
            await TryExecuteAsyncTask($"{nameof(ProductsImagesInsert)}({nameof(ProductID)} = {ProductID}, {nameof(ProductImages)} = {ProductImages.ToXml()})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.ProductsImagesInsert(ProductID, ProductImages.ToXml());
                }
            });
        }

        public async Task ProductsImagesSyncSortIndex(List<SyncSortIndexesItem> SortIndexXml)
        {
            await TryExecuteAsyncTask($"{nameof(ProductsImagesSyncSortIndex)}({nameof(SortIndexXml)} = {SortIndexXml.ToXml()})", async () =>
            {
                if (SortIndexXml?.Count > 0)
                {
                    using (var db = ConnectionFactory.GetDBCoreDataContext())
                    {
                        await db.ProductsImagesSyncSortIndex(SortIndexXml.ToXml());
                    }
                }
            });
        }
        #endregion
    }

    public class Product
    {
        #region Properties
        public int? ProductID { get; set; }
        public int? BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandNameEng { get; set; }
        public string BrandNameRus { get; set; }
        public string BrandImageFilename { get; set; }
        public int? CategoryID { get; set; }
        public string CategorySlug { get; set; }
        public string CategorySlugParent { get; set; }
        public string CategoryName { get; set; }
        public string CategoryNameEng { get; set; }
        public string CategoryNameRus { get; set; }
        public string ProductSlug { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductNameEng { get; set; }
        public string ProductNameRus { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? ProductPriceOld { get; set; }
        public decimal? ProductRemainder { get; set; }        
        public bool ProductIsOutOfStock => !(ProductRemainder > 0);
        public decimal? ProductQuantityBasket { get; set; }
        public string ProductImageFilename { get; set; }
        public bool ProductIsPublished { get; set; }
        public string ProductDescriptionShort { get; set; }
        public string ProductDescriptionShortEng { get; set; }
        public string ProductDescriptionShortRus { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDescriptionEng { get; set; }
        public string ProductDescriptionRus { get; set; }
        public bool ProductIsFeatured { get; set; }
        public string ProductSKU { get; set; }
        public int? ProductProducerCountryID { get; set; }
        public string ProductProducerCountry { get; set; }
        public string ProductProducerCountryEng { get; set; }
        public string ProductProducerCountryRus { get; set; }
        public int? ProductWeightKG { get; set; }
        public DateTime? ProductDateCreated { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        #endregion

        #region Sub Classes
        public class ProductImage
        {
            #region Properties
            public int? ProductImageID { get; set; }
            public int? ProductID { get; set; }
            public string ProductImageFilename { get; set; }
            public int? ProductImageSortIndex { get; set; }            
            #endregion

            #region Serialization
            public bool ShouldSerializeProductImageID() { return ProductImageID != null; }
            public bool ShouldSerializeProductID() { return ProductID != null; }
            public bool ShouldSerializeProductImageFilename() { return !string.IsNullOrWhiteSpace(ProductImageFilename); }
            public bool ShouldSerializeProductImageSortIndex() { return ProductImageSortIndex != null; }            
            #endregion
        }

        public class ProductSyncItem
        {
            #region Properties
            public string ProductName { get; set; }
            public string ProductNameEng { get; set; }
            public string ProductNameRus { get; set; }
            public int? ProductID { get; set; }
            public string ProductCode { get; set; }
            public string ProductCategory { get; set; }
            public int? CategoryID { get; set; }
            public string ProductBrand { get; set; }
            public int? BrandID { get; set; }
            public decimal? ProductPrice { get; set; }
            public decimal? ProductRemainder { get; set; }            
            #endregion

            #region Serialization
            public bool ShouldSerializeProductName() { return !string.IsNullOrWhiteSpace(ProductName); }
            public bool ShouldSerializeProductNameEng() { return !string.IsNullOrWhiteSpace(ProductNameEng); }
            public bool ShouldSerializeProductNameRus() { return !string.IsNullOrWhiteSpace(ProductNameRus); }
            public bool ShouldSerializeProductID() { return ProductID != null; }
            public bool ShouldSerializeProductCode() { return !string.IsNullOrWhiteSpace(ProductCode); }
            public bool ShouldSerializeProductCategory() { return !string.IsNullOrWhiteSpace(ProductCategory); }
            public bool ShouldSerializeCategoryID() { return CategoryID != null; }
            public bool ShouldSerializeProductBrand() { return !string.IsNullOrWhiteSpace(ProductBrand); }
            public bool ShouldSerializeBrandID() { return BrandID != null; }
            public bool ShouldSerializeProductPrice() { return ProductPrice != null; }
            public bool ShouldSerializeProductRemainder() { return ProductRemainder != null; }
            #endregion
        }
        #endregion
    }    

    public class ProductFilters
    {
        #region Properties
        public List<Filter> Categories { get; set; }
        public List<Filter> Brands { get; set; }
        public List<Filter> ProducerCountries { get; set; }
        public List<Filter> Volumes { get; set; }
        public List<Filter> Powers { get; set; }
        public List<Filter> SortOptions { get; set; }
        public decimal? PriceMax { get; set; }
        #endregion

        #region Sub Classes
        public class Filter
        {
            #region Properties
            public int? FilterID { get; set; }
            public int? FilterIDParent { get; set; }
            public string FilterSlug { get; set; }            
            public string FilterName { get; set; }
            #endregion
        } 
        #endregion
    }
}
