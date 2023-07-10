using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class RedirectsModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Redirects.GridAdd);

            ViewModel.Grid = new PageViewModel.GridModel();
            ViewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Redirects.Grid);
            ViewModel.Grid.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Redirects.GridAdd);
            ViewModel.Grid.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Redirects.GridUpdate);
            ViewModel.Grid.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Redirects.GridDelete);
            ViewModel.Grid.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Redirects.GridAdd);
            ViewModel.Grid.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Redirects.GridUpdate);
            ViewModel.Grid.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Redirects.GridDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.GridModel.GridItem>> GetGridViewModel()
        {
            var Redirects = (await DataAccessFactory.Redirects.RedirectsList())?.Select(Item => new PageViewModel.GridModel.GridItem
            {
                RedirectID = Item.RedirectID,                
                RedirectFrom = Item.RedirectFrom,
                RedirectTo = Item.RedirectTo
            }).ToList();
            return Redirects;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? RedirectID, PageViewModel.GridModel.GridItem SubmitModel)
        {

            await DataAccessFactory.Redirects.RedirectsIUD(
                DatabaseAction: DatabaseAction,
                RedirectID: RedirectID,
                RedirectFrom: SubmitModel.RedirectFrom,
                RedirectTo: SubmitModel.RedirectTo                
            );

            if (DataAccessFactory.Redirects.IsError)
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
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase, IDevExtremeGridModel<GridModel.GridItem>
            {                
                #region Methods
                public DataGridBuilder<GridItem> Render(IHtmlHelper Html)
                {
                    var Grid = GetGridWithStartupValues<GridItem>(Html: Html, KeyFieldName: nameof(GridItem.RedirectID));

                    Grid
                   .ID("RedirectsGrid")                   
                   .OnInitialized("RedirectsModel.OnGridInit")
                   .Columns(Columns =>
                   {
                       Columns.AddFor(m => m.RedirectFrom).Caption("Redirect From").Width(500);
                       Columns.AddFor(m => m.RedirectTo).Caption("Redirect To").Width(500);                       
                       Columns.Add();
                   });

                    return Grid;

                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? RedirectID { get; set; }
                    public string RedirectFrom { get; set; }
                    public string RedirectTo { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }    
}