# C# Scraper Overview (ScraperService)

The scraper in `ScraperService.cs` follows a "best-source-first" strategy to ensure high accuracy:

## Extraction Priority

1. **JSON Data (Highest Priority)**: Extracts structured data from `ld+json` (Schema.org) and `dataLayer` (Google Tag Manager). This is typically the most reliable source for prices and product names.

2. **Open Graph / Meta Tags**: Looks for `og:title`, `og:image`, and product meta tags used for social sharing.

3. **DOM Selectors (Fallback)**: Uses CSS selectors (via AngleSharp) to find titles, prices, and images based on common e-commerce patterns.

4. **Title Tag (Final Fallback)**: For SPA sites, extracts from `<title>` tag with site name suffix cleanup.

5. **Normalization**: Cleans currency symbols, handles different decimal formats (European/Georgian), and converts relative image paths to absolute URLs.

## Endpoints

- `/scraper` - Testing UI
- `/scraper/api/scrape` - Programmatic API

---

## Site-Specific Fixes

### jysk.ge
**Issue**: Price not extracted from their custom HTML structure.

**Structure**:
```html
<div class="price-wrap">
    <div class="d-flex">
        <span class="price-current"><span>190,00 ₾</span>/ცალი</span>
    </div>
</div>
```

**Fix**: Added `.price-current` and `.price-wrap` to DOM price selectors. Currency detection already handles `₾` symbol for GEL.

---

### Amazon
**Issue**: URLs without `https://` prefix caused "invalid request URI" error.

**Example**: `amazon.com/Google-Pixel-Watch...` failed because HttpClient requires absolute URIs.

**Fix**: Added URL normalization to automatically prepend `https://` if the URL doesn't start with `http://` or `https://`.

---

### eBay
**Issue**: JSON-LD extraction failed because `@type` can be an array.

**Example JSON-LD**:
```json
{
  "@type": "Product",
  "offers": {
    "@type": "Offer",
    "price": "415.06",
    "priceCurrency": "USD"
  }
}
```

**Fix**: Added `IsProductType()` helper method that handles both:
- String format: `"@type": "Product"`
- Array format: `"@type": ["Product", "ItemPage"]`

---

### Jomashop (SPA Sites)
**Issue**: Client-side rendered (React/SPA) site returns only JavaScript bundles, not rendered HTML.

**Fix**:
1. Added `<title>` tag fallback - extracts title and cleans up site name suffixes like ` | Jomashop`
2. Added SPA state patterns to search for embedded product data:
   - `__NEXT_DATA__` (Next.js)
   - `window.__INITIAL_STATE__`
   - `window.__PRELOADED_STATE__`
   - Inline `"product":` and `"productData":` JSON objects

**Note**: For SPA sites, only title may be available without price/image if all data is loaded client-side.
