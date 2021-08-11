using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SixtyThreeBits.Core.Modules.CategoriesDataAccess;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class CategoriesModel : WebProjectModelBase
    {
        #region Methods
        public async Task<PageViewModel> GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Categories.Add);
            ViewModel.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Add);
            ViewModel.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Delete);
            ViewModel.UrlSync = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Sync);
            ViewModel.Categories = (await DataAccessFactory.Categories.ListCategories())?.Select(Item => new TreeNodeItem
            {
                NodeID = Item.CategoryID.ToString(),
                ParentID = Item.CategoryParentID.HasValue ? Item.CategoryParentID.ToString() : null,
                Caption = Item.CategoryName,
                NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties, new { CategoryID = Item.CategoryID }),
                ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Categories.Add) && Item.CategoryParentID == null,                
                ShowDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.Categories.Delete)                
            }).ToList();
            if (ViewModel.HasCategories)
            {
                ViewModel.Categories.ToRecursive(IDPropertyName: nameof(TreeNodeItem.NodeID), nameof(TreeNodeItem.ParentID), nameof(TreeNodeItem.Children));
            }
            return ViewModel;
        }

        public async Task<AjaxResponse> DeleteRecursive(int? CategoryID)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Categories.DeleteRecursive(CategoryID);
            AR.IsSuccess = !DataAccessFactory.Categories.IsError;
            return AR;
        }

        public async Task<AjaxResponse> CreateCategory(int? CategoryParentID, string CategoryName)
        {
            TreeNodeItem Node = null;

            var CategoryID = await DataAccessFactory.Categories.CategoriesIUD(
                DatabaseAction: Enums.DatabaseActions.CREATE,
                CategoryParentID: CategoryParentID,
                CategoryName: CategoryName
            );

            if (CategoryID > 0)
            {
                Node = new TreeNodeItem();
                Node.NodeID = CategoryID.ToString();
                Node.ParentID = CategoryParentID.HasValue ? CategoryParentID.ToString() : null;
                Node.Caption = CategoryName;
                Node.NavigateUrl = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.Properties, new { CategoryID = CategoryID });                
                Node.ShowDeleteButton = User.HasPermission(ControllerActionRouteNames.Admin.Categories.Delete);
            }

            var AR = new AjaxResponse();

            if (Node != null)
            {
                AR.IsSuccess = true;
                AR.Data = Node;
            }

            return AR;
        }        

        public async Task<AjaxResponse> SyncParentsAndSortIndexes(SyncSortIndexesModel SubmitModel)
        {
            var AR = new AjaxResponse();
            await DataAccessFactory.Categories.SyncParentsAndSortIndexes(SubmitModel.SortIndexes);
            AR.IsSuccess = !DataAccessFactory.Categories.IsError;
            return AR;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool HasCategories => Categories != null && Categories.Count > 0;
            public List<TreeNodeItem> Categories { get; set; }
            public bool ShowAddNewButton { get; set; }
            public string UrlAddNew { get; set; }            
            public string UrlDelete { get; set; }
            public string UrlSync { get; set; }
            public readonly string TextConfirmDeleteRecord = Resources.TextConfirmDelete;
            public readonly string TextConfirmDeleteRecursive = Resources.TextConfirmDeleteRecursive;            
            #endregion            
        }
        #endregion
    }
    public class CategoriesModelBase : WebProjectModelBase
    {
        #region Properties
        public Category DBItemCategories { get; set; }
        #endregion
    }

    public class CategoryPropertiesModel : CategoriesModelBase
    {
        #region Methods
        public CategoryPropertiesViewModel GetPageViewModel(int? CategoryID, CategoryPropertiesViewModel ViewModel)
        {
            if (ViewModel == null)
            {
                ViewModel = new CategoryPropertiesViewModel();
                
                ViewModel.CategoryParentID = DBItemCategories.CategoryParentID;
                ViewModel.CategoryName = DBItemCategories.CategoryName;
                ViewModel.CategoryNameEng = DBItemCategories.CategoryNameEng;
                ViewModel.CategoryNameRus = DBItemCategories.CategoryNameRus;
                ViewModel.CategoryDescriptionShort = DBItemCategories.CategoryDescriptionShort;
                ViewModel.CategoryDescriptionShortEng = DBItemCategories.CategoryDescriptionShortEng;
                ViewModel.CategoryDescriptionShortRus = DBItemCategories.CategoryDescriptionShortRus;
                
            }
            ViewModel.CategoryImageFilename = DBItemCategories.CategoryImageFilename;
            ViewModel.CategoryImageHttpPath = Utilities.GetUploadedFileHttpPath(DBItemCategories.CategoryImageFilename);
            ViewModel.UrlDeleteImage = Url.RouteUrl(ControllerActionRouteNames.Admin.Categories.Category.DeleteImage, new { CategoryID = CategoryID });

            return ViewModel;
        }

        public async Task<AjaxResponse> DeleteImage(int? CategoryID)
        {

            Utilities.DeleteUploadedFile(DBItemCategories.CategoryImageFilename);

            var AR = new AjaxResponse();

            await DataAccessFactory.Categories.CategoriesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                CategoryID: CategoryID,
                CategoryImageFilename: Constants.NullValueFor.String
            );

            AR.IsSuccess = !DataAccessFactory.Categories.IsError;

            return AR;
        }

        public async Task SaveCategoryProperties(int? CategoryID, CategoryPropertiesViewModel ViewModel)
        {
            var HasCategoryImage = ViewModel.PostedFile?.Length > 0;
            var CategoryImageFilename = HasCategoryImage ? GetFilenameFromUploadedFile(ViewModel.PostedFile) : null;

            if (HasCategoryImage)
            {
                Utilities.DeleteUploadedFile(ViewModel.CategoryImageFilename);
            }

            await DataAccessFactory.Categories.CategoriesIUD(
                DatabaseAction: Enums.DatabaseActions.UPDATE,
                CategoryID: CategoryID,
                CategoryParentID: ViewModel.CategoryParentID,
                CategoryName: ViewModel.CategoryName,
                CategoryNameEng: ViewModel.CategoryNameEng ?? Constants.NullValueFor.String,
                CategorynameRus: ViewModel.CategoryNameRus ?? Constants.NullValueFor.String,
                CategoryImageFilename: CategoryImageFilename,
                CategoryDescriptionShort: ViewModel.CategoryDescriptionShort ?? Constants.NullValueFor.String,
                CategoryDescriptionShortEng: ViewModel.CategoryDescriptionShortEng ?? Constants.NullValueFor.String,
                CategoryDescriptionShortRus: ViewModel.CategoryDescriptionShortRus ?? Constants.NullValueFor.String
            );

            if (!DataAccessFactory.Categories.IsError)
            {
                ViewModel.IsSaved = true;
                if (HasCategoryImage)
                {
                    await SaveUploadedFile(PostedFile: ViewModel.PostedFile, Filename: CategoryImageFilename);
                }
            }
        }

        public void ValidatePageViewModel(CategoryPropertiesViewModel ViewModel)
        {
            ViewModel.Errors = new List<SimpleKeyValue<string, string>>
            {

                Validation.ValidateRequired(ErrorKey:$"[name=\"{nameof(ViewModel.CategoryName)}\"]", ValueToValidate:ViewModel.CategoryName)

            };
            ViewModel.Errors.RemoveAll(Item => Item == null);
        }

        #endregion

        #region Sub Classes
        public class CategoryPropertiesViewModel : FormViewModelBase
        {
            public int? CategoryID { get; set; }
            public int? CategoryParentID { get; set; }
            public string CategoryName { get; set; }
            public string CategoryNameEng { get; set; }
            public string CategoryNameRus { get; set; }
            public string CategoryImageFilename { get; set; }
            public string CategoryDescriptionShort { get; set; }
            public string CategoryDescriptionShortEng { get; set; }
            public string CategoryDescriptionShortRus { get; set; }
            public string CategoryImageHttpPath { get; set; }
            public bool HasCategoryImage => !string.IsNullOrWhiteSpace(CategoryImageFilename);
            public string UrlDeleteImage { get; set; }            
            public IFormFile PostedFile { get; set; }

            public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
        }
        #endregion
    }
}
