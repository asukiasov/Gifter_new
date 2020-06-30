using DevExtreme.AspNet.Mvc.FileManagement;
using Microsoft.AspNetCore.Mvc;
using SixtyThreeBits.Web.Admin.Models;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Controllers
{
    [Route("admin/file-manager")]
    public class FileManagerController : AdminControllerBase<FileManagerModel>
    {
        #region Constructors
        public FileManagerController()
        {
            Model = new FileManagerModel();
        }
        #endregion

        #region Methods
        [HttpGet]
        [Route("", Name = ControllerActionRouteNames.Admin.FileManager.Index)]
        public IActionResult FileManager(string FolderVirtualPathHash, string FolderPhysicalPathHash, bool AllowSelectMultiple = false, string AllowedExtensions = null, string OnSelectedFilesChooseClientCallback = null)
        {            
            var ViewModel = Model.GetPageViewModel(FolderVirtualPathHash, FolderPhysicalPathHash, AllowSelectMultiple, AllowedExtensions, OnSelectedFilesChooseClientCallback, Request.Query["opener"]);
            return View(ViewNames.Admin.FileManager.Page, ViewModel);
        }

        [Route("files", Name = ControllerActionRouteNames.Admin.FileManager.Files)]
        public object FileSystem(FileSystemCommand command, string arguments, string FolderVirtualPathHash, string FolderPhysicalPathHash)
        {
            var Result = Model.GetFileManagerResult(Request, command, arguments, FolderVirtualPathHash, FolderPhysicalPathHash);
            return Result;
        }

        
        #endregion
    }
}