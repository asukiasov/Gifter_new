using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class TabsViewModel
    {
        #region Properties
        public List<ProjectMenuItem> Tabs { get; set; }
        public bool HasTabs => Tabs?.Count > 0;
        #endregion
    }
}