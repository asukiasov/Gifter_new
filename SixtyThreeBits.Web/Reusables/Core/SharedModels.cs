using SixtyThreeBits.Core.Abstractions;
using SixtyThreeBits.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class Breadcrumbs
    {
        #region Properties
        public int ItemsCount => Items?.Count ?? 0;
        public List<BreadCrumbItem> Items { get; set; } = new List<BreadCrumbItem>();
        public bool HasItems => ItemsCount > 0;
        #endregion Properties

        #region Constructors
        public Breadcrumbs() { }
        public Breadcrumbs(IEnumerable<BreadCrumbItem> Items)
        {
            this.Items = Items.ToList();
        }
        #endregion Constructors        

        #region Methods                
        public void AddItem(BreadCrumbItem NewItem)
        {
            if (Items != null && NewItem != null)
            {
                foreach (var Item in Items)
                {
                    Item.IsLastItem = false;
                }
                NewItem.IsLastItem = true;
                Items.Add(NewItem);
            }
        }

        public void DeleteItem(int Index)
        {
            if (Items?.Count > Index && Index >= 0)
            {
                Items[Index - 1].IsLastItem = Index == Items.Count - 1;

                Items.RemoveAt(Index);
            }
        }

        public void DeleteLastItem()
        {
            if (Items?.Count > 0)
            {
                Items.RemoveAt(Items.Count - 1);
            }
        }

        public static Breadcrumbs GetBreadcrumbsByPageUrl<T>(List<HierarchyItem<T>> PageHierarchy, string UrlCurrentPage)
        {
            var Items = new List<BreadCrumbItem>();

            var Page = default(HierarchyItem<T>);
            foreach (var Item in PageHierarchy)
            {
                var UrlToCompare = Item.PageHttpPath?.ToLower();
                if (UrlToCompare == UrlCurrentPage || (!string.IsNullOrWhiteSpace(Item.PageHttpPath) && Regex.IsMatch(UrlCurrentPage, $"{UrlToCompare}+$")))
                {
                    Page = Item;
                }
            }

            if (Page != null)
            {
                Items.Add(new BreadCrumbItem { Title = Page.PageTitle, IsLastItem = true });
            }

            while (Page != null)
            {
                Page = PageHierarchy.Where(p => p.ID.Equals(Page.ParentID)).FirstOrDefault();
                if (Page != null)
                {
                    Items.Add(new BreadCrumbItem { Title = Page.PageTitle, NavigateUrl = Page.PageHttpPath });
                }
            }

            Items.Reverse();
            return new Breadcrumbs(Items);
        }

        public void RenameLastItem(string ItemCaption)
        {
            if (Items?.Count > 0)
            {
                Items[Items.Count - 1].Title = ItemCaption;
            }
        }

        public void UpdateItem(BreadCrumbItem NewItem, int Index)
        {
            if (Items != null && NewItem != null && Index < Items.Count)
            {
                if (Index == Items.Count - 1)
                {
                    NewItem.IsLastItem = true;
                }
                Items[Index] = NewItem;
            }
        }
        #endregion Methods

        #region Sub Classes
        public class BreadCrumbItem
        {
            #region Properties
            public string Title { get; set; }
            public bool HasNavigateUrl => !string.IsNullOrWhiteSpace(NavigateUrl);
            public string NavigateUrl { get; set; }
            public bool IsLastItem { get; set; }
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">ID and ParentID type</typeparam>
        public class HierarchyItem<T>
        {
            #region Properties
            public T ID { get; set; }
            public T ParentID { get; set; }
            public string PageHttpPath { get; set; }
            public string PageTitle { get; set; }
            #endregion
        }
        #endregion
    }

    public class DevExtremeGridFilterItem
    {
        #region Properties
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        #endregion
    }

    public class DevExtremeGridSortItem
    {
        #region Properties
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }
        #endregion
    }

    public class Pager
    {
        #region Properties
        int? ItemsCountTotal;
        int? CurrentPageNumber;
        int? ItemsPerPage;
        string CurrentPageHttpPath;
        bool UseQueryStringStyle;
        #endregion

        #region Constructors
        public Pager(string CurrentPageHttpPath, int? ItemsPerPage, int? ItemsCountTotal, int? CurrentPageNumber = 1, bool UseQueryStringStyle = false)
        {
            this.ItemsCountTotal = ItemsCountTotal;
            this.CurrentPageNumber = CurrentPageNumber;
            this.ItemsPerPage = ItemsPerPage;
            this.CurrentPageHttpPath = CurrentPageHttpPath;
            this.UseQueryStringStyle = UseQueryStringStyle;
        }
        #endregion

        #region Methods
        public List<Item> GetPager()
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
                List<Item> Pager = new List<Item>();

                var PageCount = Convert.ToInt32(Math.Ceiling((decimal)(ItemsCountTotal.Value) / (ItemsPerPage.Value)));
                if (PageCount < 2)
                {
                    return null;
                }
                else if (PageCount < 11)
                {
                    Pager = Enumerable.Range(1, PageCount).Select(PageNumber => new Item
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
                        Pager.Insert(0, new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber - PagesOffset > 1)
                    {
                        Pager.Insert(0, new Item { Text = "..." });
                        Pager.Insert(0, new Item { Text = "1", Url = CurrentPageHttpPath });
                    }

                    Pager.Add(new Item { Text = CurrentPageNumber.ToString(), IsActive = true });

                    for (var i = 1; (i <= PagesOffset && CurrentPageNumber + i <= PageCount); i++)
                    {
                        var PageNumber = CurrentPageNumber + i;
                        Pager.Add(new Item { Text = PageNumber.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageNumber) });
                    }

                    if (CurrentPageNumber + PagesOffset < PageCount)
                    {
                        Pager.Add(new Item { Text = "..." });
                        Pager.Add(new Item { Text = PageCount.ToString(), Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, PageCount) });
                    }
                }


                if (CurrentPageNumber > 1)
                {
                    Pager.Insert(0, new Item { Text = "&lt; Prev ", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber - 1) });
                }

                if (CurrentPageNumber < PageCount)
                {
                    Pager.Add(new Item { Text = "Next &gt;", Url = GetPagerItemUrl(CurrentPageHttpPath, RootUrl, UseQueryStringStyle, QueryString, QueryStringSeparator, CurrentPageNumber + 1) });
                }

                return Pager;
            }
            else
            {
                return null;
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

    public class PageTitle : IPageTitle 
    {
        #region Properties        
        string _TitleHead = Constants.ProjectName;
        string _TitleValue = Constants.ProjectName;

        public string TitleHead => _TitleHead;
        public string Value => _TitleValue;
        #endregion

        #region Methods
        public void Set(string PageTitle)
        {
            if (!string.IsNullOrWhiteSpace(PageTitle))
            {
                _TitleHead = $"{PageTitle} | {Constants.ProjectName}";
                _TitleValue = PageTitle;
            }
        }

        public override string ToString()
        {
            return _TitleValue;
        } 
        #endregion
    }

    public class PluginsClient
    {
        #region Properties
        bool _Is63BitsFormsEnabled;
        bool _Is63BitsComponentsEnabled;
        bool _Is63BitsFontsEnabled;
        bool _IsAngleEnabled;
        bool _IsBootstrapEnabled;        
        bool _IsDevextremeEnabled;
        bool _IsGoogleFontsEnabled;
        bool _IsFancyboxEnabled;
        bool _IsFontAwesomeEnabled;
        bool _IsJQueryEnabled;
        bool _IsJQueryUIJsEnabled;
        bool _IsJQueryUICssEnabled;
        bool _IsJQueryConfirmEnabled;
        bool _IsJQueryNestedSortableEnabled;
        bool _IsJsClientEnabled;
        bool _IsJsZipEnabled;
        bool _IsPageBuilderEnabled;
        bool _IsPageBuilderEditorEnabled;
        bool _IsPreloaderEnabled;
        bool _IsSelect2Enabled;
        bool _IsSlickSliderEnabled;
        bool _IsSuccessErrorMessageEnabled;
        bool _IsTemplate7Enabled;
        bool _IsTinyMceEnabled;
        bool _IsUtilsEnabled;

        public bool Is63BitsFormsEnabled => _Is63BitsFormsEnabled;
        public bool Is63BitsComponentsEnabled => _Is63BitsComponentsEnabled;
        public bool Is63BitsFontsEnabled => _Is63BitsFontsEnabled;
        public bool IsAngleEnabled => _IsAngleEnabled;
        public bool IsBootstrapEnabled => _IsBootstrapEnabled;        
        public bool IsDevextremeEnabled => _IsDevextremeEnabled;
        public bool IsGoogleFontsEnabled => _IsGoogleFontsEnabled;
        public bool IsFancyboxEnabled => _IsFancyboxEnabled;
        public bool IsFontAwesomeEnabled => _IsFontAwesomeEnabled;
        public bool IsJQueryEnabled => _IsJQueryEnabled;
        public bool IsJQueryConfirmEnabled => _IsJQueryConfirmEnabled;
        public bool IsJQueryNestedSortableEnabled => _IsJQueryNestedSortableEnabled;
        public bool IsJQueryUICssEnabled => _IsJQueryUICssEnabled;
        public bool IsJQueryUIJsEnabled => _IsJQueryUIJsEnabled;
        public bool IsJsClientEnabled => _IsJsClientEnabled;
        public bool IsJsZipEnabled => _IsJsZipEnabled;
        public bool IsPageBuilderEnabled => _IsPageBuilderEnabled;
        public bool IsPageBuilderEditorEnabled => _IsPageBuilderEditorEnabled;
        public bool IsPreloaderEnabled => _IsPreloaderEnabled;        
        public bool IsSelect2Enabled => _IsSelect2Enabled;
        public bool IsSlickSliderEnabled => _IsSelect2Enabled;        
        public bool IsSuccessErrorMessageEnabled => _IsSuccessErrorMessageEnabled;
        public bool IsTemplate7Enabled => _IsTemplate7Enabled;
        public bool IsTinyMceEnabled => _IsTinyMceEnabled;        
        public bool IsUtilsEnabled => _IsUtilsEnabled;
        #endregion

        #region Methods
        public PluginsClient Enable63BitsForms(bool Value)
        {
            _Is63BitsFormsEnabled = Value;
            return this;
        }

        public PluginsClient Enable63BitsComponents(bool Value)
        {
            _Is63BitsComponentsEnabled = Value;
            return this;
        }

        public PluginsClient Enable63BitsFonts(bool Value)
        {
            _Is63BitsFontsEnabled = Value;
            return this;
        }

        public PluginsClient EnableAngle(bool Value)
        {
            _IsAngleEnabled = Value;
            return this;
        }

        public PluginsClient EnableBootstrap(bool Value)
        {
            _IsBootstrapEnabled = Value;
            return this;
        }        

        public PluginsClient EnableDevextreme(bool Value)
        {
            _IsDevextremeEnabled = Value;
            return this;
        }

        public PluginsClient EnableFancybox(bool Value)
        {
            _IsFancyboxEnabled = Value;
            return this;
        }

        public PluginsClient EnableGoogleFonts(bool Value)
        {
            _IsGoogleFontsEnabled = Value;
            return this;
        }

        public PluginsClient EnableFontAwesome(bool Value)
        {
            _IsFontAwesomeEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQuery(bool Value)
        {
            _IsJQueryEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryUI(bool EnableJs, bool EnableCss)
        {
            _IsJQueryUIJsEnabled = EnableJs;
            _IsJQueryUICssEnabled = EnableCss;
            return this;
        }

        public PluginsClient EnableJQueryConfirm(bool Value)
        {
            _IsJQueryConfirmEnabled = Value;
            return this;
        }

        public PluginsClient EnableJQueryNestedSortable(bool Value)
        {
            _IsJQueryNestedSortableEnabled = Value;
            return this;
        }

        public PluginsClient EnableJsClient(bool Value)
        {
            _IsJsClientEnabled = Value;
            return this;
        }

        public PluginsClient EnableJsZip(bool Value)
        {
            _IsJsZipEnabled = Value;
            return this;
        }

        public PluginsClient EnablePageBuilder(bool Value)
        {
            _IsPageBuilderEnabled = Value;
            return this;
        }

        public PluginsClient EnablePageBuilderEditor(bool Value)
        {
            _IsPageBuilderEditorEnabled = Value;
            return this;
        }

        public PluginsClient EnablePreloader(bool Value)
        {
            _IsPreloaderEnabled = Value;
            return this;
        }

        public PluginsClient EnableSelect2(bool Value)
        {
            _IsSelect2Enabled = Value;
            return this;
        }

        public PluginsClient EnableSlickSlider(bool Value)
        {
            _IsSlickSliderEnabled = Value;
            return this;
        }

        public PluginsClient EnableSuccessErrorMessage(bool Value)
        {
            _IsSuccessErrorMessageEnabled = Value;
            return this;
        }

        public PluginsClient EnableTemplate7(bool Value)
        {
            _IsTemplate7Enabled = Value;
            return this;
        }

        public PluginsClient EnableTinyMce(bool Value)
        {
            _IsTinyMceEnabled = Value;
            return this;
        }

        public PluginsClient EnableUtils(bool Value)
        {
            _IsUtilsEnabled = Value;
            return this;
        }
        #endregion
    }

    public class ProjectMenuItem
    {
        #region Properties
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string NavigateUrl { get; set; }
        public bool IsHashTag => NavigateUrl == "#";
        public bool IsSelected { get; set; }
        public bool IsHomePage { get; set; }
        public bool IsTargetBlank { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<ProjectMenuItem> Children { get; set; } 
        #endregion
    }

    public class TreeNodeItem
    {
        #region Properties
        public string NodeID { get; set; }
        public string ParentID { get; set; }
        public string Filename { get; set; }
        public string NavigateUrl { get; set; }
        public bool HasNavigateUrl => !string.IsNullOrEmpty(NavigateUrl);
        public bool IsTargetBlank { get; set; }
        public string Caption { get; set; }
        public string CaptionEng { get; set; }
        public bool IsToggler1Checked { get; set; }
        public bool IsToggler2Checked { get; set; }
        public string LanguageCode { get; set; }
        public bool IsFolder { get; set; }
        public bool HasChildren => Children?.Count > 0;
        public List<TreeNodeItem> Children { get; set; }

        public bool IsDisabled { get; set; }
        public bool IsNoNestingEnabled { get; set; }

        public bool ShowAddNewButton { get; set; }
        public bool ShowEditButton { get; set; }
        public bool ShowDeleteButton { get; set; }
        public bool ShowToggler1 { get; set; }
        public bool ShowToggler2 { get; set; }
        public bool ShowCustomButton { get; set; }
        public bool ShowCustomButtonFirst { get; set; }
        public bool ShowCustomButtonLast => !ShowCustomButtonFirst;

        public string CustomButtonIcon { get; set; }
        public string TextToggler1 { get; set; } = "Published?";
        public string TextToggler2 { get; set; } = "Menu?";
        #endregion        
    }

    public class SyncSortIndexesModel
    {
        #region Properties
        public List<SyncSortIndexesItem> SortIndexes { get; set; }
        #endregion
    }

    public class SuccessErrorPartialViewModel
    {
        #region Properties
        public bool IsTop { set; get; }
        public string Message { set; get; }
        public bool ShowError { set; get; }
        public bool ShowSuccess { set; get; }
        public bool IsInitialized { get; set; }
        #endregion
    }
}