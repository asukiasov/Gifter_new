using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Controllers.Base;
using SixtyThreeBits.Web.Filters.Website;

namespace SixtyThreeBits.Web.Controllers.Website.Base
{
    [TypeFilter(typeof(BeforeWebsitePageLoad), Order = 1)]
    public class WebsiteControllerBase<T> : ControllerBase<T>
    {

    }
}
