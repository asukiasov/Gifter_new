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
                cfg.CreateMap<DbContextQueries.ProductCategoriesListEntity, ProductCategoryDTO>();
                cfg.CreateMap<DbContextQueries.ProductCategoriesListForDeleteRecursiveEntity, ProductCategoriesListForDeleteRecursiveDTO>();
                cfg.CreateMap<DbContextQueries.ProductsListEntity, ProductsListDTO>();
            }).CreateMapper();
        }
        #endregion

        #region Methods
        public async Task ProductCategoriesDeleteRecursive(int? productCategoryID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(ProductCategoriesDeleteRecursive)}({nameof(productCategoryID)} = {productCategoryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.ProductCategoriesDeleteRecursive(productCategoryID: productCategoryID);
                    }
                }
            );
        }

        public async Task<ProductCategoryDTO> ProductCategoriesGetSingleByID(int? productCategoryID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesGetSingleByID)}({nameof(productCategoryID)} = {productCategoryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.ProductCategoriesGetSingleByID(productCategoryID: productCategoryID);
                        var result = resultJson?.DeserializeJsonTo<ProductCategoryDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<ProductCategoryDTO> ProductCategoriesGetSingleBySlug(string productCategorySlug)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesGetSingleBySlug)}({nameof(productCategorySlug)} = {productCategorySlug})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.ProductCategoriesGetSingleBySlug(productCategorySlug: productCategorySlug);
                        var result = resultJson?.DeserializeJsonTo<ProductCategoryDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> ProductCategoriesIUD(Enums.DatabaseActions databaseAction, int? productCategoryID = null, int? productCategoryParentID = null, string productCategoryName = null, string productCategoryNameEng = null, string productCategoryImageFilename = null, string productCategoryDescriptionShort = null, string productCategoryDescriptionShortEng = null)
        {
            productCategoryID = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productCategoryID)} = {productCategoryID}, {nameof(productCategoryParentID)} = {productCategoryParentID}, {nameof(productCategoryName)} = {productCategoryName}, {nameof(productCategoryNameEng)} = {productCategoryNameEng}, {nameof(productCategoryImageFilename)} = {productCategoryImageFilename}, {nameof(productCategoryDescriptionShort)} = {productCategoryDescriptionShort}, {nameof(productCategoryDescriptionShortEng)} = {productCategoryDescriptionShortEng})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        productCategoryID = await db.ProductCategoriesIUD(
                            databaseAction: databaseAction,
                            productCategoryID: productCategoryID,
                            productCategoryParentID: productCategoryParentID,
                            productCategoryName: productCategoryName,
                            productCategoryNameEng: productCategoryNameEng,
                            productCategoryImageFilename: productCategoryImageFilename,
                            productCategoryDescriptionShort: productCategoryDescriptionShort,
                            productCategoryDescriptionShortEng: productCategoryDescriptionShortEng
                        );
                        return productCategoryID;
                    }
                }
            );
            return productCategoryID;
        }

        public async Task<List<ProductCategoryDTO>> ProductCategoriesList(int? productCategoryParentID = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesList)}({nameof(productCategoryParentID)} = {productCategoryParentID})",
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.ProductCategoriesList(productCategoryParentID: productCategoryParentID)
                            .OrderBy(item => item.ProductCategorySortIndex)
                            .ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<ProductCategoryDTO>(item)).ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<ProductCategoriesListForDeleteRecursiveDTO>> ProductCategoriesListForDeleteRecursive(int? productCategoryID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesListForDeleteRecursive)}({nameof(productCategoryID)} = {productCategoryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.ProductCategoriesListForDeleteRecursive(productCategoryID: productCategoryID)
                            .ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<ProductCategoriesListForDeleteRecursiveDTO>(item))
                        .ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<List<ProductCategoryDTO>> ProductCategoriesListWithTitlePaddindHierarchy(char padChar = ' ')
        {
            var result = new List<ProductCategoryDTO>();

            Action<ProductCategoryDTO, int, List<ProductCategoryDTO>> InitCategoryNameByHierarchy = null;
            InitCategoryNameByHierarchy = (parent, padCount, categorysList) =>
            {
                if (padCount > 0)
                {
                    parent.ProductCategoryName = parent.ProductCategoryName.PadLeft(parent.ProductCategoryName.Length + padCount, padChar);
                    result.Add(parent);
                }
                else
                {
                    result.Add(parent);
                }

                var children = categorysList.Where(item => item.ProductCategoryParentID == parent.ProductCategoryID).ToList();
                foreach (var Category in children)
                {
                    InitCategoryNameByHierarchy(Category, padCount + 4, categorysList);
                }
            };

            var categories = await ProductCategoriesList();
            if (categories?.Count > 0)
            {
                var Parents = categories.Where(item => item.ProductCategoryParentID == null).OrderBy(item => item.ProductCategorySortIndex).ToList();
                foreach (var parent in Parents)
                {
                    InitCategoryNameByHierarchy(parent, 0, categories);
                }
            }

            return result;
        }

        public async Task ProductCategoriesSyncParentsAndSortIndexes(List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(ProductCategoriesSyncParentsAndSortIndexes)}({nameof(sortIndexesJson)} = {sortIndexesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.ProductCategoriesSyncParentsAndSortIndexes(sortIndexesJson: sortIndexesJson);
                    }
                }
            );
        }

        public async Task<ProductDTO> ProductsGetSingleByID(int? productID)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsGetSingleByID)}({nameof(productID)} = {productID})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.ProductsGetSingleByID(productID: productID);
                        var result = resultJson?.DeserializeJsonTo<ProductDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<ProductDTO> ProductsGetsingleBySlug(string productSlug)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsGetsingleBySlug)}({nameof(productSlug)} = {productSlug})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var resultJson = await db.ProductsGetsingleBySlug(productSlug: productSlug);
                        var result = resultJson?.DeserializeJsonTo<ProductDTO>();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions databaseAction, int? productID = null, int? productCategoryID = null, int? countryIDProducer = null, int? brandID = null, string productName = null, string productNameEng = null, string productSlug = null, string productSlugEng = null, decimal? productPrice = null, decimal? productPriceOld = null, decimal? productRemainder = null, string productImageFilename = null, string productDescriptionShort = null, string productDescriptionShortEng = null, string productDescription = null, string productDescriptionEng = null, bool? productIsPublished = null, bool? productIsFeatured = null, string productSKU = null, string productIDExternal = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productID)} = {productID}, {nameof(productCategoryID)} = {productCategoryID}, {nameof(countryIDProducer)} = {countryIDProducer}, {nameof(brandID)} = {brandID}, {nameof(productName)} = {productName}, {nameof(productNameEng)} = {productNameEng}, {nameof(productSlug)} = {productSlug}, {nameof(productSlugEng)} = {productSlugEng}, {nameof(productPrice)} = {productPrice}, {nameof(productPriceOld)} = {productPriceOld}, {nameof(productRemainder)} = {productRemainder}, {nameof(productImageFilename)} = {productImageFilename}, {nameof(productDescriptionShort)} = {productDescriptionShort}, {nameof(productDescriptionShortEng)} = {productDescriptionShortEng}, {nameof(productDescription)} = {productDescription}, {nameof(productDescriptionEng)} = {productDescriptionEng}, {nameof(productIsPublished)} = {productIsPublished}, {nameof(productIsFeatured)} = {productIsFeatured}, {nameof(productSKU)} = {productSKU}, {nameof(productIDExternal)} = {productIDExternal})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        productID = await db.ProductsIUD(
                            databaseAction: databaseAction,
                            productID: productID, 
                            productCategoryID: productCategoryID, 
                            countryIDProducer: countryIDProducer, 
                            brandID: brandID, 
                            productName: productName, 
                            productNameEng: productNameEng, 
                            productSlug: productSlug, 
                            productSlugEng: productSlugEng, 
                            productPrice: productPrice, 
                            productPriceOld: productPriceOld, 
                            productRemainder: productRemainder,
                            productImageFilename: productImageFilename,
                            productDescriptionShort: productDescriptionShort,
                            productDescriptionShortEng:productDescriptionShortEng, 
                            productDescription: productDescription,
                            productDescriptionEng: productDescriptionEng,
                            productIsPublished: productIsPublished,
                            productIsFeatured: productIsFeatured,
                            productSKU: productSKU,
                            productIDExternal: productIDExternal
                        );
                        return productID;
                    }
                }
            );
            return result;
        }

        public async Task<int?> ProductsImagesIUD(Enums.DatabaseActions databaseAction, int? productImageID = null, int? productID = null, string productImageFilename = null, int? productImageSortIndex = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsImagesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productImageID)} = {productImageID}, {nameof(productID)} = {productID}, {nameof(productImageFilename)} = {productImageFilename}, {nameof(productImageSortIndex)} = {productImageSortIndex})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        productImageID = await db.ProductsImagesIUD(
                            databaseAction: databaseAction,
                            productImageID: productImageID, 
                            productID: productID, 
                            productImageFilename: productImageFilename,
                            productImageSortIndex: productImageSortIndex
                        );
                        return productImageID;
                    }
                }
            );
            return result;
        }

        public async Task<List<ProductsListDTO>> ProductsList()
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsList)}()", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextQueries())
                    {
                        var result = (
                            await db.ProductsList()
                            .OrderByDescending(item => item.ProductDateCreated).ToListAsync()
                        )
                        ?.Select(item => _mapper.Map<ProductsListDTO>(item))
                        .ToList();
                        return result;
                    }
                }
            );
            return result;
        }

        public async Task ProductsImagesSyncSortIndex(int? productID, List<SyncSortIndexesDTO> sortIndexes)
        {
            var sortIndexesJson = sortIndexes.ToJson();
            await TryExecuteAsyncTask(
                logString: $"{nameof(ProductsImagesSyncSortIndex)}({nameof(productID)} = {productID}, {nameof(sortIndexes)} = {sortIndexesJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var db = _connectionFactory.GetDbContextCommands())
                    {
                        await db.ProductsImagesSyncSortIndex(productID: productID, sortIndexesJson: sortIndexesJson);
                    }
                }
            );
        }
        #endregion
    }
}