using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;


namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/projects")]
    public class ProjectsController : AdminControllerBase<ProjectsModel>
    {
        #region Constructors
        public ProjectsController()
        {
            Model = new ProjectsModel();
        }
        #endregion

        #region Actions
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.Projects.Index)]
        public ActionResult Project()
        {
            Model.PluginsClient.EnableDevextreme(true);
            var ViewModel = Model.GetPageViewModel();
            return View(ViewNames.Admin.Projects.Page, ViewModel);
        }

        [Route("grid", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGrid)]
        public async Task<ActionResult> ProjectGrid()
        {
            var ViewModel = await Model.GetGridViewModel();
            return Json(ViewModel);
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd)]
        public async Task<ActionResult> ProjectGridAdd(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<ProjectsModel.PageViewModel.GridModel.GridItem>() ?? new ProjectsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.CREATE, ProjectID: key, SubmitModel: SubmitModel);
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
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridUpdate)]
        public async Task<ActionResult> ProjectGridUpdate(int? key, string values)
        {
            var SubmitModel = values.FromJsonTo<ProjectsModel.PageViewModel.GridModel.GridItem>() ?? new ProjectsModel.PageViewModel.GridModel.GridItem();
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.UPDATE, ProjectID: key, SubmitModel: SubmitModel);
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
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridDelete)]
        public async Task<ActionResult> ProjectGridDelete(int? key)
        {
            await Model.CRUD(DatabaseAction: Enums.DatabaseActions.DELETE, ProjectID: key, SubmitModel: new ProjectsModel.PageViewModel.GridModel.GridItem());
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

    [Route("admin/projects/{ProjectID:int}")]
    public class ProjectsPropertiesController : AdminControllerBase<ProjectsPropertiesModel>
    {
        #region Constructors
        public ProjectsPropertiesController()
        {
            Model = new ProjectsPropertiesModel();
        }
        #endregion

        #region Projects Properties
        [HttpGet]
        [Route("properties", Name = ControllerActionRouteNames.Admin.Projects.Project.Properties)]
        public async Task<IActionResult> Properties(int? ProjectID)
        {
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableTinyMce(true);
            var ViewModel = await Model.GetPageViewModel(ProjectID, ViewModel: null);
            if (ViewModel == null)
            {
                return Model.GetNotFoundAdminViewResult();
            }
            else
            {
                return View(ViewNames.Admin.Projects.Project.Properties, ViewModel);
            }
        }

        [HttpPost]
        [Route("properties")]
        public async Task<IActionResult> Properties(ProjectsPropertiesModel.ProjectsPropertiesViewModel SubmitModel, int? ProjectID)
        {
            var Result = default(IActionResult);
            Model.PluginsClient.Enable63BitsForms(true).EnableFancybox(true).EnableDevextreme(true).EnableTinyMce(true);
            var ViewModel = await Model.GetPageViewModel(ProjectID, SubmitModel);
            Model.ValidatePageViewModel(ViewModel);
            if (ViewModel.IsValid)
            {
                await Model.SaveProjectsProperties(ProjectID, ViewModel);
                if (ViewModel.IsSaved)
                {
                    Model.ShowSuccess();
                    Result = Redirect(Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.Project.Properties, new { ProjectID = ProjectID }));
                }
                else
                {
                    Model.ShowError();
                    Result = View(ViewNames.Admin.Projects.Project.Properties, ViewModel);
                }
            }
            else
            {
                Result = View(ViewNames.Admin.Projects.Project.Properties, ViewModel);
            }
            return Result;
        }

        [HttpPost]
        [Route("properties/delete-image", Name = ControllerActionRouteNames.Admin.Projects.Project.DeleteCoverImage)]
        public async Task<IActionResult> ProjectsItemDeleteImage(int? ProjectID)
        {
            var Result = await Model.DeleteImage(ProjectID);
            return Json(Result);
        }
        #endregion
    }
}
