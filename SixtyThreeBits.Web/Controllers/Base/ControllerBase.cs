using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Filters.Base;

namespace SixtyThreeBits.Web.Controllers.Base
{
    [TypeFilter(typeof(BaseFilterAttribute), Order = 0)]
    public class ControllerBase<T> : Controller where T : new()
    {
        #region Properties
        public T Model { get; set; }
        #endregion        

        #region Constructors
        public ControllerBase()
        {
            Model = new T();
        }
        #endregion
    }
}