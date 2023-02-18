using Newtonsoft.Json;
using RestSharp;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SixtyThreeBits.Core.Services
{
    public class IPayService : ApiBase
    {
        #region Properties
        readonly string ClientID;
        readonly string SecretKey;
        readonly UtilityCollection Utilities;

        const string BaseUrl = "https://ipay.ge/opay/";
        const string BaseUrlInstallment = "https://installment.bog.ge/";

        static string AccessToken;
        static string AccessTokenType;
        static DateTime? AccessTokenExpirationDate;
        #endregion

        #region Constructor        
        public IPayService(string ClientID, string SecretKey, UtilityCollection Utilities) : base(BaseUrl)
        {            
            this.ClientID = ClientID;
            this.SecretKey = SecretKey;
            this.Utilities = Utilities;
        }
        #endregion

        #region Methods
        async Task InitAccessToken()
        {
            if (!(AccessTokenExpirationDate > DateTime.Now))
            {
                var Headers = new List<SimpleKeyValue<string, string>>(2)
                {
                    new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/x-www-form-urlencoded"},
                    new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientID}:{SecretKey}"))}"},
                };
                var Parameters = new List<SimpleKeyValue<string, string>>(1)
                {
                    new SimpleKeyValue<string, string>{ Key = "grant_type", Value = "client_credentials"},
                };


                var Result = await ExecuteAsyncTask(
                    Resource: "api/v1/oauth2/token",
                    Method: Method.Post,
                    Headers: Headers,
                    Parameters: Parameters,
                    Body: null
                );

                var ResponseData = Result.ResponseContent.DeserializeJsonTo<AccessTokenResponse>();
                if (ResponseData != null)
                {
                    AccessToken = ResponseData.AccessToken;
                    AccessTokenType = ResponseData.TokenType;
                    AccessTokenExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(ResponseData.ExpiresIn).LocalDateTime;
                }
            }
        }

        public async Task<RegisterTransactionResult> RegisterTransactionTaskAsync(decimal? TransactionAmount, int? OrderID, string Description, string UrlBackToWebsite)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/json"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };
            
            var Body = new
            {
                intent = "AUTHORIZE",
                redirect_url = UrlBackToWebsite,
                shop_order_id = $"{OrderID}",
                show_shop_order_id_on_extract = true,
                capture_method = "AUTOMATIC",
                items = new[]
                {
                    new
                    {
                        amount = Utilities.FormatPriceValue(TransactionAmount),
                        description = "",
                        quantity = 1,
                        product_id = OrderID
                    }
                },
                purchase_units = new[]
                {
                    new
                    {                        
                        amount = new
                        {
                            value = Utilities.FormatPriceValue(TransactionAmount),
                            currency_code = "GEL"
                        }
                    }
                }
            }.ToJson();

            var ResultBase = await ExecuteAsyncTask(
                Resource: "api/v1/checkout/orders",
                Method: Method.Post,
                Headers: Headers,
                Parameters: null,
                Body: Body
            );
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<RegisterTransactionResponse>();

            var Result = new RegisterTransactionResult(ResultBase);
            if (ResponseData != null)
            {
                if (ResponseData.Status == "CREATED")
                {
                    var OrderIDBank = ResponseData.OrderIDBank;
                    var RedirectUrlPayment = ResponseData.Links?.FirstOrDefault(Item => Item.Method == "REDIRECT")?.Href;

                    if(!string.IsNullOrWhiteSpace(OrderIDBank) && !string.IsNullOrWhiteSpace(RedirectUrlPayment))
                    {
                        Result.IsSuccess = true;
                        Result.OrderIDBank = OrderIDBank;
                        Result.RedirectUrlPayment = RedirectUrlPayment;
                    }
                }
            }
                        
            return Result;
        }

        public async Task<ExecuteSubscriptionPaymentResult> ExecuteSubscriptionPayment(string OrderPaymentGatewayTransactionID, decimal TransactionAmount, string CurrencyISOCode, int? OrderID)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/json"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };

                        
            var Body = new
            {
                order_id = OrderPaymentGatewayTransactionID,
                amount = new
                {
                    currency_code = CurrencyISOCode,
                    value = Utilities.FormatPriceValue(TransactionAmount)
                },
                shop_order_id = OrderID
            }.ToJson();

            var ResultBase = await ExecuteAsyncTask(
                Resource: "api/v1/checkout/payment/subscription",
                Method: Method.Post,
                Headers: Headers,
                Parameters: null,
                Body: Body
            );
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<ExecuteSubscriptionPaymentResponse>();

            var Result = new ExecuteSubscriptionPaymentResult(ResultBase);            
            if (ResponseData != null)
            {
                Result.OrderIDBank = ResponseData.OrderIDBank;
                Result.IsSuccess = true;
            }

            return Result;
        }

        public async Task<GetTransactionStatusResult> GetTransactionStatus(string OrderIDBank)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                //new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/json"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };
            
            var ResultBase = await ExecuteAsyncTask(
                Resource: $"api/v1/checkout/payment/{OrderIDBank}",
                Method: Method.Get,
                Headers: Headers,
                Parameters: null,
                Body: null
            );            
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<GetTransactionStatusResponse>();

            var Result = new GetTransactionStatusResult(ResultBase);
            if (ResponseData != null)
            {
                Result.IsSuccess = true;
                Result.IsPaid = ResponseData.Status == "success";                
            }

            return Result;
        }

        public async Task<RefundResult> Refund(string OrderIDBank, decimal? TransactionAmount)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/x-www-form-urlencoded"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };

            var Parameters = new List<SimpleKeyValue<string, string>>
            {
                new SimpleKeyValue<string, string>{ Key = "order_id", Value=OrderIDBank},
                new SimpleKeyValue<string, string>{ Key = "amount", Value = Utilities.FormatPriceValue(TransactionAmount) },
            };

            var ResultBase = await ExecuteAsyncTask(
                Resource: "api/v1/checkout/refund",
                Method: Method.Post,
                Headers: Headers,
                Parameters: Parameters,
                Body: null
            );            

            var Result = new RefundResult(ResultBase);
            Result.IsSuccess = Result.ResponseStatusCode == 200;

            return Result;
        }

        public async Task<InstallmentCalculateResult> InstallmentCalculate(decimal? TransactionAmount)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/json"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };
            
            //var Body = new
            //{
            //    amount = Utility.FormatPriceValue(TransactionAmount),
            //    client_id = ClientID
            //}.ToJson();

            var Parameters = new List<SimpleKeyValue<string, string>>
            {
                new SimpleKeyValue<string,string>{ Key = "amount" , Value = Utilities.FormatPriceValue(TransactionAmount), },
                new SimpleKeyValue<string,string>{ Key = "client_id " , Value =  ClientID}
            };

            SetBaseUrl(BaseUrlInstallment);
            var ResultBase = await ExecuteAsyncTask(
                Resource: "v1/services/installment/calculate",
                Method: Method.Post,
                Headers: Headers,
                Parameters: Parameters,
                Body: null
            );
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<InstallmentCalculateResponse>();
            var Result = new InstallmentCalculateResult(ResultBase);

            ResultBase.ToJson().LogString();

            if (ResponseData != null)
            {
                Result.InstallmentItems = ResponseData.InstallmentItems?.Select(Item => new InstallmentCalculateResult.InstallmentItem
                {
                    Month = Item.Month,
                    Amount = Item.Amount,
                    DiscountCode = Item.DiscountCode
                }).ToList();
            }

            return Result;
        }

        public async Task<InstallmentCheckoutResult> InstallmentCheckout(decimal? TransactionAmount, int? InstallmentMonth, string InstallmentType, int? OrderID, decimal? OrderTotalPricePaid, string OrderDescription, string UrlBackToWebsite)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/json"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };

            var Body = new
            {
                intent = "LOAN",
                installment_month = InstallmentMonth,
                installment_type = InstallmentType,
                shop_order_id = OrderID,
                success_redirect_url = UrlBackToWebsite,
                fail_redirect_url = UrlBackToWebsite,
                reject_redirect_url = UrlBackToWebsite,
                validate_items = true,
                locale = "ka",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "GEL",
                            value = Utilities.FormatPriceValue(TransactionAmount),
                        }
                    }
                },
                cart_items = new[]
                {
                    new
                    {
                        total_item_amount = Utilities.FormatPriceValue(OrderTotalPricePaid),
                        item_description = OrderDescription,
                        total_item_qty = 1,
                        item_vendor_code = OrderID
                    }
                }
            }.ToJson();

            SetBaseUrl(BaseUrlInstallment);
            var ResultBase = await ExecuteAsyncTask(
                Resource: "v1/installment/checkout",
                Method: Method.Post,
                Headers: Headers,
                Parameters: null,
                Body: Body
            );
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<InstallmentCheckoutResponse>();

            var Result = new InstallmentCheckoutResult(ResultBase);
            if (ResponseData != null)
            {
                if (ResponseData.Status == "CREATED")
                {
                    var OrderIDBank = ResponseData.OrderIDBank;
                    var RedirectUrlPayment = ResponseData.Links?.FirstOrDefault(Item => Item.Method == "REDIRECT")?.Href;

                    if (!string.IsNullOrWhiteSpace(OrderIDBank) && !string.IsNullOrWhiteSpace(RedirectUrlPayment))
                    {
                        Result.IsSuccess = true;
                        Result.OrderIDBank = OrderIDBank;
                        Result.RedirectUrlPayment = RedirectUrlPayment;
                    }
                }
            }

            return Result;
        }

        public async Task<InstallmentGetTransactionStatusResult> InstallmentGetStatus(string OrderIDBank)
        {
            await InitAccessToken();

            var Headers = new List<SimpleKeyValue<string, string>>(2)
            {
                //new SimpleKeyValue<string, string>{ Key = "Content-Type", Value = "application/x-www-form-urlencoded"},
                new SimpleKeyValue<string, string>{ Key = "Authorization", Value = $"{AccessTokenType} {AccessToken}"},
            };

            SetBaseUrl(BaseUrlInstallment);
            var ResultBase = await ExecuteAsyncTask(
                Resource: $"v1/installment/checkout/{OrderIDBank}",
                Method: Method.Get,
                Headers: Headers,
                Parameters: null,
                Body: null
            );
            var ResponseData = ResultBase.ResponseContent.DeserializeJsonTo<InstallmentGetTransactionStatusResponse>();

            var Result = new InstallmentGetTransactionStatusResult(ResultBase);
            if (ResponseData != null)
            {
                if (ResponseData.Status == "success")
                {
                    Result.IsSuccess = true;
                    Result.IsPaid = true;
                }
            }

            return Result;
        }
        #endregion

        #region Sub Classes
        class AccessTokenResponse
        {
            #region Properties
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("app_id")]
            public string AppID { get; set; }
            [JsonProperty("expires_in")]
            public long ExpiresIn { get; set; }
            #endregion
        }        

        class RegisterTransactionResponse
        {
            #region Properties
            [JsonProperty("order_id")]
            public string OrderIDBank { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("links")]
            public List<Link> Links { get; set; }
            #endregion

            #region Sub Classes
            public class Link
            {
                #region Properties
                [JsonProperty("href")]
                public string Href { get; set; }

                [JsonProperty("rel")]
                public string Rel { get; set; }

                [JsonProperty("method")]
                public string Method { get; set; }
                #endregion
            }
            #endregion
        }
        public class RegisterTransactionResult : ApiResultBase
        {
            #region Constructors            

            public RegisterTransactionResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Properties
            public string OrderIDBank { get; set; }
            public string RedirectUrlPayment { get; set; }
            #endregion
        }

        class ExecuteSubscriptionPaymentResponse
        {
            #region Properties            
            [JsonProperty("order_id")]
            public string OrderIDBank { get; set; }            
            #endregion

        }
        public class ExecuteSubscriptionPaymentResult : ApiResultBase
        {
            #region Constructors            
            public ExecuteSubscriptionPaymentResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Properties                        
            public string OrderIDBank { get; set; }
            #endregion
        }

        class GetTransactionStatusResponse
        {
            #region Properties
            [JsonProperty("status")]
            public string Status { get; set; }            
            #endregion
        }
        public class GetTransactionStatusResult : ApiResultBase
        {
            #region Constructors            
            public GetTransactionStatusResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Properties
            public bool IsPaid { get; set; }
            #endregion
        }

        public class RefundResult : ApiResultBase
        {
            #region Constructors            
            public RefundResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion
        }

        class InstallmentCalculateResponse
        {
            #region Properties
            public List<InstallmentItem> InstallmentItems { get; set; }
            #endregion

            #region Sub Classes
            public class InstallmentItem
            {
                #region Properties
                [JsonProperty("month")]
                public int? Month { get; set; }
                [JsonProperty("amount")]
                public decimal? Amount { get; set; }
                [JsonProperty("discount_code")]
                public decimal? DiscountCode { get; set; }
                #endregion
            }
            #endregion
        }
        public class InstallmentCalculateResult : ApiResultBase
        {
            #region Properties
            public List<InstallmentItem> InstallmentItems { get; set; }
            #endregion

            #region Constructors            
            public InstallmentCalculateResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Sub Classes
            public class InstallmentItem
            {
                #region Properties
                public int? Month { get; set; }
                public decimal? Amount { get; set; }
                public decimal? DiscountCode { get; set; }
                #endregion
            }
            #endregion
        }

        class InstallmentCheckoutResponse
        {
            #region Properties
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("order_id")]
            public string OrderIDBank { get; set; }
            public List<Link> Links { get; set; }
            #endregion

            #region Sub Classes
            public class Link
            {
                #region Properties
                [JsonProperty("href")]
                public string Href { get; set; }
                [JsonProperty("rel")]
                public string Rel { get; set; }
                [JsonProperty("method")]
                public string Method { get; set; }
                #endregion
            }
            #endregion
        }
        public class InstallmentCheckoutResult : ApiResultBase
        {
            #region Constructors            
            public InstallmentCheckoutResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Properties
            public string OrderIDBank { get; set; }
            public string RedirectUrlPayment { get; set; }
            #endregion
        }

        class InstallmentGetTransactionStatusResponse
        {
            #region Properties
            [JsonProperty("status")]
            public string Status { get; set; }
            #endregion
        }
        public class InstallmentGetTransactionStatusResult : ApiResultBase
        {
            #region Constructors            
            public InstallmentGetTransactionStatusResult(ApiResultBase ResultBase) : base(ResultBase)
            {

            }
            #endregion

            #region Properties
            public bool IsPaid { get; set; }
            #endregion
        }
        #endregion
    }    
}