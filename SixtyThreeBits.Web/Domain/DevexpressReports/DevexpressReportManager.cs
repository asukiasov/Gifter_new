using SixtyThreeBits.Core.DevexpressReports;

namespace SixtyThreeBits.Core
{
    public class DevexpressReportManager
    {
        #region Properties
        //private SystemProperties SystemProperties;
        //private UtilityCollection Utilities;
        #endregion

        #region Constructors
        public DevexpressReportManager()
        {

        }

        //public DevexpressReportManager(SystemProperties SystemProperties, UtilityCollection Utilities)
        //{
        //    this.SystemProperties = SystemProperties;
        //    this.Utilities = Utilities;
        //}
        #endregion

        #region Methods
        public byte[] GetOrderInvoiceReportPdfBytes()
        {
            byte[] FileBytes = null;
            using (var ms = new System.IO.MemoryStream())
            {
                var Report = new InvoiceReport();
                Report.ExportToPdf(ms);
                FileBytes = ms.ToArray();
            }

            return FileBytes;
        }
        //public byte[] GetOrderInvoiceReportPdfBytes(Order O)
        //{
        //    byte[] FileBytes = null;
        //    using (var ms = new System.IO.MemoryStream())
        //    {
        //        var Report = new InvoiceReport(O, SystemProperties, Utilities);
        //        Report.ExportToPdf(ms);
        //        FileBytes = ms.ToArray();
        //    }

        //    return FileBytes;
        //}
        #endregion
    }
}
