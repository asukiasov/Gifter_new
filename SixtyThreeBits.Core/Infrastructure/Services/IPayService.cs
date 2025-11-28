using Newtonsoft.Json;
using RestSharp;
using SixtyThreeBits.Core.Infrastructure.Services.Base;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SixtyThreeBits.Core.Infrastructure.Services
{
    public class IPayService : RestApiServiceBase
    {
        #region Properties
        readonly string _clientID;
        readonly string _secretKey;
        readonly UtilityCollection _utilities;

        const string _baseUrl = "https://ipay.ge/opay/";
        const string _baseUrlInstallment = "https://installment.bog.ge/";

        static string _accessToken;
        static string _accessTokenType;
        static DateTime? _accessTokenExpirationDate;
        #endregion

        #region Constructor        
        public IPayService(string clientID, string secretKey, UtilityCollection utilities) : base(_baseUrl)
        {
            _clientID = clientID;
            _secretKey = secretKey;
            _utilities = utilities;
        }
        #endregion

        #region Methods
        async Task InitAccessToken()
        {
            if (!(_accessTokenExpirationDate > DateTime.Now))
            {
                var headers = new List<Parameter>(2)
                {
                    new (Key:"Content-Type", Value: "application/x-www-form-urlencoded"),
                    new (Key:"Authorization", Value: $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientID}:{_secretKey}"))}"),
                };
                var parameters = new List<Parameter>(1)
                {
                    new (Key: "grant_type", Value: "client_credentials")
                };


                var result = await ExecuteAsyncTask(
                    resource: "api/v1/oauth2/token",
                    method: Method.Post,
                    headers: headers,
                    parameters: parameters                                
                );

                var responseData = result.ResponseContent.DeserializeJsonTo<AccessTokenResponse>();
                if (responseData != null)
                {
                    _accessToken = responseData.AccessToken;
                    _accessTokenType = responseData.TokenType;
                    _accessTokenExpirationDate = DateTimeOffset.FromUnixTimeMilliseconds(responseData.ExpiresIn).LocalDateTime;
                }
            }
        }

        public async Task<RegisterTransactionResult> RegisterTransactionTaskAsync(decimal? transactionAmount, int? orderID, string description, string urlBackToWebsite)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {
                new (Key: "Content-Type", Value: "application/json"),
                new (Key: "Authorization",Value: $"{_accessTokenType} {_accessToken}")
            };

            var Body = new
            {
                intent = "AUTHORIZE",
                redirect_url = urlBackToWebsite,
                shop_order_id = $"{orderID}",
                show_shop_order_id_on_extract = true,
                capture_method = "AUTOMATIC",
                items = new[]
                {
                    new
                    {
                        amount = _utilities.FormatPriceValue(transactionAmount),
                        description = "",
                        quantity = 1,
                        product_id = orderID
                    }
                },
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            value = _utilities.FormatPriceValue(transactionAmount),
                            currency_code = "GEL"
                        }
                    }
                }
            }.ToJson();

            var resultBase = await ExecuteAsyncTask(
                resource: "api/v1/checkout/orders",
                method: Method.Post,
                headers: headers,                
                body: Body
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<RegisterTransactionResponse>();

            var result = new RegisterTransactionResult(resultBase);
            if (responseData != null)
            {
                if (responseData.Status == "CREATED")
                {
                    var orderIDBank = responseData.OrderIDBank;
                    var redirectUrlPayment = responseData.Links?.FirstOrDefault(item => item.Method == "REDIRECT")?.Href;

                    if (!string.IsNullOrWhiteSpace(orderIDBank) && !string.IsNullOrWhiteSpace(redirectUrlPayment))
                    {
                        result.IsSuccess = true;
                        result.OrderIDBank = orderIDBank;
                        result.RedirectUrlPayment = redirectUrlPayment;
                    }
                }
            }

            return result;
        }

        public async Task<ExecuteSubscriptionPaymentResult> ExecuteSubscriptionPayment(string orderPaymentGatewayTransactionID, decimal transactionAmount, string currencyISOCode, int? orderID)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {
                new (Key: "Content-Type", Value: "application/json"),
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}"),
            };

            var body = new
            {
                order_id = orderPaymentGatewayTransactionID,
                amount = new
                {
                    currency_code = currencyISOCode,
                    value = _utilities.FormatPriceValue(transactionAmount)
                },
                shop_order_id = orderID
            }.ToJson();

            var resultBase = await ExecuteAsyncTask(
                resource: "api/v1/checkout/payment/subscription",
                method: Method.Post,
                headers: headers,
                body: body
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<ExecuteSubscriptionPaymentResponse>();

            var result = new ExecuteSubscriptionPaymentResult(resultBase);
            if (responseData != null)
            {
                result.OrderIDBank = responseData.OrderIDBank;
                result.IsSuccess = true;
            }

            return result;
        }

        public async Task<GetTransactionStatusResult> GetTransactionStatus(string orderIDBank)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {                
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}"),
            };

            var resultBase = await ExecuteAsyncTask(
                resource: $"api/v1/checkout/payment/{orderIDBank}",
                method: Method.Get,
                headers: headers
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<GetTransactionStatusResponse>();

            var result = new GetTransactionStatusResult(resultBase);
            if (responseData != null)
            {
                result.IsSuccess = true;
                result.IsPaid = responseData.Status == "success";
            }

            return result;
        }

        public async Task<RefundResult> Refund(string orderIDBank, decimal? transactionAmount)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {
                new (Key: "Content-Type", Value: "application/x-www-form-urlencoded"),
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}")
            };

            var parameters = new List<Parameter>
            {
                new (Key:  "order_id", Value: orderIDBank),
                new (Key: "amount", Value: _utilities.FormatPriceValue(transactionAmount))
            };

            var resultBase = await ExecuteAsyncTask(
                resource: "api/v1/checkout/refund",
                method: Method.Post,
                headers: headers,
                parameters: parameters
            );

            var result = new RefundResult(resultBase);            

            return result;
        }

        public async Task<InstallmentCalculateResult> InstallmentCalculate(decimal? transactionAmount)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {
                new (Key: "Content-Type", Value: "application/json"),
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}"),
            };

            var parameters = new List<Parameter>
            {
                new (Key:"amount" , Value: _utilities.FormatPriceValue(transactionAmount)),
                new (Key:"client_id " , Value:  _clientID)
            };

            SetBaseUrl(_baseUrlInstallment);
            var resultBase = await ExecuteAsyncTask(
                resource: "v1/services/installment/calculate",
                method: Method.Post,
                headers: headers,
                parameters: parameters                
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<InstallmentCalculateResponse>();
            var result = new InstallmentCalculateResult(resultBase);            

            if (responseData != null)
            {
                result.InstallmentItems = responseData.InstallmentItems?.Select(item => new InstallmentCalculateResult.InstallmentItem
                {
                    Month = item.Month,
                    Amount = item.Amount,
                    DiscountCode = item.DiscountCode
                }).ToList();
            }

            return result;
        }

        public async Task<InstallmentCheckoutResult> InstallmentCheckout(decimal? transactionAmount, int? installmentMonth, string installmentType, int? orderID, decimal? orderTotalPricePaid, string orderDescription, string urlBackToWebsite)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {
                new (Key: "Content-Type", Value: "application/json"),
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}"),
            };

            var body = new
            {
                intent = "LOAN",
                installment_month = installmentMonth,
                installment_type = installmentType,
                shop_order_id = orderID,
                success_redirect_url = urlBackToWebsite,
                fail_redirect_url = urlBackToWebsite,
                reject_redirect_url = urlBackToWebsite,
                validate_items = true,
                locale = "ka",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "GEL",
                            value = _utilities.FormatPriceValue(transactionAmount),
                        }
                    }
                },
                cart_items = new[]
                {
                    new
                    {
                        total_item_amount = _utilities.FormatPriceValue(orderTotalPricePaid),
                        item_description = orderDescription,
                        total_item_qty = 1,
                        item_vendor_code = orderID
                    }
                }
            }.ToJson();

            SetBaseUrl(_baseUrlInstallment);
            var resultBase = await ExecuteAsyncTask(
                resource: "v1/installment/checkout",
                method: Method.Post,
                headers: headers,
                body: body
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<InstallmentCheckoutResponse>();

            var result = new InstallmentCheckoutResult(resultBase);
            if (responseData != null)
            {
                if (responseData.Status == "CREATED")
                {
                    var orderIDBank = responseData.OrderIDBank;
                    var redirectUrlPayment = responseData.Links?.FirstOrDefault(item => item.Method == "REDIRECT")?.Href;

                    if (!string.IsNullOrWhiteSpace(orderIDBank) && !string.IsNullOrWhiteSpace(redirectUrlPayment))
                    {
                        result.IsSuccess = true;
                        result.OrderIDBank = orderIDBank;
                        result.RedirectUrlPayment = redirectUrlPayment;
                    }
                }
            }

            return result;
        }

        public async Task<InstallmentGetTransactionStatusResult> InstallmentGetStatus(string orderIDBank)
        {
            await InitAccessToken();

            var headers = new List<Parameter>(2)
            {                
                new (Key: "Authorization", Value: $"{_accessTokenType} {_accessToken}"),
            };

            SetBaseUrl(_baseUrlInstallment);
            var resultBase = await ExecuteAsyncTask(
                resource: $"v1/installment/checkout/{orderIDBank}",
                method: Method.Get,
                headers: headers
            );
            var responseData = resultBase.ResponseContent.DeserializeJsonTo<InstallmentGetTransactionStatusResponse>();

            var result = new InstallmentGetTransactionStatusResult(resultBase);
            if (responseData != null)
            {
                if (responseData.Status == "success")
                {
                    result.IsSuccess = true;
                    result.IsPaid = true;
                }
            }

            return result;
        }
        #endregion

        #region Nested Classes
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

            #region Nested Classes
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

            #region Nested Classes
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

            #region Nested Classes
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

            #region Nested Classes
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