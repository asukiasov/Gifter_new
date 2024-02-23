using SixtyThreeBits.Core.Utilities;
using System.Data;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    public partial class DbContextCommands
    {
        #region Methods
        public async Task ProductCategoriesDeleteRecursive(int? productCategoryID)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductCategoriesDeleteRecursive),
                itemType: null,
                sqlParameters:
                [
                    productCategoryID.ToSqlParameter(nameof(productCategoryID),SqlDbType.Int)
                ]
            );

            await sqb.ExecuteCommand();
        }

        public async Task<int?> ProductCategoriesIUD(Enums.DatabaseActions databaseAction, int? productCategoryID, int? productCategoryParentID, string productCategoryName, string productCategoryNameEng, string productCategoryImageFilename, string productCategoryDescriptionShort, string productCategoryDescriptionShortEng)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductCategoriesIUD),
                itemType: null,
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

            await sqb.ExecuteCommand();
            productCategoryID = sqb.GetNextOutputParameterValue<int?>();
            return productCategoryID;
        }

        public async Task ProductCategoriesSyncParentsAndSortIndexes(string parentsAndSortIndexesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductCategoriesSyncParentsAndSortIndexes),
                itemType: null,
                sqlParameters:
                [
                    parentsAndSortIndexesJson.ToSqlParameter(nameof(parentsAndSortIndexesJson),SqlDbType.NVarChar)
                ]
           );

            await sqb.ExecuteCommand();
        }

        public async Task<int?> ProductsIUD(Enums.DatabaseActions databaseAction, int? productID, int? productCategoryID, int? countryIDProducer, int? brandID, string productName, string productNameEng, string productSlug, string productSlugEng, decimal? productPrice, decimal? productPriceOld, decimal? productRemainder, string productImageFilename, string productDescriptionShort, string productDescriptionShortEng, string productDescription, string productDescriptionEng, bool? productIsPublished, bool? productIsFeatured, string productSKU, string productIDExternal)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductsIUD),
                itemType: null,
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

            await sqb.ExecuteCommand();
            productID = sqb.GetNextOutputParameterValue<int?>();
            return productID;
        }

        public async Task<int?> ProductsImagesIUD(Enums.DatabaseActions databaseAction, int? productImageID, int? productID, string productImageFilename, int? productImageSortIndex)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductsImagesIUD),
                itemType: null,
                sqlParameters:
                [
                    databaseAction.ToSqlParameter(nameof(databaseAction),SqlDbType.TinyInt),
                    productImageID.ToSqlOutputParameter(nameof(productImageID),SqlDbType.Int),
                    productID.ToSqlParameter(nameof(productID), SqlDbType.Int),
                    productImageFilename.ToSqlParameter(nameof(productImageFilename),SqlDbType.NVarChar),
                    productImageSortIndex.ToSqlParameter(nameof(productImageSortIndex),SqlDbType.Int)
                ]
            );

            await sqb.ExecuteCommand();
            productImageID = sqb.GetNextOutputParameterValue<int?>();
            return productImageID;
        }

        public async Task ProductsImagesSyncSortIndex(int? productID, string sortIndexesJson)
        {
            var sqb = new SqlQueryBuilder(
                dbContext: this,
                databaseObjectType: DatabaseObjectTypes.STORED_PROCEDURE,
                databaseObjectName: nameof(ProductsImagesSyncSortIndex),
                itemType: null,
                sqlParameters:
                [
                    productID.ToSqlParameter(nameof(productID), SqlDbType.Int),
                    sortIndexesJson.ToSqlParameter(nameof(sortIndexesJson),SqlDbType.NVarChar)
                ]
            );

            await sqb.ExecuteCommand();
        }

        #endregion
    }
}