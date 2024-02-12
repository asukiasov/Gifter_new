using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class Pager
    {
        #region Properties
        int? _itemsCountTotal;
        int? _currentPageNumber;
        int? _itemsPerPage;
        string _currentPageHttpPath;
        bool _useQueryStringStyle;        
        #endregion

        #region Constructors
        public Pager() { }

        public Pager(string currentPageHttpPath, int? itemsPerPage, int? itemsCountTotal, int? currentPageNumber = 1, bool useQueryStringStyle = false)
        {
            _itemsCountTotal = itemsCountTotal;
            _currentPageNumber = currentPageNumber;
            _itemsPerPage = itemsPerPage;
            _currentPageHttpPath = currentPageHttpPath;
            _useQueryStringStyle = useQueryStringStyle;
        }
        #endregion

        #region Methods
        public PagerViewModel GetPagerViewModel()
        {
            var items = new List<PagerViewModel.Item>();

            _currentPageNumber = _currentPageNumber.HasValue ? _currentPageNumber : 1;

            if (_useQueryStringStyle)
            {
                _currentPageHttpPath = _currentPageHttpPath.Replace($"?page={_currentPageNumber}", "").Replace($"&page={_currentPageNumber}", "");
            }
            else
            {
                _currentPageHttpPath = _currentPageHttpPath.Replace($"/page-{_currentPageNumber}", "");
            }

            var parts = _currentPageHttpPath.Split('?');
            var rootUrl = parts[0].TrimEnd('/');
            var queryString = parts.Length > 1 ? parts[1] : "";
            var queryStringSeparator = "&";
            var isQueryStringEmpty = string.IsNullOrEmpty(queryString);

            if (!isQueryStringEmpty) { queryString = $"?{queryString}"; }

            if (isQueryStringEmpty && _useQueryStringStyle) { queryStringSeparator = "?"; }
            
            if (_itemsCountTotal.HasValue && _itemsPerPage.HasValue)
            {
                var pageCount = Convert.ToInt32(Math.Ceiling((decimal)_itemsCountTotal.Value / _itemsPerPage.Value));
                if (pageCount > 1)
                {
                    if (pageCount < 11)
                    {
                        items = Enumerable.Range(1, pageCount).Select(PageNumber => new PagerViewModel.Item
                        {
                            Text = PageNumber.ToString(),
                            Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, PageNumber),
                            IsActive = _currentPageNumber == PageNumber
                        }).ToList();
                    }
                    else
                    {
                        const int pagesOffset = 3;

                        for (var i = 1; i <= pagesOffset && _currentPageNumber - i > 0; i++)
                        {
                            var PageNumber = _currentPageNumber - i;
                            items.Insert(0, new PagerViewModel.Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, PageNumber) });
                        }

                        if (_currentPageNumber - pagesOffset > 1)
                        {
                            items.Insert(0, new PagerViewModel.Item { Text = "..." });
                            items.Insert(0, new PagerViewModel.Item { Text = "1", Url = _currentPageHttpPath });
                        }

                        items.Add(new PagerViewModel.Item { Text = _currentPageNumber.ToString(), IsActive = true });

                        for (var i = 1; i <= pagesOffset && _currentPageNumber + i <= pageCount; i++)
                        {
                            var PageNumber = _currentPageNumber + i;
                            items.Add(new PagerViewModel.Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, PageNumber) });
                        }

                        if (_currentPageNumber + pagesOffset < pageCount)
                        {
                            items.Add(new PagerViewModel.Item { Text = "..." });
                            items.Add(new PagerViewModel.Item { Text = pageCount.ToString(), Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, pageCount) });
                        }
                    }


                    if (_currentPageNumber > 1)
                    {
                        items.Insert(0, new PagerViewModel.Item { Text = $"&lt; {Resources.TextPrev} ", Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, _currentPageNumber - 1) });
                    }

                    if (_currentPageNumber < pageCount)
                    {
                        items.Add(new PagerViewModel.Item { Text = $"{Resources.TextNext} &gt;", Url = GetPagerItemUrl(_currentPageHttpPath, rootUrl, _useQueryStringStyle, queryString, queryStringSeparator, _currentPageNumber + 1) });
                    }
                }
            }

            return new PagerViewModel(items);
        }

        string GetPagerItemUrl(string currentPageHttpPath, string rootUrl, bool useQueryStringStyle, string queryString, string queryStringSeparator, int? pageNumber)
        {
            return pageNumber == 1 ?
            currentPageHttpPath :
            $"{rootUrl}{(useQueryStringStyle ? $"{queryString}{queryStringSeparator}page={pageNumber}" : $"/page-{pageNumber}/{queryString}")}";
        }
        #endregion

        
    }
}
