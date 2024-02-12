using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Filters;

namespace SixtyThreeBits.Web.Controllers
{
    [TypeFilter(typeof(BeforeWebsitePageLoad), Order = 1)]
    public class WebsiteControllerBase<T> : WebProjectControllerBase<T>
    { 
        
    }
}
