using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Web.Reusables.Core;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class ProjectModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = true;

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.AllowAdd = true;
            ViewModel.Grid.AllowUpdate = true;
            ViewModel.Grid.AllowDelete = true;
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGrid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Projects.ProjectsGridDelete);

            return ViewModel;
        }
        #endregion

        #region Sub Classes
        public class PageViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public GridModel Grid { get; set; }
            #endregion

            #region Sub Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.ProjectID));

                    Grid
                    .ID("ProjectsGrid")
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.ProjectCaption).Caption("Caption").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.ProjectIsPublished).Caption("Published").DataType(GridColumnDataType.Boolean).Width(50);
                        Columns.Add();
                    });


                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? ProjectID { get; set; }
                    public string ProjectCaption { get; set; }
                    public bool ProjectIsPublished { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
