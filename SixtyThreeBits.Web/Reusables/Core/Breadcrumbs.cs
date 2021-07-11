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
}
