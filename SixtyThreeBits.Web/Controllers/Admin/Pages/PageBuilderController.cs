using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Controllers.Admin.Base;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Filters.Admin;
using SixtyThreeBits.Web.Models.Admin;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Controllers.Admin
{
    [Route("admin/pages/{pageID:int}/page-builder")]
    [TypeFilter(typeof(PageFilterAttribute), Order = 2)]
    public class PageBuilderController : AdminControllerBase<PageBuilderModel>
    {
        #region Actions
        [Route("", Name = ControllerActionRouteNames.Admin.PageBuilderController.Builder)]
        [Route("{Language:length(2)}", Name = ControllerActionRouteNames.Admin.PageBuilderController.BuilderLanguage)]
        public IActionResult PageBuilder(int? pageID, string language)
        {
            var viewModel = Model.GetViewModel(pageID, language);
            viewModel.PluginsClient.EnableJsClient(true).EnableJQuery(true).EnableBootstrap(true).EnableFancybox(true).EnablePreloader(true).EnableTemplate7(true).EnableTinyMce(true).EnableUtils(true).EnablePageBuilderEditor(true).EnableJWPlayer(true).EnableFontAwesome(true).EnableMalihuScroll(true).EnableSortableJS(true);
            return View(ViewNames.Admin.Pages.Page.PageBuilderView, viewModel);
        }

        [HttpPost]
        [Route("")]
        [Route("{Language:length(2)}")]
        public async Task<IActionResult> PageBuilder(PageBuilderModel.SubmitModel submitModel)
        {
            var viewModel = new AjaxResponse();
            var errors = Model.Validate(submitModel);
            if (errors.HasErrors)
            {
                viewModel.Data = errors;
            }
            else
            {
                viewModel = await Model.Save(submitModel);
            }
            return Json(viewModel);
        }
        #endregion
    }
}