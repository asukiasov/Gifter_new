using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextQueries
    {
        #region ProductCategoriesGetSingleByID        
        public async Task<string> ProductCategoriesGetSingleByID(int? productCategoryID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductCategoriesGetSingleByID),
                sqlParameters:
                [
                    productCategoryID.ToSqlParameter(nameof(productCategoryID), SqlDbType.Int)
                ]
             );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region ProductCategoriesGetSingleBySlug        
        public async Task<string> ProductCategoriesGetSingleBySlug(string productCategorySlug)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductCategoriesGetSingleBySlug),
                sqlParameters:
                [
                    productCategorySlug.ToSqlParameter(nameof(productCategorySlug), SqlDbType.NVarChar)
                ]
             );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region ProductCategoriesList
        public record ProductCategoriesListEntity
        (
            int? ProductCategoryID,
            int? ProductCategoryParentID,
            string ProductCategorySlug,
            string ProductCategoryName,
            string ProductCategoryNameEng,
            int? ProductCategorySortIndex,
            string ProductCategoryImageFilename,
            DateTime? ProductCategoryDateCreated
        );
        public IQueryable<ProductCategoriesListEntity> ProductCategoriesList(int? productCategoryParentID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductCategoriesList),
                sqlParameters:
                [
                    productCategoryParentID.ToSqlParameter(nameof(productCategoryParentID), SqlDbType.Int),
                ]
            );
            var result = sqb.ExecuteQuery<ProductCategoriesListEntity>();
            return result;
        }
        #endregion

        #region ProductCategoriesListForDeleteRecursive
        public record ProductCategoriesListForDeleteRecursiveEntity
        (
            int? ProductCategoryID,
            string ProductCategoryImageFilename
        );
        public IQueryable<ProductCategoriesListForDeleteRecursiveEntity> ProductCategoriesListForDeleteRecursive(int? productCategoryID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductCategoriesListForDeleteRecursive),
                sqlParameters:
                [
                    productCategoryID.ToSqlParameter(nameof(productCategoryID), SqlDbType.Int),
                ]
            );
            var result = sqb.ExecuteQuery<ProductCategoriesListForDeleteRecursiveEntity>();
            return result;
        }
        #endregion

        #region ProductsGetSingleByID        
        public async Task<string> ProductsGetSingleByID(int? productID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductsGetSingleByID),
                sqlParameters:
                [
                    productID.ToSqlParameter(nameof(productID), SqlDbType.Int)
                ]
             );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region ProductsGetsingleBySlug        
        public async Task<string> ProductsGetsingleBySlug(string productSlug)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductsGetsingleBySlug),
                sqlParameters:
                [
                    productSlug.ToSqlParameter(nameof(productSlug), SqlDbType.NVarChar)
                ]
            );
            var result = await sqb.ExecuteQueryScalar<string>();
            return result;
        }
        #endregion

        #region ProductsList
        public record ProductsListEntity
        (
            int? ProductID,
            int? ProductCategoryID,
            int? CountryIDProducer,
            int? BrandID,
            string ProductName,
            string ProductNameEng,
            string ProductSlug,
            string ProductSlugEng,
            decimal? ProductPrice,
            decimal? ProductPriceOld,
            decimal? ProductRemainder,
            string ProductImageFilename,
            bool ProductIsPublished,
            bool ProductIsFeatured,
            string ProductSKU,
            string ProductIDExternal,
            DateTime? ProductDateCreated
        );
        public IQueryable<ProductsListEntity> ProductsList()
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectName: nameof(ProductsList)
            );
            var result = sqb.ExecuteQuery<ProductsListEntity>();
            return result;
        }
        #endregion        
    }
}