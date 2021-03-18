using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Admin.Filters;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;


namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/projects")]
    public class ProjectsController : AdminControllerBase<ProjectModel>
    {
        #region Constructors
        public ProjectsController()
        {
            Model = new ProjectModel();
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
        public  ActionResult ProjectGrid()
        {
            return null;
        }

        [HttpPost]
        [Route("grid/add", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd)]
        public  ActionResult ProjectGridAdd(int? key, string values)
        {
            return null;
        }

        [HttpPut]
        [Route("grid/update", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridUpdate)]
        public ActionResult ProjectGridUpdate(int? key, string values)
        {
            return null;
        }

        [HttpDelete]
        [Route("grid/delete", Name = ControllerActionRouteNames.Admin.Projects.ProjectsGridDelete)]
        public ActionResult ProjectGridDelete(int? key)
        {
            return null;
        }
        #endregion
    }
}
