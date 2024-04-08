using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.ViewModels.Website
{
    public class PagerViewModel
    {
        #region Properties
        public IEnumerable<Item> Items { get; private set; }
        public bool HasItems => Items.Any();
        #endregion

        #region Constructors
        public PagerViewModel()
        {
            Items = Enumerable.Empty<Item>();
        }

        public PagerViewModel(IEnumerable<Item> items)
        {
            Items = items ?? Enumerable.Empty<Item>();
        }
        #endregion        

        #region Nested Classes
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
