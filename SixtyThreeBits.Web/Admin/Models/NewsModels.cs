using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Services;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class NewsModel : WebProjectModelBase
    {

        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.News.GridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.News.GridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.News.GridDelete);
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.News.Grid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.News.GridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var ViewModel = (await DataAccessFactory.News.ListNews()).Select(Item => new PageViewModel.GridModel.GridItem
            {
                NewsID = Item.NewsID,
                NewsTitle = Item.NewsTitle,
                NewsDatePublished = Item.NewsDatePublished,
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? NewsID, PageViewModel.GridModel.GridItem SubmitModel)
        {
            if (DatabaseAction == Enums.DatabaseActions.DELETE)
            {
                var DBItem = await DataAccessFactory.News.GetSingleNewsByID(NewsID);
                Utilities.DeleteUploadedFile(DBItem?.NewsImageFilename);
            }
            await DataAccessFactory.News.NewsIUD(
                DatabaseAction: DatabaseAction,
                NewsID: NewsID,
                NewsTitle: SubmitModel.NewsTitle,
                NewsDatePublished: SubmitModel.NewsDatePublished
            );

            if (DataAccessFactory.News.IsError)
            {
                Form.AddError(Resources.TextError);
            }
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
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.NewsID));

                    Grid
                    .ID("NewsGrid")
                    .OnInitialized("NewsModel.OnNewsGridInit")
                    .Columns(Columns =>
                    {
                        Columns.AddFor(m => m.NewsTitle).Caption("Title").Width(400).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        var ColumnNewsDatePublished = Columns.AddFor(m => m.NewsDatePublished).Caption("Publish Date").DataType(GridColumnDataType.Date).Width(150);
                        InitDateColumn(ColumnNewsDatePublished);
                        Columns.Add();
                    });

                    return Grid;
                }
                #endregion

                #region Sub CLasses
                public class GridItem
                {
                    #region Properties
                    public int? NewsID { get; set; }
                    public string NewsTitle { get; set; }
                    public DateTime? NewsDatePublished { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
