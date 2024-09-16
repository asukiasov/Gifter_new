using ExcelDataReader;
using OfficeOpenXml;
using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Infrastructure.Repositories;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.BusinessLogics
{
    public class ProductsBusinessLogic
	{
		public class GetProductsPricesAndRemaindersExcelFile
		{
			#region Properties
			readonly RepositoryFactory _dataAccessFactory;

            readonly AppSettingsCollection _appSettings;			
            readonly GetProductsPricesAndRemaindersExcelFileResult _result = new ();
            #endregion

            #region Constructors
            public GetProductsPricesAndRemaindersExcelFile(RepositoryFactory dataAccessFactory, AppSettingsCollection appSettings)
            {
				_dataAccessFactory = dataAccessFactory;
                _appSettings = appSettings;
				
            }
            #endregion

            #region Methods
            public async Task<GetProductsPricesAndRemaindersExcelFileResult> Execute()
			{
				try
				{
					using (var excel = new ExcelPackage(new FileInfo($"{_appSettings.DownloadFolderPhysicalPath}\\ProductsSync.xlsx")))
					{
						var workSheet = excel.Workbook.Worksheets[0];
						var repository = _dataAccessFactory.CreateProductsRepository();													
						var products = await repository.ProductsList();

						if (products?.Any() == true)
						{
							var index = 2;
							foreach (var p in products)
							{
								workSheet.Cells[index, 1].Value = p.ProductName;
								workSheet.Cells[index, 2].Value = p.ProductPrice;
								workSheet.Cells[index, 3].Value = p.ProductRemainder;
								++index;
                            }
						}
						_result.ExcelFileBytes = excel.GetAsByteArray();
					}
				}
				catch(Exception ex)
				{
					_result.IsError = true;
					_result.ErrorMessage = ex.Message;
				}

				return _result;
            }
            #endregion

            #region Nested Classes
			public class GetProductsPricesAndRemaindersExcelFileResult : BusinessLogicResultBase
			{
				#region Properties
				public byte[] ExcelFileBytes { get; set; }
				#endregion
			}
            #endregion
        }

        public class SyncProductPricesAndRemainders
		{
			#region Properties
			readonly byte[] _excelFileBytes;
			readonly bool _isXlsx;
			readonly RepositoryFactory _repositoryFactory;

            Dictionary<string, int?> _productsDictionary;
			List<productExcelItem> _excelItems;

            readonly SyncProductPricesAndRemaindersResult _result = new();
            #endregion

            #region Constructors
            public SyncProductPricesAndRemainders(byte[] excelFileBytes, bool isXslx, RepositoryFactory dataAccessFactory)
			{
				_excelFileBytes = excelFileBytes;
				_isXlsx = isXslx;
				_repositoryFactory = dataAccessFactory;
            }
			#endregion

			#region Methods
			public async Task<SyncProductPricesAndRemaindersResult> Execute()
			{
				await initBusinessLogicProperties();
				if (!_result.IsError)
				{
					parseExcel();
					if (!_result.IsError)
					{
						validateExcel();
						if (!_result.IsError)
						{
							await initProductIDs();
							if (!_result.IsError)
							{
								await syncPricesAndRemainders();
							}
						}
					}
				}
				return _result;
			}

			async Task initBusinessLogicProperties()
			{
				var repository = _repositoryFactory.CreateProductsRepository();
				var products = await repository.ProductsList();
				if (products == null)
				{
					_result.IsError = true;
					_result.ErrorMessage = repository.ErrorMessage;
				}
				else
				{
					_productsDictionary = products.ToDictionary(Key => Key.ProductName, Value => Value.ProductID);
				}
			}

			void parseExcel()
			{
				try
				{
					using (var inputStream = new MemoryStream(_excelFileBytes))
					{
						using (var excelReader = _isXlsx ? ExcelReaderFactory.CreateOpenXmlReader(inputStream) : ExcelReaderFactory.CreateBinaryReader(inputStream))
						{
							excelReader.Read();

							_excelItems = new List<productExcelItem>(excelReader.ResultsCount);
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

								_excelItems.Add(item);
							}
						}
					}
				}
				catch(Exception ex)
				{
					_result.IsError = true;
					_result.ErrorMessage = ex.Message;
				}
			}

			void validateExcel()
			{
				var errorStringTemplateLineColumn = "Line {0} - Column {1} - {2}";
				_result.ExcelErrors = [];

				foreach(var excelItem in _excelItems)
				{
					if (string.IsNullOrWhiteSpace(excelItem.ProductName))
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "A", Resources.ValidationProductNameRequired));
					}

					if (string.IsNullOrWhiteSpace(excelItem.ProductPriceString))
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductPriceRequired));
					}
					else if(excelItem.ProductPrice is null || excelItem.ProductPrice < 0)
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductPriceFormatInvalid));
					}

					if (string.IsNullOrWhiteSpace(excelItem.ProductRemainderString))
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductRemainderRequired));
					}
					else if (excelItem.ProductRemainder is null || excelItem.ProductRemainder < 0)
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "B", Resources.ValidationProductRemainderFormatInvalid));
					}

					var duplicateFound = _excelItems.LastOrDefault(item => item.RowNumber < excelItem.RowNumber && !string.IsNullOrWhiteSpace(item.ProductName) && item.ProductName == excelItem.ProductName);
					if(duplicateFound != null)
					{
						_result.ExcelErrors.Add(string.Format(errorStringTemplateLineColumn, excelItem.RowNumber, "A", string.Format(Resources.ValidationExcelDuplicateItemFound, duplicateFound.RowNumber)));
					}
				}

				_result.HasExcelErrors = _result.ExcelErrors.Any();
				_result.IsError = _result.HasExcelErrors;
			}

			async Task initProductIDs()
			{
				var repository = _repositoryFactory.CreateProductsRepository();

				foreach(var excelItem in _excelItems)
				{
					var isProductFound = _productsDictionary.ContainsKey(excelItem.ProductName);
					if (isProductFound)
					{
						excelItem.ProductID = _productsDictionary[excelItem.ProductName].Value;
					}
					else
					{
						excelItem.ProductID = await repository.ProductsIUD(
							databaseAction: Enums.DatabaseActions.CREATE,
							productID: null,
							product: new ProductIudDTO
							{
                                ProductName = excelItem.ProductName
                            }							
						);
						if (repository.IsError)
						{
							_result.IsError = true;
							_result.ErrorMessage = repository.ErrorMessage;
							break;
						}
					}
				}
			}

			async Task syncPricesAndRemainders()
			{
                var command = _repositoryFactory.CreateProductsRepository();

                foreach (var ExcelItem in _excelItems)
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
						_result.IsError = true;
						_result.ErrorMessage = command.ErrorMessage;
						break;
					}
				}
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

            public class SyncProductPricesAndRemaindersResult : BusinessLogicResultBase
			{
				#region Properties
				public List<string> ExcelErrors { get; set; }
				public bool HasExcelErrors { get; set; }
				#endregion
			}
			#endregion
		}		
	}	
}
