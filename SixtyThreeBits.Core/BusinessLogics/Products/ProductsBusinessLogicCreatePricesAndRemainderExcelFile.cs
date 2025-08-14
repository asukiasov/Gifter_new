using OfficeOpenXml;
using SixtyThreeBits.Core.BusinessLogics.Base;
using SixtyThreeBits.Core.Factories;
using SixtyThreeBits.Core.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.BusinessLogics
{
	public class ProductsBusinessLogicCreatePricesAndRemainderExcelFile
    {
		#region Properties
		readonly RepositoryFactory _dataAccessFactory;
		readonly AppSettingsCollection _appSettings;
		#endregion

		#region Constructors
		public ProductsBusinessLogicCreatePricesAndRemainderExcelFile(RepositoryFactory dataAccessFactory, AppSettingsCollection appSettings)
		{
			_dataAccessFactory = dataAccessFactory;
			_appSettings = appSettings;
		}
		#endregion

		#region Methods
		public async Task<Result> Execute()
		{
			var result = new Result();

            (result.ExcelFileBytes, result.IsError, result.ErrorMessage) = await createProductsExcel();

			return result;
		}

		async Task<(byte[], bool, string)> createProductsExcel()
		{
            var excelBytes = default(byte[]);
            var isError = false;
            var errorMessage = default(string);

            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("63NonCommercial");
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

                    excelBytes = excel.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage = ex.Message;
            }

            return (excelBytes, isError, errorMessage);
        }
		#endregion

		#region Nested Classes
		public class Result : BusinessLogicResultBase
		{
			#region Properties
			public byte[] ExcelFileBytes { get; set; }
			#endregion
		}
		#endregion
	}
}