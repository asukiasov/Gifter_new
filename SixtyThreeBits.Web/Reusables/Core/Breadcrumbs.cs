using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public class Breadcrumbs
    {
        #region Properties        
        readonly List<BreadCrumbItem> BreadCrumbItems = new List<BreadCrumbItem>();

        public ReadOnlyCollection<BreadCrumbItem> Items => new ReadOnlyCollection<BreadCrumbItem>(BreadCrumbItems);
        public bool HasItems => ItemsCount > 0;
        public int ItemsCount => BreadCrumbItems?.Count ?? 0;
        #endregion Properties

        #region Constructors
        public Breadcrumbs() { }
        public Breadcrumbs(IEnumerable<BreadCrumbItem> Items)
        {
            this.BreadCrumbItems = Items.ToList();
        }
        #endregion Constructors        

        #region Methods                
        public void AddItem(BreadCrumbItem NewItem)
        {
            if (BreadCrumbItems != null && NewItem != null)
            {
                foreach (var Item in BreadCrumbItems)
                {
                    Item.IsLastItem = false;
                }
                NewItem.IsLastItem = true;
                BreadCrumbItems.Add(NewItem);
            }
        }

        public void DeleteItem(int Index)
        {
            if (BreadCrumbItems?.Count > Index && Index >= 0)
            {
                BreadCrumbItems[Index - 1].IsLastItem = Index == BreadCrumbItems.Count - 1;

                BreadCrumbItems.RemoveAt(Index);
            }
        }

        public void DeleteLastItem()
        {
            if (BreadCrumbItems?.Count > 0)
            {
                BreadCrumbItems.RemoveAt(BreadCrumbItems.Count - 1);
            }
        }

        public void InitBreadcrumbsByPageUrl<T>(List<HierarchyItem<T>> PageHierarchy, string UrlCurrentPage)
        {
            if (PageHierarchy?.Any() == true)
            {
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
                    BreadCrumbItems.Add(new BreadCrumbItem { Title = Page.PageTitle, IsLastItem = true });
                }

                while (Page != null)
                {
                    Page = PageHierarchy.Where(p => p.ID.Equals(Page.ParentID)).FirstOrDefault();
                    if (Page != null)
                    {
                        BreadCrumbItems.Add(new BreadCrumbItem { Title = Page.PageTitle, NavigateUrl = Page.PageHttpPath });
                    }
                }
            }

            BreadCrumbItems.Reverse();
        }

        public void RemoveAt(int Index)
        {
            if (Index < ItemsCount)
            {
                BreadCrumbItems.RemoveAt(Index);
            }
        }

        public void RenameAt(int Index, string Title)
        {
            if (Index < ItemsCount)
            {
                BreadCrumbItems[Index].Title = Title;
            }
        }

        public void RenameLastItem(string ItemCaption)
        {
            if (BreadCrumbItems?.Count > 0)
            {
                BreadCrumbItems[BreadCrumbItems.Count - 1].Title = ItemCaption;
            }
        }

        public void UpdateItem(int Index, BreadCrumbItem NewItem)
        {
            if (BreadCrumbItems != null && NewItem != null && Index < BreadCrumbItems.Count)
            {
                if (Index == BreadCrumbItems.Count - 1)
                {
                    NewItem.IsLastItem = true;
                }
                BreadCrumbItems[Index] = NewItem;
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
