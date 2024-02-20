using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Models.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class PermissionsModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Permissions.TreeAdd);

            viewModel.Tree = new PageViewModel.TreeModel();
            viewModel.Tree.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Permissions.TreeAdd);
            viewModel.Tree.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Permissions.TreeUpdate);
            viewModel.Tree.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Permissions.TreeDelete);
            viewModel.Tree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Permissions.Tree);
            viewModel.Tree.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Permissions.TreeAdd);
            viewModel.Tree.UrlUpdate = viewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Permissions.TreeUpdate);
            viewModel.Tree.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Permissions.TreeDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.TreeModel.TreeItem>> GetGridViewModel()
        {
            var repository = RepositoriesFactory.GetPermissionsRepository();
            var viewModel = (await repository.PermissionsList()).Select(item => new PageViewModel.TreeModel.TreeItem
            {
                PermissionID = item.PermissionID,
                PermissionParentID = item.PermissionParentID,
                PermissionCaption = item.PermissionCaption,
                PermissionCaptionEng = item.PermissionCaptionEng,
                PermissionPagePath = item.PermissionPagePath,
                PermissionCodeName = item.PermissionCodeName,
                PermissionCode = item.PermissionCode,
                PermissionIsMenuItem = item.PermissionIsMenuItem,
                PermissionMenuIcon = item.PermissionMenuIcon,
                PermissionMenuTitle = item.PermissionMenuTitle,
                PermissionMenuTitleEng = item.PermissionMenuTitleEng,
                PermissionSortIndex = item.PermissionSortIndex
            }).ToList();
            return viewModel;
        }

        public async Task CRUD(Enums.DatabaseActions databaseAction, int? permissionID, PageViewModel.TreeModel.TreeItem submitModel)
        {
            var repository = RepositoriesFactory.GetPermissionsRepository();
            await repository.PermissionsIUD(
                databaseAction: databaseAction,
                permissionID: permissionID,
                permissionParentID: submitModel.PermissionParentID,
                permissionCaption: submitModel.PermissionCaption,
                permissionCaptionEng: submitModel.PermissionCaptionEng,
                permissionPagePath: submitModel.PermissionPagePath,
                permissionCodeName: submitModel.PermissionCodeName,
                permissionCode: submitModel.PermissionCode,
                permissionIsMenuItem: submitModel.PermissionIsMenuItem,
                permissionMenuIcon: submitModel.PermissionMenuIcon,
                permissionMenuTitle: submitModel.PermissionMenuTitle,
                permissionMenuTitleEng: submitModel.PermissionMenuTitleEng,
                permissionSortIndex: submitModel.PermissionSortIndex
            );

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        public async Task DeleteRecursive(int? permissionID)
        {
            var repository = RepositoriesFactory.GetPermissionsRepository();
            await repository.PermissionsDeleteRecursive(permissionID);
            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public TreeModel Tree { get; set; }
            public string UrlUpdate { get; set; }
            #endregion

            #region Nested Classes
            public class TreeModel : DevExtremeGridViewModelBase, IDevExtremeTreeModel<TreeModel.TreeItem>
            {
                #region Methods
                public TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var Tree = GetTreeWithStartupValues<TreeItem>(html: Html, keyFieldName: nameof(TreeItem.PermissionID), parentFieldName: nameof(TreeItem.PermissionParentID));

                    Tree
                    .ID("PermissionsTree")
                    .OnInitialized("permissionsModel.onTreeInit")
                    .OnInitNewRow("permissionsModel.onTreeInitNewRow")
                    .RowDragging(Options =>
                    {
                        if (AllowUpdate)
                        {
                            Options.AllowDropInsideItem(true);
                            Options.AllowReordering(false);
                            Options.ShowDragIcons(true);
                            Options.OnReorder("permissionsModel.onTreeReorder");
                        }
                    })
                    .AutoExpandAll(false)
                    .Pager(Options =>
                    {
                        Options.ShowInfo(false);
                    })
                    .Paging(Options =>
                    {
                        Options.Enabled(false);
                    })
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.PermissionCaption).Caption(Resources.TextCaption).Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.PermissionCaptionEng).Caption(Resources.TextCaptionEng).Width(200);
                        Columns.AddFor(m => m.PermissionPagePath).Caption(Resources.TextPageUrl).Width(300);
                        Columns.AddFor(m => m.PermissionCodeName).Caption(Resources.TextCodename).Width(300);
                        Columns.AddFor(m => m.PermissionSortIndex).Caption(Resources.TextSortIndex).DataType(GridColumnDataType.Number).Width(100);
                        Columns.AddFor(m => m.PermissionIsMenuItem).Caption(Resources.TextMenu).DataType(GridColumnDataType.Boolean).Width(80);
                        Columns.AddFor(m => m.PermissionMenuTitle).Caption(Resources.TextMenuTitle).Width(200);
                        Columns.AddFor(m => m.PermissionMenuTitleEng).Caption(Resources.TextMenuTitleEng).Width(200);
                        Columns.AddFor(m => m.PermissionMenuIcon).Caption(Resources.TextMenuIcon).Width(100);
                        Columns.AddFor(m => m.PermissionCode).Caption(Resources.TextCode).Width(250);
                        Columns.Add();

                    });

                    return Tree;
                }
                #endregion

                #region Nested Classes
                public class TreeItem
                {
                    #region Properties
                    public int? PermissionID { get; set; }
                    public int? PermissionParentID { get; set; }
                    public string PermissionCaption { get; set; }
                    public string PermissionCaptionEng { get; set; }
                    public string PermissionPagePath { get; set; }
                    public string PermissionCodeName { get; set; }
                    public string PermissionCode { get; set; }
                    public bool? PermissionIsMenuItem { get; set; }
                    public string PermissionMenuIcon { get; set; }
                    public string PermissionMenuTitle { get; set; }
                    public string PermissionMenuTitleEng { get; set; }
                    public int? PermissionSortIndex { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
