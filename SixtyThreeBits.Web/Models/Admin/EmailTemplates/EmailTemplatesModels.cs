using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class EmailTemplatesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.Grid = new();
            viewModel.Grid.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplatesController.Grid);
            return viewModel;
        }

        public async Task<List<ViewModel.GridModel.GridItem>> GetGridModel()
        {
            var repository = RepositoriesFactory.CreateEmailTemplatesRepository();
            var viewModel = (await repository.EmailTemplatesList())?
            .Select(Item => new ViewModel.GridModel.GridItem
            {
                EmailTemplateID = Item.EmailTemplateID,
                EmailTemplateName = Item.EmailTemplateName,
                UrlProperties = Url.RouteUrl(ControllerActionRouteNames.Admin.EmailTemplatePropertiesController.Properties, new { emailTemplateID = Item.EmailTemplateID })
            })
            .ToList();
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties            
            public GridModel Grid { get; set; }
            #endregion

            #region Nested Classes
            public class GridModel : DevExtremeGridViewModelBase<GridModel.GridItem>
            {
                #region Methods
                public override DataGridBuilder<GridItem> Render(IHtmlHelper html)
                {
                    var grid = CreateGridWithStartupValues(html: html, keyFieldName: nameof(GridItem.EmailTemplateID));

                    grid
                    .ID("EmailTemplatesGrid")
                    .OnInitialized("model.onGridInit")
                    .Columns(columns =>
                    {
                        columns.Add().Width(30).Caption(" ").InitDetailsUrlCellTemplate(nameof(GridItem.UrlProperties));
                        columns.AddFor(m => m.EmailTemplateID).Caption("#").Width(60);
                        columns.AddFor(m => m.EmailTemplateName).Caption(Resources.TextTemplate).Width(300);
                        columns.Add();
                    });


                    return grid;
                }
                #endregion

                #region Nested Classes
                public class GridItem
                {
                    #region Properties
                    public int? EmailTemplateID { get; set; }
                    public string EmailTemplateName { get; set; }
                    public string UrlProperties { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}