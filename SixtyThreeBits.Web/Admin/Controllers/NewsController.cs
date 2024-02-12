using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Infrastructure.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Domain;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/news")]
    public class NewsController : AdminControllerBase<NewsModel>
    {
        #region Constructors
        public NewsController()
        {
            Model = new NewsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.News.Page)]
        public ActionResult News()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.News.Page, viewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.News.Grid)]
        public async Task<ActionResult> NewsGrid()
        {
            var viewModel = await Model.GetGridViewModel();
            return Json(viewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.News.GridAdd)]
        public async Task<ActionResult> NewsGridAdd(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<NewsModel.PageViewModel.GridModel.GridItem>() ?? new NewsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.CREATE, newsID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.News.GridUpdate)]
        public async Task<ActionResult> NewsGridUpdate(int? key, string values)
        {
            var submitModel = values.DeserializeJsonTo<NewsModel.PageViewModel.GridModel.GridItem>() ?? new NewsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(databaseAction: Enums.DatabaseActions.UPDATE, newsID: key, submitModel: submitModel);
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.News.GridDelete)]
        public async Task<ActionResult> NewsGridDelete(int? key)
        {
            await Model.CRUD(databaseAction: Enums.DatabaseActions.DELETE, newsID: key, submitModel: new NewsModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        } 
        #endregion

    }

    [Route("admin/news/{newsID:int}")]
    [TypeFilter(typeof(BeforeNewsPageLoad), Order = 2)]
    public class NewsPropertiesController : AdminControllerBase<NewsPropertiesModel>
    {
        #region Constructors
        public NewsPropertiesController()
        {
            Model = new NewsPropertiesModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.News.NewsItem)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel(viewModel: null);
            return View(ViewNames.Admin.News.NewsProperties, viewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(NewsPropertiesModel.PageViewModel submitModel)
        {
            var result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true).EnableDevextreme(true);
            var viewModel = Model.GetPageViewModel(viewModel: submitModel);

            await Model.ValidatePageViewModel(viewModel);
            if (viewModel.IsValid)
            {
                await Model.Save(viewModel);
                if (viewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItem, new { NewsID = Model.DBItem.NewsID }));
                }
                else
                {
                    Model.ShowError();
                    result = View(ViewNames.Admin.News.NewsProperties, viewModel);
                }
            }
            else
            {
                result = View(ViewNames.Admin.News.NewsProperties, viewModel);
            }
            
            return result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.News.NewsItemDeleteImage)]
        public async Task<IActionResult> NewsItemDeleteImage()
        {
            var viewModel = await Model.DeleteImage();
            return Json(viewModel);
        }
        #endregion
    }
}
