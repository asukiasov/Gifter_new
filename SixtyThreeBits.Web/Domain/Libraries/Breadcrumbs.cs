using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class Breadcrumbs
    {
        #region Properties        
        readonly List<BreadCrumbItem> _breadCrumbItems = new List<BreadCrumbItem>();
        public ReadOnlyCollection<BreadCrumbItem> Items => new ReadOnlyCollection<BreadCrumbItem>(_breadCrumbItems);
        public bool HasItems => ItemsCount > 0;
        public int ItemsCount => _breadCrumbItems.Count;
        #endregion Properties

        #region Constructors
        public Breadcrumbs() { }        
        #endregion Constructors        

        #region Methods                
        public void AddItem(BreadCrumbItem newItem)
        {
            if (_breadCrumbItems != null && newItem != null)
            {
                foreach (var item in _breadCrumbItems)
                {
                    item.IsLastItem = false;
                }
                newItem.IsLastItem = true;
                _breadCrumbItems.Add(newItem);
            }
        }

        public void DeleteItem(int index)
        {
            if (_breadCrumbItems?.Count > index && index >= 0)
            {
                _breadCrumbItems[index - 1].IsLastItem = index == _breadCrumbItems.Count - 1;

                _breadCrumbItems.RemoveAt(index);
            }
        }

        public void DeleteLastItem()
        {
            if (_breadCrumbItems?.Count > 0)
            {
                _breadCrumbItems.RemoveAt(_breadCrumbItems.Count - 1);
            }
        }

        public void InitBreadcrumbsByPageUrl<T>(List<HierarchyItem<T>> pageHierarchy, string urlCurrentPage)
        {
            if (pageHierarchy?.Any() == true)
            {
                var page = default(HierarchyItem<T>);
                foreach (var item in pageHierarchy)
                {
                    var urlToCompare = item.PageHttpPath?.ToLower();
                    if (urlToCompare == urlCurrentPage || !string.IsNullOrWhiteSpace(item.PageHttpPath) && Regex.IsMatch(urlCurrentPage, $"{urlToCompare}+$"))
                    {
                        page = item;
                    }
                }

                if (page != null)
                {
                    _breadCrumbItems.Add(new BreadCrumbItem { Title = page.PageTitle, IsLastItem = true });
                }

                while (page != null)
                {
                    page = pageHierarchy.Where(p => p.ID.Equals(page.ParentID)).FirstOrDefault();
                    if (page != null)
                    {
                        _breadCrumbItems.Add(new BreadCrumbItem { Title = page.PageTitle, NavigateUrl = page.PageHttpPath });
                    }
                }
            }

            _breadCrumbItems.Reverse();
        }

        public void RemoveAt(int index)
        {
            if (index < ItemsCount)
            {
                _breadCrumbItems.RemoveAt(index);
            }
        }

        public void RenameAt(int index, string title)
        {
            if (index < ItemsCount)
            {
                _breadCrumbItems[index].Title = title;
            }
        }

        public void RenameLastItem(string itemCaption)
        {
            if (_breadCrumbItems?.Count > 0)
            {
                _breadCrumbItems[_breadCrumbItems.Count - 1].Title = itemCaption;
            }
        }

        public void UpdateItem(int index, BreadCrumbItem newItem)
        {
            if (_breadCrumbItems != null && newItem != null && index < _breadCrumbItems.Count)
            {
                if (index == _breadCrumbItems.Count - 1)
                {
                    newItem.IsLastItem = true;
                }
                _breadCrumbItems[index] = newItem;
            }
        }
        #endregion Methods

        #region Nested Classes
        public class BreadCrumbItem
        {
            #region Properties
            public string Title { get; set; }
            public bool HasNavigateUrl => !string.IsNullOrWhiteSpace(NavigateUrl);
            public string NavigateUrl { get; set; }
            public bool IsLastItem { get; set; }
            #endregion
        }

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
