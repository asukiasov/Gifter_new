using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
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

        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.News.Page)]
        public ActionResult News()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.News.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.News.Grid)]
        public async Task<ActionResult> NewsGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.News.GridAdd)]
        public async Task<ActionResult> NewsGridAdd(int? key, string values)
        {
            var SubmitModel = values.DeserializeJsonTo<NewsModel.PageViewModel.GridModel.GridItem>() ?? new NewsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, NewsID: key, SubmitModel: SubmitModel);
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
            var SubmitModel = values.DeserializeJsonTo<NewsModel.PageViewModel.GridModel.GridItem>() ?? new NewsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, NewsID: key, SubmitModel: SubmitModel);
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
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, NewsID: key, SubmitModel: new NewsModel.PageViewModel.GridModel.GridItem());
            if (Model.Form.HasErrors)
            {
                return GetDevexpressErrorResult(Model.Form.ErrorMessage);
            }
            else
            {
                return GetDevexpressSuccessResult();
            }
        }

    }

    [Route("admin/news/{NewsID:int}")]
    [TypeFilter(typeof(BeforeNewsPageLoad), Order = 2)]
    public class NewsPropertiesController : AdminControllerBase<NewsPropertiesModel>
    {
        #region Constructors
        public NewsPropertiesController()
        {
            Model = new NewsPropertiesModel();
        }
        #endregion

        #region News Properties
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.News.NewsItem)]
        public IActionResult Properties()
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetNewsPropertiesViewModel(ViewModel: null);
            return View(ViewNames.Admin.News.NewsItem, ViewModel);
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(NewsPropertiesModel.NewsPropertiesViewModel SubmitModel)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = Model.GetNewsPropertiesViewModel(ViewModel: SubmitModel);

            await Model.ValidateNewsPropertiesViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                var IsSaved = await Model.SaveNewsProperties(ViewModel);
                if (IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.News.NewsItem, new { NewsID = Model.DBItemNews.NewsID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.News.NewsItem, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.News.NewsItem, ViewModel);
            }
            
            return Result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.News.NewsItemDeleteImage)]
        public async Task<IActionResult> NewsItemDeleteImage(int? NewsID)
        {
            var Result = await Model.DeleteImage(NewsID);
            return Json(Result);
        }
        #endregion
    }

}
