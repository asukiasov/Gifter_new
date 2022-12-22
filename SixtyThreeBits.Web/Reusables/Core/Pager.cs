using SixtyThreeBits.Core.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class Pager
    {
        #region Properties
        int? ItemsCountTotal;
        int? CurrentPageNumber;
        int? ItemsPerPage;
        string CurrentPageHttpPath;
        bool UseQueryStringStyle;

        public List<Item> Items { get; set; }
        public bool HasItems => Items?.Any() == true;
        #endregion

        #region Constructors
        public Pager() { }

        public Pager(string CurrentPageHttpPath, int? ItemsPerPage, int? ItemsCountTotal, int? CurrentPageNumber = 1, bool UseQueryStringStyle = false)
        {
            this.ItemsCountTotal = ItemsCountTotal;
            this.CurrentPageNumber = CurrentPageNumber;
            this.ItemsPerPage = ItemsPerPage;
            this.CurrentPageHttpPath = CurrentPageHttpPath;
            this.UseQueryStringStyle = UseQueryStringStyle;

            this.InitItems();
        }
        #endregion

        #region Methods
        public void InitItems()
        {
            CurrentPageNumber = CurrentPageNumber.HasValue ? CurrentPageNumber : 1;

            if (UseQueryStringStyle)
            {
                CurrentPageHttpPath = CurrentPageHttpPath.Replace($"?page={CurrentPageNumber}", "").Replace($"&page={CurrentPageNumber}", "");
            }
            else
            {
                CurrentPageHttpPath = CurrentPageHttpPath.Replace($"/page-{CurrentPageNumber}", "");
            }

            var Parts = CurrentPageHttpPath.Split('?');
            var RootUrl = Parts[0].TrimEnd('/');
            var QueryString = Parts.Length > 1 ? Parts[1] : "";
            var QueryStringSeparator = "&";
            var IsQueryStringEmpty = string.IsNullOrEmpty(QueryString);

            if (!IsQueryStringEmpty) { QueryString = $"?{QueryString}"; }

            if (IsQueryStringEmpty && UseQueryStringStyle) { QueryStringSeparator = "?"; }


            if (ItemsCountTotal.HasValue && ItemsPerPage.HasValue)
            {
                Items = new List<Item>();

                var PageCount = Convert.ToInt32(Math.Ceiling((decimal)(ItemsCountTotal.Value) / (ItemsPerPage.Value)));
                if (PageCount < 2)
                {
                    return;
                }
                else if (PageCount < 11)
                {
                    Items = Enumerable.Range(1, PageCount).Select(PageNumber => new Item
                    {
                        Text = PageNumber.ToString(),
                        Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber),
                        IsActive = CurrentPageNumber == PageNumber
                    }).ToList();
                }
                else
                {
                    const int PagesOffset = 3;

                    for (var i = 1; (i <= PagesOffset && CurrentPageNumber - i > 0); i++)
                    {
                        var PageNumber = CurrentPageNumber - i;
                        Items.Insert(0, new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber - PagesOffset > 1)
                    {
                        Items.Insert(0, new Item { Text = "..." });
                        Items.Insert(0, new Item { Text = "1", Url = CurrentPageHttpPath });
                    }

                    Items.Add(new Item { Text = CurrentPageNumber.ToString(), IsActive = true });

                    for (var i = 1; (i <= PagesOffset && CurrentPageNumber + i <= PageCount); i++)
                    {
                        var PageNumber = CurrentPageNumber + i;
                        Items.Add(new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber + PagesOffset < PageCount)
                    {
                        Items.Add(new Item { Text = "..." });
                        Items.Add(new Item { Text = PageCount.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageCount) });
                    }
                }


                if (CurrentPageNumber > 1)
                {
                    Items.Insert(0, new Item { Text = $"&lt; {Resources.TextPrev} ", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber - 1) });
                }

                if (CurrentPageNumber < PageCount)
                {
                    Items.Add(new Item { Text = $"{Resources.TextNext} &gt;", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber + 1) });
                }                
            }            
        }

        string GetPagerItemUrl(string CurrentPageHttpPath, string RootUrl, bool UseQueryStringStyle, string QueryString, string QueryStringSeparator, int? PageNumber)
        {
            return PageNumber == 1 ?
            CurrentPageHttpPath :
            $"{RootUrl}{(UseQueryStringStyle ? $"{QueryString}{QueryStringSeparator}page={PageNumber}" : $"/page-{PageNumber}/{QueryString}")}";
        }
        #endregion

        #region Sub Classes
        public class Item
        {
            #region Properties
            public string Text { get; set; }
            public string Url { get; set; }
            public bool HasUrl => !string.IsNullOrWhiteSpace(Url);
            public bool IsActive { get; set; }
            #endregion Properties
        }
        #endregion
    }
}
