using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Core.DevexpressReports
{
    public partial class InvoiceReport : DevExpress.XtraReports.UI.XtraReport
    {
        //private Order O;
        //private SystemProperties P;
        //private UtilityCollection Utilities;
        //private string Language;

        public InvoiceReport()
        {
            InitializeComponent();
        }

        //public InvoiceReport(Order O, SystemProperties P, UtilityCollection Utilities, string Language = Constants.Languages.GEORGIAN)
        //{
        //    this.O = O;
        //    this.P = P;
        //    this.Utilities = Utilities;
        //    this.Language = Language;
        //    InitializeComponent();
        //    InitData();
        //}

        //void InitData()
        //{
        //    InvoiceCaptionLabel.Text = Resources.TextInvoice;
        //    OrderNumberCaptionCell.Text = $"{Resources.TextOrder}#";
        //    InvoiceDateCaptionLabel.Text = $"{Resources.TextDate}:";
        //    InvoiceDateLabel.Text = Utilities.FormatDate(O.OrderDateCreated);
        //    OrderIDLabel.Text = O.OrderID.ToString();

        //    CompanyPhoneCell.Text = P.ContactPhone;
        //    CompanyEmailCell.Text = P.ContactEmail;
        //    CompanyBankAccountCell.Text = $"{P.BankCode}, {P.BankAccountNumber}";
        //    CompanyAddressCell.Text = P.ContactAddress;

        //    CustomerNameCell.Text = string.IsNullOrWhiteSpace(O.OrderUserCompanyName) ? $"{O.OrderUserFirstname} {O.OrderUserLastname}" : $"{O.OrderUserCompanyName}, {O.OrderUserCompanyNumber}";
        //    CustomerPhoneCell.Text = O.OrderUserPhone;
        //    CustomerEmailCell.Text = O.OrderUserEmail;
        //    CustomerAddressCell.Text = O.OrderShippingAddressFull;

        //    ProductCaptionHeaderCell.Text = Resources.TextProduct;
        //    ProductQuantityHeaderCell.Text = Resources.TextQty;
        //    ProductUnitPriceHeaderCell.Text = Resources.TextUnitPrice;
        //    ProductPriceHeaderCell.Text = Resources.TextPrice;

        //    SubTotalCaptionCell.Text = Resources.TextSubTotal;
        //    ShippingCaptionCell.Text = Resources.TextShipping;
        //    DiscountCaptionCell.Text = Resources.TextDiscount;

        //    OrderTotalPriceCell.Text = Utilities.FormatPrice(O.OrderTotalPrice, WithCurrencySign: true);
        //    OrderShippingCell.Text = Utilities.FormatPrice(O.OrderShippingPrice, WithCurrencySign: true);
        //    OrderDiscountCell.Text = Utilities.FormatPrice(O.OrderDiscountedAmount < 0 ? O.OrderDiscountedAmount * -1 : 0, WithCurrencySign: true);
        //    OrderTotalPricePaidCell.Text = Utilities.FormatPrice(O.OrderTotalPricePaid, WithCurrencySign: true);

        //    DataSource = O.OrderDetails?.Select(Item => new
        //    {
        //        OrderDetailProductCaption = $"{Utilities.GetValuesByLanguage(Language, Item.OrderDetailProductName, Item.OrderDetailProductNameEng, Item.OrderDetailProductNameRus)} - {Item.OrderDetailProductCode}",
        //        OrderDetailProductPriceUnit = Utilities.FormatPrice(Item.OrderDetailProductUnitPrice, WithCurrencySign: true),
        //        OrderDetailProductQuantity = Utilities.FormatQuantity(Item.OrderDetailProductQuantity),
        //        OrderDetailProductPricePaid = Utilities.FormatPrice(Item.OrderDetailProductQuantity * Item.OrderDetailProductUnitPrice, WithCurrencySign: true)
        //    }).ToList();

        //    OrderDetailProductCaptionCell.DataBindings.Add("Text", DataSource, "OrderDetailProductCaption");
        //    OrderDetailProductPriceUnitCell.DataBindings.Add("Text", DataSource, "OrderDetailProductPriceUnit");
        //    OrderDetailProductQuantityCell.DataBindings.Add("Text", DataSource, "OrderDetailProductQuantity");
        //    OrderDetailProductPricePaidCell.DataBindings.Add("Text", DataSource, "OrderDetailProductPricePaid");

        //    ThankYouCell.Text = Resources.TextOrderInvoiceThankYou;
        //}
    }
}
