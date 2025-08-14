using ExcelDataReader;
using SixtyThreeBits.Core.BusinessLogics.Base;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.BusinessLogics.Products
{
    public class ProductsBusinessLogicSyncPricesAndRemainders
    {
		#region Properties
		readonly byte[] _excelFileBytes;
		readonly bool _isXlsx;
		readonly RepositoryFactory _repositoryFactory;
        #endregion

        #region Constructors
        public ProductsBusinessLogicSyncPricesAndRemainders(byte[] excelFileBytes, bool isXslx, RepositoryFactory dataAccessFactory)
		{
			_excelFileBytes = excelFileBytes;
			_isXlsx = isXslx;
			_repositoryFactory = dataAccessFactory;
        }
		#endregion

		#region Methods
		public async Task<Result> Execute()
		{
			var result = new Result();

			(var excelItems, result.IsError, result.ErrorMessage) = parseExcel();
			if (!result.IsError)
			{
				(result.IsError, result.HasExcelErrors, result.ExcelErrors) = validateExcel(excelItems);
				if (!result.IsError)
				{
					(result.IsError, result.ErrorMessage) = await updateExcelItemProductIDs(excelItems);
                    if (!result.IsError)
					{
                        (result.IsError, result.ErrorMessage) = await syncPricesAndRemainders(excelItems);
					}
                }
			}

			return result;
		}

		(List<productExcelItem>, bool, string) parseExcel()
		{
			var isError = false;
			var errorMessage = default(string);
			var excelItems = default(List<productExcelItem>);

            try
			{
				using (var inputStream = new MemoryStream(_excelFileBytes))
				{
                    using (var excelReader = _isXlsx ? ExcelReaderFactory.CreateOpenXmlReader(inputStream) : ExcelReaderFactory.CreateBinaryReader(inputStream))
					{
						excelReader.Read();

						excelItems = new List<productExcelItem>(excelReader.ResultsCount);
						var rowNumber = 1;
						while (excelReader.Read())
						{
							var item = new productExcelItem();
							item.RowNumber = ++rowNumber;
							item.ProductName = excelReader.GetValue(0)?.ToString().Trim();

							item.ProductPriceString = excelReader.GetValue(1)?.ToString().Trim();
							item.ProductPrice = item.ProductPriceString.ToDecimal();

							item.ProductRemainderString = excelReader.GetValue(2)?.ToString().Trim();
							item.ProductRemainder = item.ProductRemainderString.ToInt();

							excelItems.Add(item);
						}
					}
				}
			}
			catch(Exception ex)
			{
				isError = true;
				errorMessage = ex.Message;
			}

			return (excelItems, isError, errorMessage);
		}

		(bool, bool, List<string>) validateExcel(List<productExcelItem> excelItems)
		{
			var errorStringTemplateLineColumn = "Line {0} - Column {1} - {2}";
			var excelErrors = new List<string>(excelItems.Count);

			foreach(var excelItem in excelItems)
			{
				if (string.IsNullOrWhiteSpace(excelItem.ProductName))
				{
					excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "A", Resources.ValidationProductNameRequired));
				}

				if (string.IsNullOrWhiteSpace(excelItem.ProductPriceString))
				{
                    excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductPriceRequired));
				}
				else if(excelItem.ProductPrice is null || excelItem.ProductPrice < 0)
				{
                    excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductPriceFormatInvalid));
				}

				if (string.IsNullOrWhiteSpace(excelItem.ProductRemainderString))
				{
                    excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductRemainderRequired));
				}
				else if (excelItem.ProductRemainder is null || excelItem.ProductRemainder < 0)
				{
                    excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductRemainderFormatInvalid));
				}

				var duplicateFound = excelItems.LastOrDefault(item => item.RowNumber < excelItem.RowNumber && !string.IsNullOrWhiteSpace(item.ProductName) && item.ProductName == excelItem.ProductName);
				if(duplicateFound != null)
				{
                    excelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "A", string.Format(Resources.ValidationExcelDuplicateItemFound, duplicateFound.RowNumber)));
				}
			}

			var hasExcelErrors = excelErrors.Any();
			var isError = hasExcelErrors;

			return (isError, hasExcelErrors, excelErrors);
		}

		async Task<(bool,string)> updateExcelItemProductIDs(List<productExcelItem> excelItems)
		{
			var isError = false;
			var errorMessage = default(string);

			var repository = _repositoryFactory.CreateProductsRepository();

            var products = await repository.ProductsList();
			if (repository.IsError)
			{
                isError = true;
                errorMessage = repository.ErrorMessage;
            }
			else
			{
                var productsDictionary = products.ToDictionary(Key => Key.ProductName, Value => Value.ProductID);

                foreach (var excelItem in excelItems)
                {
                    var isProductFound = productsDictionary.ContainsKey(excelItem.ProductName);
                    if (isProductFound)
                    {
                        excelItem.ProductID = productsDictionary[excelItem.ProductName].Value;
                    }
                    else
                    {
                        excelItem.ProductID = await repository.ProductsIUD(
                            databaseAction: Enums.DatabaseActions.INSERT,
                            productID: null,
                            product: new ProductIudDTO
                            {
                                ProductName = excelItem.ProductName
                            }
                        );
                        if (repository.IsError)
                        {
                            isError = true;
                            errorMessage = repository.ErrorMessage;
                            break;
                        }
                    }
                }
            }

			return (isError, errorMessage);
		}

        async Task<(bool, string)> syncPricesAndRemainders(List<productExcelItem> excelItems)
		{
			var isError = false;
			var errorMessage = default(string);

            var command = _repositoryFactory.CreateProductsRepository();

            foreach (var ExcelItem in excelItems)
			{
				await command.ProductsIUD(
					databaseAction: Enums.DatabaseActions.UPDATE,
					productID: ExcelItem.ProductID,
					product: new ProductIudDTO
					{
						ProductPrice = ExcelItem.ProductPrice,
						ProductRemainder = ExcelItem.ProductRemainder
					}        
				);
				if (command.IsError)
				{
					isError = true;
					errorMessage = command.ErrorMessage;
					break;
				}
			}

			return (isError, errorMessage);
		}
		#endregion

		#region Nested Classes
		class productExcelItem
		{
			#region Properties
			public int RowNumber { get; set; }
			public int? ProductID { get; set; }
			public string ProductName { get; set; }
			public string ProductPriceString { get; set; }
			public decimal? ProductPrice { get; set; }
			public string ProductRemainderString { get; set; }
			public int? ProductRemainder { get; set; }
            #endregion

            #region Methods
            public override string ToString()
            {
				return $"{ProductID} {ProductName} {ProductPriceString} {ProductRemainderString}";
            }
            #endregion
        }

        public class Result : BusinessLogicResultBase
		{
			#region Properties
			public List<string> ExcelErrors { get; set; }
			public bool HasExcelErrors { get; set; }
			#endregion
		}
		#endregion
	}		
}
