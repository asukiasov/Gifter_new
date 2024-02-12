using Microsoft.AspNetCore.Mvc;

namespace SixtyThreeBits.Web.Domain
{
    [TypeFilter(typeof(BeforeWebProjectControllerLoaded), Order = 0)]
    public class WebProjectControllerBase<T> : Controller
    {
        #region Properties
        public T Model { get; set; }
        #endregion        
    }
}
