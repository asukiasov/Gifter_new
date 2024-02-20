using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase
    {
        #region Constructors
        public ProductsRepository(ConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DBQueriesDataContext.ProductCategoriesListEntity, ProductCategoryDTO>();
                cfg.CreateMap<DBQueriesDataContext.ProductCategoriesListForDeleteRecursiveEntity, ProductCategoriesListForDeleteRecursiveDTO>();
                cfg.CreateMap<DBQueriesDataContext.ProductsListEntity, ProductsListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task ProductCategoriesDeleteRecursive(int? productCategoryID)
        {
            await TryExecuteAsyncTask($"{nameof(ProductCategoriesDeleteRecursive)}({nameof(productCategoryID)} = {productCategoryID})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.ProductCategoriesDeleteRecursive(productCategoryID);
                }
            });
        }

        public async Task<ProductCategoryDTO> ProductCategoriesGetSingleByID(int? productCategoryID)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesGetSingleByID)}({nameof(productCategoryID)} = {productCategoryID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.ProductCategoriesGetSingleByID(productCategoryID);
                    return Result?.DeserializeJsonTo<ProductCategoryDTO>();
                }
            });
        }

        public async Task<ProductCategoryDTO> ProductCategoriesGetSingleBySlug(string productCategorySlug)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesGetSingleBySlug)}({nameof(productCategorySlug)} = {productCategorySlug})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.ProductCategoriesGetSingleBySlug(productCategorySlug);
                    return Result?.DeserializeJsonTo<ProductCategoryDTO>();
                }
            });
        }

        public async Task<int?> ProductCategoriesIUD(Enums.DatabaseActions databaseAction, int? productCategoryID = null, int? productCategoryParentID = null, string productCategoryName = null, string productCategoryNameEng = null, string productCategoryImageFilename = null, string productCategoryDescriptionShort = null, string productCategoryDescriptionShortEng = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productCategoryID)} = {productCategoryID}, {nameof(productCategoryParentID)} = {productCategoryParentID}, {nameof(productCategoryName)} = {productCategoryName}, {nameof(productCategoryNameEng)} = {productCategoryNameEng}, {nameof(productCategoryImageFilename)} = {productCategoryImageFilename}, {nameof(productCategoryDescriptionShort)} = {productCategoryDescriptionShort}, {nameof(productCategoryDescriptionShortEng)} = {productCategoryDescriptionShortEng})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    productCategoryID = await db.ProductCategoriesIUD(databaseAction, productCategoryID, productCategoryParentID, productCategoryName, productCategoryNameEng, productCategoryImageFilename, productCategoryDescriptionShort, productCategoryDescriptionShortEng);
                    return productCategoryID;
                }
            });
        }

        public async Task<List<ProductCategoryDTO>> ProductCategoriesList(int? productCategoryParentID = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesList)}({nameof(productCategoryParentID)} = {productCategoryParentID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.ProductCategoriesList(productCategoryParentID).OrderBy(item => item.ProductCategorySortIndex).ToListAsync())?.Select(item => _mapper.Map<ProductCategoryDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<ProductCategoriesListForDeleteRecursiveDTO>> ProductCategoriesListForDeleteRecursive(int? productCategoryID)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductCategoriesListForDeleteRecursive)}({nameof(productCategoryID)} = {productCategoryID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.ProductCategoriesListForDeleteRecursive(productCategoryID).ToListAsync())?.Select(item => _mapper.Map<ProductCategoriesListForDeleteRecursiveDTO>(item)).ToList();
                }
            });
        }

        public async Task<List<ProductCategoryDTO>> ProductCategoriesListWithTitlePaddindHierarchy(char padChar = ' ')
        {
            var Result = new List<ProductCategoryDTO>();

            Action<ProductCategoryDTO, int, List<ProductCategoryDTO>> InitCategoryNameByHierarchy = null;
            InitCategoryNameByHierarchy = (Parent, PadCount, CategorysList) =>
            {
                if (PadCount > 0)
                {
                    Parent.ProductCategoryName = Parent.ProductCategoryName.PadLeft(Parent.ProductCategoryName.Length + PadCount, padChar);
                    Result.Add(Parent);
                }
                else
                {
                    Result.Add(Parent);
                }

                var Children = CategorysList.Where(item => item.ProductCategoryParentID == Parent.ProductCategoryID).ToList();
                foreach (var Category in Children)
                {
                    InitCategoryNameByHierarchy(Category, PadCount + 4, CategorysList);
                }
            };

            var Categories = await ProductCategoriesList();
            if (Categories?.Count > 0)
            {
                var Parents = Categories.Where(item => item.ProductCategoryParentID == null).OrderBy(item => item.ProductCategorySortIndex).ToList();
                foreach (var Item in Parents)
                {
                    InitCategoryNameByHierarchy(Item, 0, Categories);
                }
            }

            return Result;
        }

        public async Task ProductCategoriesSyncParentsAndSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var SortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(ProductCategoriesSyncParentsAndSortIndexes)}({nameof(sortIndexes)} = {SortIndexesJson})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.ProductCategoriesSyncParentsAndSortIndexes(SortIndexesJson);
                }
            });
        }

        public async Task<ProductDTO> ProductsGetSingleByID(int? productID)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsGetSingleByID)}({nameof(productID)} = {productID})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.ProductsGetSingleByID(productID);
                    return Result?.DeserializeJsonTo<ProductDTO>();
                }
            });
        }

        public async Task<ProductDTO> ProductsGetsingleBySlug(string productSlug)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsGetsingleBySlug)}({nameof(productSlug)} = {productSlug})", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    var Result = await db.ProductsGetsingleBySlug(productSlug);
                    return Result?.DeserializeJsonTo<ProductDTO>();
                }
            });
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions databaseAction, int? productID = null, int? productCategoryID = null, int? countryIDProducer = null, int? brandID = null, string productName = null, string productNameEng = null, string productSlug = null, string productSlugEng = null, decimal? productPrice = null, decimal? productPriceOld = null, decimal? productRemainder = null, string productImageFilename = null, string productDescriptionShort = null, string productDescriptionShortEng = null, string productDescription = null, string productDescriptionEng = null, bool? productIsPublished = null, bool? productIsFeatured = null, string productSKU = null, string productIDExternal = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productID)} = {productID}, {nameof(productCategoryID)} = {productCategoryID}, {nameof(countryIDProducer)} = {countryIDProducer}, {nameof(brandID)} = {brandID}, {nameof(productName)} = {productName}, {nameof(productNameEng)} = {productNameEng}, {nameof(productSlug)} = {productSlug}, {nameof(productSlugEng)} = {productSlugEng}, {nameof(productPrice)} = {productPrice}, {nameof(productPriceOld)} = {productPriceOld}, {nameof(productRemainder)} = {productRemainder}, {nameof(productImageFilename)} = {productImageFilename}, {nameof(productDescriptionShort)} = {productDescriptionShort}, {nameof(productDescriptionShortEng)} = {productDescriptionShortEng}, {nameof(productDescription)} = {productDescription}, {nameof(productDescriptionEng)} = {productDescriptionEng}, {nameof(productIsPublished)} = {productIsPublished}, {nameof(productIsFeatured)} = {productIsFeatured}, {nameof(productSKU)} = {productSKU}, {nameof(productIDExternal)} = {productIDExternal})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    productID = await db.ProductsIUD(databaseAction, productID, productCategoryID, countryIDProducer, brandID, productName, productNameEng, productSlug, productSlugEng, productPrice, productPriceOld, productRemainder, productImageFilename, productDescriptionShort, productDescriptionShortEng, productDescription, productDescriptionEng, productIsPublished, productIsFeatured, productSKU, productIDExternal);
                    return productID;
                }
            });
        }

        public async Task<int?> ProductsImagesIUD(Enums.DatabaseActions databaseAction, int? productImageID = null, int? productID = null, string productImageFilename = null, int? productImageSyncSortIndex = null)
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsImagesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productImageID)} = {productImageID}, {nameof(productID)} = {productID}, {nameof(productImageFilename)} = {productImageFilename}, {nameof(productImageSyncSortIndex)} = {productImageSyncSortIndex})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    productImageID = await db.ProductsImagesIUD(databaseAction, productImageID, productID, productImageFilename, productImageSyncSortIndex);
                    return productImageID;
                }
            });
        }

        public async Task<List<ProductsListDTO>> ProductsList()
        {
            return await TryToReturnAsyncTask($"{nameof(ProductsList)}()", async () =>
            {
                using (var db = _connectionFactory.GetDBQueriesDataContext())
                {
                    return (await db.ProductsList().OrderByDescending(item => item.ProductDateCreated).ToListAsync())?.Select(item => _mapper.Map<ProductsListDTO>(item)).ToList();
                }
            });
        }

        public async Task ProductsImagesSyncSortIndex(int? productID, List<SyncSortIndexesDTO> sortIndexes)
        {
            var SortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask($"{nameof(ProductsImagesSyncSortIndex)}({nameof(productID)} = {productID}, {nameof(sortIndexes)} = {SortIndexesJson})", async () =>
            {
                using (var db = _connectionFactory.GetDBCommandsDataContext())
                {
                    await db.ProductsImagesSyncSortIndex(productID, SortIndexesJson);
                }
            });
        }
        #endregion
    }
}