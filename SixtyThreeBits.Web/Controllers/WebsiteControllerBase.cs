using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Filters;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Controllers
{    
    [TypeFilter(typeof(BeforeWebsitePageLoad), Order = 1)]
    public class WebsiteControllerBase<T> : WebProjectController<T>
    {        
    }
}
