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
        #region Properties
        readonly UtilityCollection Utilities;
        #endregion

        #region Constructors
        public ProductsDataAccess(ConnectionFactory ConnectionFactory, UtilityCollection Utilities) : base(ConnectionFactory)
        {
            this.Utilities = Utilities;
        }
        #endregion

        #region Methods
        public async Task ProductCategoriesDeleteRecursive(int? ProductCategoryID)
        {
            await TryExecuteAsyncTask($"{nameof(ProductCategoriesDeleteRecursive)}({nameof(ProductCategoryID)} = {ProductCategoryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var DBItems = db.ProductCategoriesListForDeleteRecursive(ProductCategoryID).ToList();
                    foreach (var Item in DBItems)
                    {
                        Utilities.DeleteUploadedFile(Item.ProductCategoryImageFilename);
                    }
                    await db.ProductCategoriesDeleteRecursive(ProductCategoryID);
                }
            });
        }

        public async Task<ProductCategory> ProductCategoriesGetSingleByID(int? ProductCategoryID)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesGetSingleByID)}({nameof(ProductCategoryID)} = {ProductCategoryID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductCategoriesGetSingleByID(ProductCategoryID);
                    return Result?.DeserializeJsonTo<ProductCategory>();
                }
            });
        }

        public async Task<ProductCategory> ProductCategoriesGetSingleBySlug(string ProductCategorySlug)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesGetSingleBySlug)}({nameof(ProductCategorySlug)} = {ProductCategorySlug})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductCategoriesGetSingleBySlug(ProductCategorySlug);
                    return Result?.DeserializeJsonTo<ProductCategory>();
                }
            });
        }

        public async Task<int?> ProductCategoriesIUD(Enums.DatabaseActions DatabaseAction, int? ProductCategoryID = null, int? ProductCategoryParentID = null, string ProductCategoryName = null, string ProductCategoryNameEng = null, string ProductCategorynameRus = null, string ProductCategoryImageFilename = null, string ProductCategoryDescriptionShort = null, string ProductCategoryDescriptionShortEng = null, string ProductCategoryDescriptionShortRus = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProductCategoryID)} = {ProductCategoryID}, {nameof(ProductCategoryParentID)} = {ProductCategoryParentID}, {nameof(ProductCategoryName)} = {ProductCategoryName}, {nameof(ProductCategoryNameEng)} = {ProductCategoryNameEng}, {nameof(ProductCategorynameRus)} = {ProductCategorynameRus}, {nameof(ProductCategoryImageFilename)} = {ProductCategoryImageFilename}, {nameof(ProductCategoryDescriptionShort)} = {ProductCategoryDescriptionShort}, {nameof(ProductCategoryDescriptionShortEng)} = {ProductCategoryDescriptionShortEng}, {nameof(ProductCategoryDescriptionShortRus)} = {ProductCategoryDescriptionShortRus} )", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    ProductCategoryID = await db.ProductCategoriesIUD(DatabaseAction, ProductCategoryID, ProductCategoryParentID, ProductCategoryName, ProductCategoryNameEng, ProductCategorynameRus, ProductCategoryImageFilename, ProductCategoryDescriptionShort, ProductCategoryDescriptionShortEng, ProductCategoryDescriptionShortRus);
                    return ProductCategoryID;
                }
            });
        }

        public async Task<List<ProductCategory>> ProductCategoriesList(int? ProductCategoryParentID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesList)}({nameof(ProductCategoryParentID)} = {ProductCategoryParentID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return (await db.ProductCategoriesList(ProductCategoryParentID).OrderBy(Item => Item.ProductCategorySortIndex).ToListAsync())?.Select(Item => new ProductCategory
                    {
                        ProductCategoryID = Item.ProductCategoryID,
                        ProductCategoryParentID = Item.ProductCategoryParentID,
                        ProductCategorySlug = Item.ProductCategorySlug,
                        ProductCategoryName = Item.ProductCategoryName,
                        ProductCategoryNameEng = Item.ProductCategoryNameEng,
                        ProductCategoryNameRus = Item.ProductCategoryNameRus,
                        ProductCategorySortIndex = Item.ProductCategorySortIndex,
                        ProductCategoryImageFilename = Item.ProductCategoryImageFilename
                    }).ToList();
                }
            });
        }

        public async Task<List<ProductCategory>> ProductCategoriesListWithTitlePaddindHierarchy(char PadChar = ' ')
        {
            var Result = new List<ProductCategory>();

            Action<ProductCategory, int, List<ProductCategory>> InitCategoryNameByHierarchy = null;
            InitCategoryNameByHierarchy = (ProductCategory Parent, int PadCount, List<ProductCategory> CategorysList) =>
            {
                if (PadCount > 0)
                {
                    Parent.ProductCategoryName = Parent.ProductCategoryName.PadLeft(Parent.ProductCategoryName.Length + PadCount, PadChar);
                    Result.Add(Parent);
                }
                else
                {
                    Result.Add(Parent);
                }

                var Children = CategorysList.Where(Item => Item.ProductCategoryParentID == Parent.ProductCategoryID).ToList();
                foreach (var Category in Children)
                {
                    InitCategoryNameByHierarchy(Category, PadCount + 4, CategorysList);
                }
            };

            var Categories = await ProductCategoriesList();
            if (Categories?.Count > 0)
            {
                var Parents = Categories.Where(Item => Item.ProductCategoryParentID == null).OrderBy(Item => Item.ProductCategorySortIndex).ToList();
                foreach (var Item in Parents)
                {
                    InitCategoryNameByHierarchy(Item, 0, Categories);
                }
            }

            return Result;
        }

        public async Task ProductCategoriesSyncParentsAndSortIndexes(List<SyncSortIndexesItem> SortIndexes)
        {
            var SortIndexesJson = SortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(ProductCategoriesSyncParentsAndSortIndexes)}({nameof(SortIndexes)} = {SortIndexesJson})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.ProductCategoriesSyncParentsAndSortIndexes(SortIndexesJson);
                }
            });
        }

        public async Task<Product> ProductsGetSingleByID(int? ProductID)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsGetSingleByID)}({nameof(ProductID)} = {ProductID})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductsGetSingleByID(ProductID);
                    return Result?.DeserializeJsonTo<Product>();
                }
            });
        }

        public async Task<Product> ProductsGetsingleBySlug(string ProductSlug)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsGetsingleBySlug)}({nameof(ProductSlug)} = {ProductSlug})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    var Result = await db.ProductsGetsingleBySlug(ProductSlug);
                    return Result?.DeserializeJsonTo<Product>();
                }
            });
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions DatabaseAction, int? ProductID = null, int? ProductCategoryID = null, int? CountryIDProducer = null, int? BrandID = null, string ProductName = null, string ProductNameEng = null, string ProductNameRus = null, string ProductSlug = null, string ProductSlugEng = null, string ProductSlugRus = null, decimal? ProductPrice = null, decimal? ProductPriceOld = null, decimal? ProductRemainder = null, string ProductImageFilename = null, string ProductDescriptionShort = null, string ProductDescriptionShortEng = null, string ProductDescriptionShortRus = null, string ProductDescription = null, string ProductDescriptionEng = null, string ProductDescriptionRus = null, bool? ProductIsPublished = null, bool? ProductIsFeatured = null, string ProductSKU = null, string ProductIDExternal = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsIUD)}({nameof(DatabaseAction)} = {DatabaseAction}, {nameof(ProductID)} = {ProductID}, {nameof(ProductCategoryID)} = {ProductCategoryID}, {nameof(CountryIDProducer)} = {CountryIDProducer}, {nameof(BrandID)} = {BrandID}, {nameof(ProductName)} = {ProductName}, {nameof(ProductNameEng)} = {ProductNameEng}, {nameof(ProductNameRus)} = {ProductNameRus}, {nameof(ProductSlug)} = {ProductSlug}, {nameof(ProductSlugEng)} = {ProductSlugEng}, {nameof(ProductSlugRus)} = {ProductSlugRus}, {nameof(ProductPrice)} = {ProductPrice}, {nameof(ProductPriceOld)} = {ProductPriceOld}, {nameof(ProductRemainder)} = {ProductRemainder}, {nameof(ProductImageFilename)} = {ProductImageFilename}, {nameof(ProductDescriptionShort)} = {ProductDescriptionShort}, {nameof(ProductDescriptionShortEng)} = {ProductDescriptionShortEng}, {nameof(ProductDescriptionShortRus)} = {ProductDescriptionShortRus}, {nameof(ProductDescription)} = {ProductDescription}, {nameof(ProductDescriptionEng)} = {ProductDescriptionEng}, {nameof(ProductDescriptionRus)} = {ProductDescriptionRus}, {nameof(ProductIsPublished)} = {ProductIsPublished}, {nameof(ProductIsFeatured)} = {ProductIsFeatured}, {nameof(ProductSKU)} = {ProductSKU}, {nameof(ProductIDExternal)} = {ProductIDExternal})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    ProductID = await db.ProductsIUD(DatabaseAction, ProductID, ProductCategoryID, CountryIDProducer, BrandID, ProductName, ProductNameEng, ProductNameRus, ProductSlug, ProductSlugEng, ProductSlugRus, ProductPrice, ProductPriceOld, ProductRemainder, ProductImageFilename, ProductDescriptionShort, ProductDescriptionShortEng, ProductDescriptionShortRus, ProductDescription, ProductDescriptionEng, ProductDescriptionRus, ProductIsPublished, ProductIsFeatured, ProductSKU, ProductIDExternal);
                    return ProductID;
                }
            });
        }

        public async Task<List<DBCoreDataContext.ProductsListResultItem>> ProductsList(bool? ProductIsPubliShed = null, bool? ProductIsFeatured = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsList)}({nameof(ProductIsPubliShed)} = {ProductIsPubliShed}, {nameof(ProductIsFeatured)} = {ProductIsFeatured})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    return await db.ProductsList(ProductIsPubliShed, ProductIsFeatured).OrderByDescending(Item => Item.ProductDateCreated).ToListAsync();
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

        public async Task ProductsImagesSyncSortIndex(int? ProductID, List<SyncSortIndexesItem> SortIndexes)
        {
            var SortIndexesJson = SortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(ProductsImagesSyncSortIndex)}({nameof(ProductID)} = {ProductID}, {nameof(SortIndexes)} = {SortIndexesJson})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.ProductsImagesSyncSortIndex(ProductID, SortIndexesJson);
                }
            });
        }

        public async Task ProductsSyncPricesAndRemainders(List<Product> Products)
        {
            var ProductsJson = Products.ToJson();
            await TryExecuteAsyncTask($"{nameof(ProductsSyncPricesAndRemainders)}({nameof(Products)} = {ProductsJson})", async () =>
            {
                using (var db = ConnectionFactory.GetDBCoreDataContext())
                {
                    await db.ProductsSync(ProductsJson);
                }
            });
        }
        #endregion
    }

    public class ProductCategory
    {
        #region Properties
        public int? ProductCategoryID { get; set; }
        public int? ProductCategoryParentID { get; set; }
        public string ProductCategorySlug { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductCategoryNameEng { get; set; }
        public string ProductCategoryNameRus { get; set; }
        public string ProductCategoryImageFilename { get; set; }
        public int? ProductCategorySortIndex { get; set; }
        public string ProductCategoryDescriptionShort { get; set; }
        public string ProductCategoryDescriptionShortEng { get; set; }
        public string ProductCategoryDescriptionShortRus { get; set; }
        public DateTime? ProductCategoryDateCreated { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{ProductCategoryID} - {ProductCategoryName}";
        }
        #endregion
    }

    public class Product
    {
        #region Properties
        public int? ProductID { get; set; }
        public int? ProductCategoryID { get; set; }
        public int? CountryIDProducer { get; set; }
        public int? BrandID { get; set; }
        public string ProductName { get; set; }
        public string ProductNameEng { get; set; }
        public string ProductNameRus { get; set; }
        public string ProductSlug { get; set; }
        public string ProductSlugEng { get; set; }
        public string ProductSlugRus { get; set; }
        public decimal? ProductPrice { get; set; }
        public decimal? ProductPriceOld { get; set; }
        public decimal? ProductRemainder { get; set; }
        public string ProductImageFilename { get; set; }
        public string ProductDescriptionShort { get; set; }
        public string ProductDescriptionShortEng { get; set; }
        public string ProductDescriptionShortRus { get; set; }
        public string ProductDescription { get; set; }
        public string ProductDescriptionEng { get; set; }
        public string ProductDescriptionRus { get; set; }
        public bool ProductIsPublished { get; set; }
        public bool ProductIsFeatured { get; set; }
        public string ProductSKU { get; set; }
        public string ProductIDExternal { get; set; }
        public DateTime? ProductDateCreated { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        #endregion

        #region Nested Classes
        public class ProductImage
        {
            #region Properties
            public int? ProductImageID { get; set; }
            public string ProductImageFilename { get; set; }
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

        #region Nested Classes
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
