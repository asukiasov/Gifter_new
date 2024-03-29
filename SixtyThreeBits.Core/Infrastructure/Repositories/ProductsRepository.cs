using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Database;
using SixtyThreeBits.Core.Infrastructure.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Repositories
{
    public class ProductsRepository : RepositoryBase
    {
        #region Constructors
        public ProductsRepository(DbContextFactory connectionFactory) : base(connectionFactory)
        {            
        }
        #endregion

        #region Methods
        public async Task ProductCategoriesDeleteRecursive(int? productCategoryID)
        {
            await TryExecuteAsyncTask(
                logString: $"{nameof(ProductCategoriesDeleteRecursive)}({nameof(productCategoryID)} = {productCategoryID})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesDeleteRecursive),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(nameof(productCategoryID), SqlDbType.Int)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();                        
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesGetSingleByID),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(nameof(productCategoryID), SqlDbType.Int)
                            ]
                         );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<ProductCategoryDTO>();

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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesGetSingleBySlug),
                            sqlParameters:
                            [
                                productCategorySlug.ToSqlParameter(nameof(productCategorySlug), SqlDbType.NVarChar)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<ProductCategoryDTO>();

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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                productCategoryID.ToSqlOutputParameter(nameof(productCategoryID),SqlDbType.Int),
                                productCategoryParentID.ToSqlParameter(nameof(productCategoryParentID),SqlDbType.Int),
                                productCategoryName.ToSqlParameter(nameof(productCategoryName),SqlDbType.NVarChar),
                                productCategoryNameEng.ToSqlParameter(nameof(productCategoryNameEng),SqlDbType.NVarChar),
                                productCategoryImageFilename.ToSqlParameter(nameof(productCategoryImageFilename),SqlDbType.NVarChar),
                                productCategoryDescriptionShort.ToSqlParameter(nameof(productCategoryDescriptionShort),SqlDbType.NVarChar),
                                productCategoryDescriptionShortEng.ToSqlParameter(nameof(productCategoryDescriptionShortEng),SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        productCategoryID = sqb.GetNextOutputParameterValue<int?>();
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesList),
                            sqlParameters:
                            [
                                productCategoryParentID.ToSqlParameter(nameof(productCategoryParentID), SqlDbType.Int),
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductCategoryDTO>();
                        resultQueryable = resultQueryable.OrderBy(item => item.ProductCategorySortIndex);
                        var result = await resultQueryable.ToListAsync();
                        
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesListForDeleteRecursive),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(nameof(productCategoryID), SqlDbType.Int),
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductCategoriesListForDeleteRecursiveDTO>();
                        var result = await resultQueryable.ToListAsync();

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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(sortIndexesJson),
                            sqlParameters:
                            [
                                sortIndexesJson.ToSqlParameter(nameof(sortIndexesJson),SqlDbType.NVarChar)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();                        
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetSingleByID),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(nameof(productID), SqlDbType.Int)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();
                        var result = resultJson.DeserializeJsonTo<ProductDTO>();

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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetsingleBySlug),
                            sqlParameters:
                            [
                                productSlug.ToSqlParameter(nameof(productSlug), SqlDbType.NVarChar)
                            ]
                        );

                        var resultJson = await sqb.ExecuteScalarValuedFunction<string>();                        
                        var result = resultJson.DeserializeJsonTo<ProductDTO>();

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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                productID.ToSqlOutputParameter(nameof(productID),SqlDbType.Int),
                                productCategoryID.ToSqlParameter(nameof(productCategoryID),SqlDbType.Int),
                                countryIDProducer.ToSqlParameter(nameof(countryIDProducer),SqlDbType.Int),
                                brandID.ToSqlParameter(nameof(brandID),SqlDbType.Int),
                                productName.ToSqlParameter(nameof(productName),SqlDbType.NVarChar),
                                productNameEng.ToSqlParameter(nameof(productNameEng),SqlDbType.NVarChar),
                                productPrice.ToSqlParameter(nameof(productPrice),SqlDbType.Money),
                                productPriceOld.ToSqlParameter(nameof(productPriceOld),SqlDbType.Money),
                                productRemainder.ToSqlParameter(nameof(productRemainder),SqlDbType.Decimal),
                                productImageFilename.ToSqlParameter(nameof(productImageFilename),SqlDbType.NVarChar),
                                productDescriptionShort.ToSqlParameter(nameof(productDescriptionShort),SqlDbType.NVarChar),
                                productDescriptionShortEng.ToSqlParameter(nameof(productDescriptionShortEng),SqlDbType.NVarChar),
                                productDescription.ToSqlParameter(nameof(productDescription),SqlDbType.NVarChar),
                                productDescriptionEng.ToSqlParameter(nameof(productDescriptionEng),SqlDbType.NVarChar),
                                productIsPublished.ToSqlParameter(nameof(productIsPublished),SqlDbType.Bit),
                                productIsFeatured.ToSqlParameter(nameof(productIsFeatured),SqlDbType.Bit),
                                productSKU.ToSqlParameter(nameof(productSKU),SqlDbType.NVarChar),
                                productIDExternal.ToSqlParameter(nameof(productIDExternal),SqlDbType.NVarChar)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        productID = sqb.GetNextOutputParameterValue<int?>();
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsImagesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                                productImageID.ToSqlOutputParameter(nameof(productImageID),SqlDbType.Int),
                                productID.ToSqlParameter(nameof(productID), SqlDbType.Int),
                                productImageFilename.ToSqlParameter(nameof(productImageFilename),SqlDbType.NVarChar),
                                productImageSortIndex.ToSqlParameter(nameof(productImageSortIndex),SqlDbType.Int)
                            ]
                        );

                        await sqb.ExecuteStoredProcedure();
                        productImageID = sqb.GetNextOutputParameterValue<int?>();
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsList)
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductsListDTO>();
                        resultQueryable = resultQueryable.OrderByDescending(item => item.ProductDateCreated);
                        var result = await resultQueryable.ToListAsync();
                        
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
                    using (var dbContext = _dbContextFactory.GetDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsImagesSyncSortIndex),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(nameof(productID), SqlDbType.Int),
                                sortIndexesJson.ToSqlParameter(nameof(sortIndexesJson),SqlDbType.NVarChar)
                            ]
                        );
                        await sqb.ExecuteStoredProcedure();                        
                    }
                }
            );
        }
        #endregion
    }
}