using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Filters.Shared;

namespace SixtyThreeBits.Web.Controllers.Base
{
    [TypeFilter(typeof(SharedFilterAttribute), Order = 0)]
    public class ControllerBase<T> : Controller
    {
        #region Properties
        public T Model { get; set; }
        #endregion        
    }
}