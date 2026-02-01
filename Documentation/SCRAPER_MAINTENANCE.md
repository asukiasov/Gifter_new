# Scraper Maintenance Guide

This guide explains how to maintain and extend the product metadata scraper in `ScraperService.cs`.

## Overview
The scraper uses a **Multi-Strategy Extraction** pattern to handle different types of websites (SSR, CSR, and SPA). 

### Extraction Strategies (In Order)
1. **JSON-LD (Structured Data)**: Extracts schema.org information from `<script type="application/ld+json">`. This is the most reliable method.
2. **Next.js Data (`__NEXT_DATA__`)**: Specifically for React/Next.js sites (like Zoommer). It parses the hydration state to find prices and titles.
3. **Meta Tags**: Checks for standard Open Graph (`og:price:amount`), Twitter Cards, and Itemprop tags.
4. **Regular Expressions**: A fallback for Georgian Lari patterns (e.g., `₾`, `Lari`) found in the HTML body.
5. **CSS Selectors**: Generic fallback for common class names like `.price`, `.product-title`.

## Adding a New Store Support
If a new store (e.g., `alta.ge`) is not returning prices:
1. **Check the Source**: View the page source to see if the price is in a JSON block.
2. **Update `SearchForPriceInJson`**: Add any new property names (e.g., `"current_price"`) to the recursive search list.
3. **Handle Currencies**: Ensure the currency is detected or default to `GEL`.

## Anti-Bot Measures
Many Georgian retailers use Cloudflare or custom anti-bot protection.
- **Headers**: The scraper sends a realistic `User-Agent` and `Accept` header to mimic a Chrome browser.
- **Timeout**: Set to **25 seconds** to allow for slow responses or anti-bot verification delays.
- **Cookies**: Currently, the scraper is stateless. If a site requires session cookies, the `HttpClient` logic will need to be updated to a `CookieContainer`.

## Troubleshooting
- **Returns 403/401**: The site has updated its bot detection. Update the `User-Agent` in `ScraperService.cs`.
- **Zero Price**: The price is likely loaded via JavaScript. Check if it's available in `window.__INITIAL_STATE__` or similar script tags and add a new extraction case.
