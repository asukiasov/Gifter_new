using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace SixtyThreeBits.Web.Services
{
    public class ScraperService
    {
        #region Nested Classes
        public class ProductData
        {
            public string Title { get; set; }
            public string ImageUrl { get; set; }
            public decimal? Price { get; set; }
            public string Currency { get; set; }
        }

        public class ScrapeResult : ProductData
        {
            public bool Success { get; set; }
            public string Url { get; set; }
            public string Error { get; set; }
            public List<string> Logs { get; set; } = new List<string>();
        }
        #endregion

        #region Fields
        private readonly HttpClient _httpClient;
        #endregion

        #region Constructors
        public ScraperService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            _httpClient.Timeout = TimeSpan.FromSeconds(25);
        }
        #endregion

        #region Methods
        public async Task<ScrapeResult> ScrapeAsync(string url)
        {
            var result = new ScrapeResult { Url = url };
            
            try
            {
                var productData = await ScrapeUrlAsync(url, result.Logs);
                
                result.Title = productData.Title;
                result.ImageUrl = productData.ImageUrl;
                result.Price = productData.Price;
                result.Currency = productData.Currency;
                result.Success = !string.IsNullOrEmpty(result.Title);

                if (!result.Success)
                {
                    result.Error = "Could not extract product information from any source";
                }
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                result.Logs.Add($"Error: {ex.Message}");
            }

            return result;
        }

        public async Task<ProductData> ScrapeUrlAsync(string url, List<string> logs = null)
        {
            logs ??= new List<string>();
            var product = new ProductData();

            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("URL is required");

            // Normalize URL - add https:// if missing
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                url = "https://" + url;
            }

            logs.Add($"Starting scrape for: {url}");
            var html = await _httpClient.GetStringAsync(url);
            logs.Add($"Fetched HTML ({html.Length} characters)");

            // Initialize AngleSharp
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var parser = context.GetService<IHtmlParser>();
            var document = await parser.ParseDocumentAsync(html);

            // 1. DataLayer / JSON-LD
            logs.Add("Attempting DataLayer/JSON-LD extraction...");
            ExtractFromJson(html, product, logs);

            // 2. Open Graph Meta Tags
            if (string.IsNullOrEmpty(product.Title) || !product.Price.HasValue || string.IsNullOrEmpty(product.ImageUrl))
            {
                logs.Add("Attempting Open Graph extraction...");
                ExtractFromOpenGraph(document, product, logs);
            }

            // 3. DOM Selectors (Final Fallback)
            if (string.IsNullOrEmpty(product.Title) || !product.Price.HasValue || string.IsNullOrEmpty(product.ImageUrl))
            {
                logs.Add("Attempting DOM Selector extraction...");
                ExtractFromDom(document, product, logs);
            }

            // Normalization
            NormalizeProduct(product, url);

            if (!string.IsNullOrEmpty(product.Title))
            {
                logs.Add($"Extraction complete. Title: {product.Title}, Price: {product.Price} {product.Currency}");
            }
            else
            {
                logs.Add("Extraction failed to find a valid product title.");
            }

            return product;
        }

        private void ExtractFromJson(string html, ProductData product, List<string> logs)
        {
            // 1.1 JSON-LD
            var jsonLdMatches = Regex.Matches(html, @"<script[^>]*type\s*=\s*[""']application/ld\+json[""'][^>]*>(.*?)</script>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            foreach (Match match in jsonLdMatches)
            {
                try
                {
                    var json = match.Groups[1].Value.Trim();
                    using var doc = JsonDocument.Parse(json);
                    IEnumerable<JsonElement> elements = doc.RootElement.ValueKind == JsonValueKind.Array 
                        ? doc.RootElement.EnumerateArray() 
                        : new[] { doc.RootElement };

                    foreach (var el in elements)
                    {
                        IEnumerable<JsonElement> productArray = el.TryGetProperty("@graph", out var graph) 
                            ? graph.EnumerateArray() 
                            : new[] { el };
                        foreach (var item in productArray)
                        {
                            if (item.TryGetProperty("@type", out var type) && IsProductType(type))
                            {
                                if (string.IsNullOrEmpty(product.Title)) product.Title = item.TryGetProperty("name", out var n) ? n.GetString() : null;
                                if (string.IsNullOrEmpty(product.ImageUrl)) product.ImageUrl = ExtractImageFromJsonLd(item);
                                
                                if (!product.Price.HasValue && item.TryGetProperty("offers", out var offers))
                                {
                                    var offer = offers.ValueKind == JsonValueKind.Array && offers.GetArrayLength() > 0 ? offers[0] : offers;
                                    if (offer.TryGetProperty("price", out var p)) product.Price = CleanPrice(p.ToString());
                                    if (string.IsNullOrEmpty(product.Currency)) product.Currency = offer.TryGetProperty("priceCurrency", out var c) ? c.GetString() : null;
                                }
                                logs.Add("Data found in JSON-LD");
                            }
                        }
                    }
                }
                catch { }
            }

            // 1.2 DataLayer and SPA state patterns
            var dataLayerPatterns = new[] {
                @"dataLayer\.push\s*\(\s*(\{.*?\})\s*\)",
                @"google_tag_params\s*=\s*(\{.*?\})",
                @"productDetail\s*=\s*(\{.*?\})",
                @"__NEXT_DATA__[^>]*>\s*(\{.*?\})\s*</script>",
                @"window\.__INITIAL_STATE__\s*=\s*(\{.*?\});",
                @"window\.__PRELOADED_STATE__\s*=\s*(\{.*?\});",
                @"""product""\s*:\s*(\{[^{}]*""name""[^{}]*\})",
                @"""productData""\s*:\s*(\{.*?\})"
            };

            foreach (var pattern in dataLayerPatterns)
            {
                var matches = Regex.Matches(html, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    try
                    {
                        var json = Regex.Replace(match.Groups[1].Value.Trim(), @",\s*}", "}");
                        using var doc = JsonDocument.Parse(json);
                        var root = doc.RootElement;

                        JsonElement item = default;
                        if (root.TryGetProperty("ecommerce", out var ecom))
                        {
                            if (ecom.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array && items.GetArrayLength() > 0) item = items[0];
                            else if (ecom.TryGetProperty("detail", out var detail) && detail.TryGetProperty("products", out var prods) && prods.ValueKind == JsonValueKind.Array && prods.GetArrayLength() > 0) item = prods[0];
                        }
                        else item = root;

                        if (item.ValueKind != JsonValueKind.Undefined)
                        {
                            if (string.IsNullOrEmpty(product.Title)) product.Title = item.TryGetProperty("item_name", out var n1) ? n1.GetString() : (item.TryGetProperty("name", out var n2) ? n2.GetString() : null);
                            if (!product.Price.HasValue) product.Price = item.TryGetProperty("price", out var p) ? CleanPrice(p.ToString()) : null;
                            if (string.IsNullOrEmpty(product.Currency)) product.Currency = item.TryGetProperty("currency", out var c1) ? c1.GetString() : (item.TryGetProperty("currencyCode", out var c2) ? c2.GetString() : null);
                            
                            if (!string.IsNullOrEmpty(product.Title)) logs.Add("Data found in DataLayer");
                        }
                    }
                    catch { }
                }
            }
        }

        private string ExtractImageFromJsonLd(JsonElement item)
        {
            if (item.TryGetProperty("image", out var img))
            {
                if (img.ValueKind == JsonValueKind.String) return img.GetString();
                if (img.ValueKind == JsonValueKind.Array && img.GetArrayLength() > 0) return img[0].ValueKind == JsonValueKind.String ? img[0].GetString() : (img[0].TryGetProperty("url", out var u) ? u.GetString() : null);
                if (img.TryGetProperty("url", out var u2)) return u2.GetString();
            }
            return null;
        }

        private bool IsProductType(JsonElement typeElement)
        {
            // Handle @type as string: "Product"
            if (typeElement.ValueKind == JsonValueKind.String)
            {
                return typeElement.GetString()?.Contains("Product") == true;
            }
            // Handle @type as array: ["Product", "ItemPage"]
            if (typeElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var t in typeElement.EnumerateArray())
                {
                    if (t.ValueKind == JsonValueKind.String && t.GetString()?.Contains("Product") == true)
                        return true;
                }
            }
            return false;
        }

        private void ExtractFromOpenGraph(IDocument document, ProductData product, List<string> logs)
        {
            var ogTitle = document.QuerySelector("meta[property='og:title']")?.GetAttribute("content") ?? document.QuerySelector("meta[name='twitter:title']")?.GetAttribute("content");
            if (string.IsNullOrEmpty(product.Title) && !string.IsNullOrEmpty(ogTitle))
            {
                product.Title = ogTitle;
                logs.Add("Title found in Open Graph");
            }

            var ogImage = document.QuerySelector("meta[property='og:image']")?.GetAttribute("content") ?? document.QuerySelector("meta[name='twitter:image']")?.GetAttribute("content");
            if (string.IsNullOrEmpty(product.ImageUrl) && !string.IsNullOrEmpty(ogImage))
            {
                product.ImageUrl = ogImage;
                logs.Add("Image found in Open Graph");
            }

            if (!product.Price.HasValue)
            {
                var ogPrice = document.QuerySelector("meta[property='og:price:amount']")?.GetAttribute("content") ?? document.QuerySelector("meta[property='product:price:amount']")?.GetAttribute("content");
                if (!string.IsNullOrEmpty(ogPrice))
                {
                    product.Price = CleanPrice(ogPrice);
                    logs.Add("Price found in Open Graph");
                }
            }

            if (string.IsNullOrEmpty(product.Currency))
            {
                product.Currency = document.QuerySelector("meta[property='og:price:currency']")?.GetAttribute("content") ?? document.QuerySelector("meta[property='product:price:currency']")?.GetAttribute("content");
            }
        }

        private void ExtractFromDom(IDocument document, ProductData product, List<string> logs)
        {
            // Title Fallbacks
            if (string.IsNullOrEmpty(product.Title))
            {
                var titleSelectors = new[] { "h1", ".product-title", "#productTitle", ".js-product-name" };
                foreach (var s in titleSelectors)
                {
                    var el = document.QuerySelector(s);
                    if (el != null && !string.IsNullOrWhiteSpace(el.TextContent))
                    {
                        product.Title = el.TextContent.Trim();
                        logs.Add($"Title found via selector: {s}");
                        break;
                    }
                }
            }

            // Fallback to <title> tag (useful for SPA sites)
            if (string.IsNullOrEmpty(product.Title))
            {
                var titleTag = document.QuerySelector("title")?.TextContent?.Trim();
                if (!string.IsNullOrEmpty(titleTag))
                {
                    // Clean up common suffixes like " | SiteName" or " - SiteName"
                    var cleaned = Regex.Replace(titleTag, @"\s*[\|\-–—]\s*[^|\-–—]+$", "").Trim();
                    if (!string.IsNullOrEmpty(cleaned))
                    {
                        product.Title = cleaned;
                        logs.Add("Title found via <title> tag");
                    }
                }
            }

            // Price Fallbacks
            if (!product.Price.HasValue)
            {
                var priceSelectors = new[] { ".products-price", ".js-products-price", ".priceToPay", ".a-offscreen", ".product-price", ".js-product-price", ".price-current", ".price-wrap" };
                foreach (var s in priceSelectors)
                {
                    var el = document.QuerySelector(s);
                    if (el != null && !string.IsNullOrWhiteSpace(el.TextContent)) 
                    { 
                        var text = el.TextContent.Trim();
                        product.Price = CleanPrice(text);
                        if (product.Price.HasValue)
                        {
                            if (text.Contains("₾") || text.Contains("GEL")) product.Currency = "GEL";
                            else if (text.Contains("$") || text.Contains("USD")) product.Currency = "USD";
                            else if (text.Contains("€") || text.Contains("EUR")) product.Currency = "EUR";
                            logs.Add($"Price found via selector: {s}");
                            break; 
                        }
                    }
                }
            }

            // Image Fallbacks
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                var imgSelectors = new[] { "img#landingImage", "img#main-image", "img.product-image", "img.js-product-image" };
                foreach (var s in imgSelectors)
                {
                    var el = document.QuerySelector(s) as IHtmlImageElement;
                    if (el != null && !string.IsNullOrEmpty(el.Source)) 
                    { 
                        product.ImageUrl = el.Source; 
                        logs.Add($"Image found via selector: {s}");
                        break; 
                    }
                }
                
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imgs = document.QuerySelectorAll("img");
                    foreach (var img in imgs.OfType<IHtmlImageElement>())
                    {
                        var src = img.Source?.ToLower();
                        if (src != null && (src.Contains("product") || src.Contains("item") || src.Contains("detail")))
                        {
                            product.ImageUrl = img.Source;
                            logs.Add("Image found via keyword matching");
                            break;
                        }
                    }
                }
            }
        }

        private void NormalizeProduct(ProductData product, string baseUrl)
        {
            if (!string.IsNullOrEmpty(product.Title)) product.Title = System.Net.WebUtility.HtmlDecode(product.Title);
            
            if (!string.IsNullOrEmpty(product.ImageUrl) && !product.ImageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var baseUri = new Uri(baseUrl);
                    var absoluteUri = new Uri(baseUri, product.ImageUrl);
                    product.ImageUrl = absoluteUri.ToString();
                }
                catch { }
            }

            if (product.Price.HasValue && string.IsNullOrEmpty(product.Currency)) product.Currency = "USD";
        }

        private decimal? CleanPrice(string priceStr)
        {
            if (string.IsNullOrWhiteSpace(priceStr)) return null;
            // Remove everything except digits, dots and commas
            var cleaned = Regex.Replace(priceStr, @"[^\d.,]", "");
            if (string.IsNullOrEmpty(cleaned)) return null;

            // Handle European/Georgian format (1,234.56 -> 1234.56 or 1.234,56 -> 1234.56)
            if (cleaned.Contains(",") && cleaned.Contains("."))
                cleaned = cleaned.Replace(",", "");
            else if (cleaned.Contains(","))
            {
                var parts = cleaned.Split(',');
                if (parts.Length > 1 && parts[parts.Length - 1].Length <= 2)
                    cleaned = cleaned.Replace(",", ".");
                else
                    cleaned = cleaned.Replace(",", "");
            }

            if (decimal.TryParse(cleaned, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var p))
                return p;
            
            return null;
        }
        #endregion
    }
}
