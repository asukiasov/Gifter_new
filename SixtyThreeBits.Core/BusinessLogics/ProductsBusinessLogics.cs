using Azure;
using ExcelDataReader;
using OfficeOpenXml;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
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
			readonly AppSettingsCollection AppSettings;
            readonly DataAccessFactory DataAccessFactory;

			readonly GetProductsPricesAndRemaindersExcelFileResult Result = new GetProductsPricesAndRemaindersExcelFileResult();
            #endregion

            #region Constructors
            public GetProductsPricesAndRemaindersExcelFile(DataAccessFactory DataAccessFactory, AppSettingsCollection AppSettings)
            {
				this.AppSettings = AppSettings;
				this.DataAccessFactory = DataAccessFactory;
            }
            #endregion

            #region Methods
            public async Task<GetProductsPricesAndRemaindersExcelFileResult> Execute()
			{

				try
				{
					using (var Excel = new ExcelPackage(new FileInfo($"{AppSettings.DownloadFolderPhysicalPath}ProductsSync.xlsx")))
					{
						var WorkSheet = Excel.Workbook.Worksheets[0];

						var Products = await DataAccessFactory.Products.ProductsList();
						if (Products?.Any() == true)
						{
							var Index = 2;
							foreach (var P in Products)
							{
								WorkSheet.Cells[Index, 1].Value = P.ProductName;
								WorkSheet.Cells[Index, 2].Value = P.ProductPrice;
								WorkSheet.Cells[Index, 3].Value = P.ProductRemainder;
								++Index;
                            }
						}

						Result.ExcelFileBytes = Excel.GetAsByteArray();

						//Response.ContentType = "application/force-download";
						//Response.AppendHeader("Content-Disposition", "attachment;filename=UserCourseSubscriptionsImportTemplate.xlsx");

						//Response.BinaryWrite(FileBytes);
					}
				}
				catch(Exception ex)
				{
					Result.IsError = true;
					Result.ErrorMessage = ex.Message;
				}

				return Result;
            }
            #endregion

            #region Sub Classes
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
			readonly byte[] ExcelFileBytes;
			readonly bool IsXlsx;
			readonly DataAccessFactory DataAccessFactory;			

			Dictionary<string, int?> ProductsDictionary;
			List<ProductExcelItem> ExcelItems;

            readonly SyncProductPricesAndRemaindersResult Result = new SyncProductPricesAndRemaindersResult();
            #endregion

            #region Constructors
            public SyncProductPricesAndRemainders(byte[] ExcelFileBytes, bool IsXslx, DataAccessFactory DataAccessFactory)
			{
				this.ExcelFileBytes = ExcelFileBytes;
				this.IsXlsx = IsXslx;
				this.DataAccessFactory = DataAccessFactory;
			}
			#endregion

			#region Methods
			public async Task<SyncProductPricesAndRemaindersResult> Execute()
			{
				await InitBusinessLogicProperties();
				if (!Result.IsError)
				{
					ParseExcel();
					if (!Result.IsError)
					{
						ValidateExcel();
						if (!Result.IsError)
						{
							await InitProductIDs();
							if (!Result.IsError)
							{
								await SyncPricesAndRemainders();
							}
						}
					}
				}
				return Result;
			}

			async Task InitBusinessLogicProperties()
			{
				var Products = await DataAccessFactory.Products.ProductsList();
				if (Products == null)
				{
					Result.IsError = true;
					Result.ErrorMessage = DataAccessFactory.Products.ErrorMessage;
				}
				else
				{
					ProductsDictionary = Products.ToDictionary(Key => Key.ProductName, Value => Value.ProductID);
				}
			}

			void ParseExcel()
			{
				try
				{
					using (var InputStream = new MemoryStream(ExcelFileBytes))
					{
						using (var ExcelReader = IsXlsx ? ExcelReaderFactory.CreateOpenXmlReader(InputStream) : ExcelReaderFactory.CreateBinaryReader(InputStream))
						{
							ExcelReader.Read();

							ExcelItems = new List<ProductExcelItem>(ExcelReader.ResultsCount);
							var RowNumber = 1;
							while (ExcelReader.Read())
							{
								var Item = new ProductExcelItem();
								Item.RowNumber = ++RowNumber;
								Item.ProductName = ExcelReader.GetValue(0)?.ToString().Trim();

								Item.ProductPriceString = ExcelReader.GetValue(1)?.ToString().Trim();
								Item.ProductPrice = Item.ProductPriceString.ToDecimal();

								Item.ProductRemainderString = ExcelReader.GetValue(2)?.ToString().Trim();
								Item.ProductRemainder = Item.ProductRemainderString.ToInt();

								ExcelItems.Add(Item);
							}
						}
					}
				}
				catch(Exception ex)
				{
					Result.IsError = true;
					Result.ErrorMessage = ex.Message;
				}
			}

			void ValidateExcel()
			{
				var ErrorStringTemplateLineColumn = "Line {0} - Column {1} - {2}";
				Result.ExcelErrors = new List<string>();

				foreach(var ExcelItem in ExcelItems)
				{
					if (string.IsNullOrWhiteSpace(ExcelItem.ProductName))
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "A", Resources.ValidationProductNameRequired));
					}

					if (string.IsNullOrWhiteSpace(ExcelItem.ProductPriceString))
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "B", Resources.ValidationProductPriceRequired));
					}
					else if(ExcelItem.ProductPrice is null || ExcelItem.ProductPrice < 0)
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "B", Resources.ValidationProductPriceFormatInvalid));
					}

					if (string.IsNullOrWhiteSpace(ExcelItem.ProductRemainderString))
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "B", Resources.ValidationProductRemainderRequired));
					}
					else if (ExcelItem.ProductRemainder is null || ExcelItem.ProductRemainder < 0)
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "B", Resources.ValidationProductRemainderFormatInvalid));
					}

					var DuplicateFound = ExcelItems.LastOrDefault(Item => Item.RowNumber < ExcelItem.RowNumber && !string.IsNullOrWhiteSpace(Item.ProductName) && Item.ProductName == ExcelItem.ProductName);
					if(DuplicateFound != null)
					{
						Result.ExcelErrors.Add(string.Format(ErrorStringTemplateLineColumn, ExcelItem.RowNumber, "A", string.Format(Resources.ValidationExcelDuplicateItemFound, DuplicateFound.RowNumber)));
					}
				}

				Result.HasExcelErrors = Result.ExcelErrors.Any();
				Result.IsError = Result.HasExcelErrors;
			}

			async Task InitProductIDs()
			{
				foreach(var ExcelItem in ExcelItems)
				{
					var IsProductFound = ProductsDictionary.ContainsKey(ExcelItem.ProductName);
					if (IsProductFound)
					{
						ExcelItem.ProductID = ProductsDictionary[ExcelItem.ProductName].Value;
					}
					else
					{
						ExcelItem.ProductID = await DataAccessFactory.Products.ProductsIUD(
							DatabaseAction: Enums.DatabaseActions.CREATE,
							ProductName: ExcelItem.ProductName
						);
						if (DataAccessFactory.Products.IsError)
						{
							Result.IsError = true;
							Result.ErrorMessage = DataAccessFactory.Products.ErrorMessage;
							break;
						}
					}
				}
			}

			async Task SyncPricesAndRemainders()
			{
				foreach (var ExcelItem in ExcelItems)
				{
					await DataAccessFactory.Products.ProductsIUD(
						DatabaseAction: Enums.DatabaseActions.UPDATE,
						ProductID: ExcelItem.ProductID,
						ProductPrice: ExcelItem.ProductPrice,
						ProductRemainder: ExcelItem.ProductRemainder
					);
					if (DataAccessFactory.Products.IsError)
					{
						Result.IsError = true;
						Result.ErrorMessage = DataAccessFactory.Products.ErrorMessage;
						break;
					}
				}
			}
			#endregion

			#region Sub Classes
			class ProductExcelItem
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
