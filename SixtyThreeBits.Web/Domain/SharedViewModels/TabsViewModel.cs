using SixtyThreeBits.Web.Domain;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class TabsViewModel
    {
        #region Properties
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        #endregion
    }
}