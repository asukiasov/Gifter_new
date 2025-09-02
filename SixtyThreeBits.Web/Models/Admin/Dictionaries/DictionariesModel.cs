using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using SixtyThreeBits.Web.Domain.Libraries;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class DictionariesModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.DictionariesController.TreeAdd);

            viewModel.Tree = new ViewModel.TreeModel();
            viewModel.Tree.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.DictionariesController.TreeAdd);
            viewModel.Tree.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.DictionariesController.TreeUpdate);
            viewModel.Tree.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.DictionariesController.TreeDelete);
            viewModel.Tree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.DictionariesController.Tree);
            viewModel.Tree.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.DictionariesController.TreeAdd);
            viewModel.Tree.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.DictionariesController.TreeUpdate);
            viewModel.Tree.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.DictionariesController.TreeDelete);

            return viewModel;
        }

        public async Task<AjaxResponse> GetTreeItems()
        {
            var viewModel = new AjaxResponse();            
            var repository = RepositoriesFactory.CreateDictionariesRepository();

            var dictionaries = (await repository.DictionariesList());

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.IsError ? repository.ErrorMessage : dictionaries.Select(Item => new ViewModel.TreeModel.TreeItem
            {
                DictionaryID = Item.DictionaryID,
                DictionaryParentID = Item.DictionaryParentID,
                DictionaryCaption = Item.DictionaryCaption,
                DictionaryCaptionEng = Item.DictionaryCaptionEng,
                DictionaryStringCode = Item.DictionaryStringCode,
                DictionaryIntCode = Item.DictionaryIntCode,
                DictionaryDecimalValue = Item.DictionaryDecimalValue,
                DictionaryCode = Item.DictionaryCode,
                DictionarySortIndex = Item.DictionarySortIndex
            }).ToList();

            return viewModel;
        }

        public async Task<AjaxResponse> IUD(Enums.DatabaseActions databaseAction, int? dictionaryID, ViewModel.TreeModel.TreeItem submitModel)
        {
            var viewModel = new AjaxResponse();

            var repository = RepositoriesFactory.CreateDictionariesRepository();
            await repository.DictionariesIUD(
                databaseAction: databaseAction,
                dictionaryID: dictionaryID,
                dictionary: new DictionariesIudDTO
                {
                    DictionaryParentID = submitModel.DictionaryParentID,
                    DictionaryCaption = submitModel.DictionaryCaption,
                    DictionaryCaptionEng = submitModel.DictionaryCaptionEng,
                    DictionaryStringCode = submitModel.DictionaryStringCode ?? Constants.NullValueFor.String,
                    DictionaryIntCode = submitModel.DictionaryIntCode ?? Constants.NullValueFor.Numeric,
                    DictionaryDecimalValue = submitModel.DictionaryDecimalValue ?? Constants.NullValueFor.Numeric,
                    DictionaryCode = submitModel.DictionaryCode,
                    DictionarySortIndex = submitModel.DictionarySortIndex ?? Constants.NullValueFor.Numeric
                }                
            );

            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;

            return viewModel;
        }

        public async Task<AjaxResponse> DeleteRecursive(int? dictionaryID)
        {
            var viewModel = new AjaxResponse();
            var repository = RepositoriesFactory.CreateDictionariesRepository();
            await repository.DictionariesDeleteRecursive(dictionaryID);
            viewModel.IsSuccess = !repository.IsError;
            viewModel.Data = repository.ErrorMessage;
            return viewModel;
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public bool ShowAddNewButton { get; set; }
            public TreeModel Tree { get; set; }            
            #endregion

            #region Nested Classes
            public class TreeModel : DevExtremeTreeViewModelBase<TreeModel.TreeItem>
            {
                #region Methods
                public override TreeListBuilder<TreeItem> Render(IHtmlHelper html)
                {
                    var tree = CreateTreeWithStartupValues(html: html, keyFieldName: nameof(TreeItem.DictionaryID), parentFieldName: nameof(TreeItem.DictionaryParentID));

                    tree
                    .ID("DictionariesTree")
                    .OnInitialized("model.onTreeInit")
                    .OnRowUpdating("model.onTreeRowUpdating")
                    .AutoExpandAll(false)
                    .Pager(options =>
                    {
                        options.ShowInfo(false);
                    })
                    .Paging(options =>
                    {
                        options.Enabled(false);
                    })
                    .Columns(columns =>
                    {
                        columns.AddFor(m => m.DictionaryCaption).Caption(Resources.TextCaption).Width(300).ValidationRules(options =>
                        {
                            options.AddRequired();
                        });
                        columns.AddFor(m => m.DictionaryCaptionEng).Caption(Resources.TextCaptionEng).Width(200);
                        columns.AddFor(m => m.DictionaryStringCode).Caption(Resources.TextStringCode).Width(150);
                        columns.AddFor(m => m.DictionaryIntCode).Caption(Resources.TextIntCode).DataType(GridColumnDataType.Number).Width(150);
                        columns.AddFor(m => m.DictionaryCode).Caption(Resources.TextDictionaryCode).DataType(GridColumnDataType.Number).Width(150);
                        columns.AddFor(m => m.DictionarySortIndex).Caption(Resources.TextSortIndex).Width(150);


                        columns.AddFor(m => m.DictionaryID).Caption("ID").EditCellTemplate($"<%= data.{nameof(TreeItem.DictionaryID)} %>").Width(100);

                        columns.Add();
                    });


                    return tree;
                }
                #endregion

                #region Nested Classes
                public class TreeItem
                {
                    #region Properties
                    public int? DictionaryID { get; set; }
                    public int? DictionaryParentID { get; set; }
                    public string DictionaryCaption { get; set; }
                    public string DictionaryCaptionEng { get; set; }
                    public string DictionaryStringCode { get; set; }
                    public int? DictionaryIntCode { get; set; }
                    public decimal? DictionaryDecimalValue { get; set; }
                    public int? DictionaryCode { get; set; }
                    public int? DictionarySortIndex { get; set; }
                    #endregion
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}