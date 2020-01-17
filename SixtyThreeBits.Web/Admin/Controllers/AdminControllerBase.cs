using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [BeforeAdminPageLoad(Order = 1)]
    public class AdminControllerBase<T> : WebProjectController<T>
    {

    }
}
