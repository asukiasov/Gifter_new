using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.Database;
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
        public ProductsRepository(DbContextFactory dbContextFactory, ILogger logger) : base(dbContextFactory, logger)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesDeleteRecursive),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(SqlDbType.Int)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesGetSingleByID),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(SqlDbType.Int)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesGetSingleBySlug),
                            sqlParameters:
                            [
                                productCategorySlug.ToSqlParameter(SqlDbType.NVarChar)
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

        public async Task<int?> ProductCategoriesIUD(Enums.DatabaseActions databaseAction, int? productCategoryID, ProductCategoryIudDTO productCategory)
        {
            var productCategoryJson = productCategory.ToJson();

            productCategoryID = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productCategoryID)} = {productCategoryID}, {nameof(productCategory)} = {productCategoryJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                productCategoryID.ToSqlParameterOutput(SqlDbType.Int),
                                productCategoryJson.ToSqlParameter(SqlDbType.NVarChar)
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

        public async Task<List<ProductCategoriesListDTO>> ProductCategoriesList(int? productCategoryParentID = null)
        {
            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductCategoriesList)}({nameof(productCategoryParentID)} = {productCategoryParentID})",
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesList),
                            sqlParameters:
                            [
                                productCategoryParentID.ToSqlParameter(SqlDbType.Int),
                            ]
                        );

                        var resultQueryable = sqb.ExecuteTableValuedFunction<ProductCategoriesListDTO>();
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesListForDeleteRecursive),
                            sqlParameters:
                            [
                                productCategoryID.ToSqlParameter(SqlDbType.Int),
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

        public async Task<List<ProductCategoriesListDTO>> ProductCategoriesListWithTitlePaddindHierarchy(char padChar = ' ')
        {
            var result = new List<ProductCategoriesListDTO>();

            Action<ProductCategoriesListDTO, int, List<ProductCategoriesListDTO>> InitCategoryNameByHierarchy = null;
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductCategoriesSyncParentsAndSortIndexes),
                            sqlParameters:
                            [
                                sortIndexesJson.ToSqlParameter(SqlDbType.NVarChar)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetSingleByID),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(SqlDbType.Int)
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsGetsingleBySlug),
                            sqlParameters:
                            [
                                productSlug.ToSqlParameter(SqlDbType.NVarChar)
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

        public async Task<int?> ProductsIUD(Enums.DatabaseActions databaseAction, int? productID, ProductIudDTO product)
        {
            var productJson = product.ToJson();

            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productID)} = {productID}, {nameof(productJson)} = {productJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                productID.ToSqlParameterOutput(SqlDbType.Int),
                                productJson.ToSqlParameter(SqlDbType.NVarChar)                                
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

        public async Task<int?> ProductsImagesIUD(Enums.DatabaseActions databaseAction, int? productImageID, ProductImageIudDTO productImage)
        {
            var productImageJson = productImage.ToJson();

            var result = await TryToReturnAsyncTask(
                logString: $"{nameof(ProductsImagesIUD)}({nameof(databaseAction)} = {databaseAction}, {nameof(productImageID)} = {productImageID}, {nameof(productImage)} = {productImageJson})", 
                asyncFuncToTry: async () =>
                {
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsImagesIUD),
                            sqlParameters:
                            [
                                databaseAction.ToSqlParameter(SqlDbType.TinyInt),
                                productImageID.ToSqlParameterOutput(SqlDbType.Int),
                                productImageJson.ToSqlParameter(SqlDbType.NVarChar)                                
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
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
                    using (var dbContext = _dbContextFactory.CreateDbContext())
                    {
                        var sqb = new SqlQueryBuilder(
                            dbContext: dbContext,
                            databaseObjectName: nameof(ProductsImagesSyncSortIndex),
                            sqlParameters:
                            [
                                productID.ToSqlParameter(SqlDbType.Int),
                                sortIndexesJson.ToSqlParameter(SqlDbType.NVarChar)
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