using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Libraries.FileStorages.Enums;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Core.Utilities.Extensions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.BusinessLogics
{
    public class ProductDeleteBusinessLogic
    {
        #region Properties
        readonly int? _productID;
        readonly RepositoryFactory _repositoryFactory;
        readonly IFileStorage _fileStorage;
        #endregion

        #region Constructors
        public ProductDeleteBusinessLogic(int? productID, RepositoryFactory repositoryFactory, IFileStorage fileStorage)
        {
            _productID = productID;
            _repositoryFactory = repositoryFactory;       
            _fileStorage = fileStorage;
        }
        #endregion

        #region Methods
        public async Task<Result> Execute()
        {
            var result = new Result();

            (var product, result.IsError, result.ErrorMessage) = await initBusinessLogicProperties();
            if (!result.IsError)
            {
                await deleteProductImageFiles(product);
                (result.IsError, result.ErrorMessage) = await deleteProduct();
            }

            return result;
        }

        async Task<(ProductDTO, bool, string)> initBusinessLogicProperties()
        {
            var isError = false;
            var errorMessage = default(string);
            var repository = _repositoryFactory.CreateProductsRepository();
            var product = await repository.ProductsGetSingleByID(_productID);
            if(product == null)
            {
                isError = true;
                errorMessage = $"Product #{_productID} is not found";
            }
            return (product, isError, errorMessage);
        }

        async Task deleteProductImageFiles(ProductDTO product)
        {
            if (product.ProductImages.HasElements())
            {
                foreach (var item in product.ProductImages)
                {
                    await _fileStorage.DeleteFile(
                        filename: item.ProductImageFilename,
                        folderPath: _fileStorage.GetFolderPathByModule(FileManagerModules.Products)
                    );
                }
            }

            if (!string.IsNullOrWhiteSpace(product.ProductImageFilename))
            {
                await _fileStorage.DeleteFile(
                    filename: product.ProductImageFilename,
                    folderPath: _fileStorage.GetFolderPathByModule(FileManagerModules.Products)
                );
            }
        }

        async Task<(bool, string)> deleteProduct()
        {
            var repository = _repositoryFactory.CreateProductsRepository();
            await repository.ProductsIUD(
                databaseAction: Enums.DatabaseActions.DELETE,
                productID: _productID,
                product: null
            );
            return (repository.IsError, repository.ErrorMessage);
        }
        #endregion

        #region Nested Classes
        public class Result : BusinessLogicResultBase
        {
            #region Properties
            #endregion
        }
        #endregion
    }
}
