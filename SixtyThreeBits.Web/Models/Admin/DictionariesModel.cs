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
    public class DictionariesModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var viewModel = new PageViewModel();
            viewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);

            viewModel.Tree = new PageViewModel.TreeModel();
            viewModel.Tree.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);
            viewModel.Tree.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate);
            viewModel.Tree.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete);
            viewModel.Tree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree);
            viewModel.Tree.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);
            viewModel.Tree.UrlUpdate = viewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate);
            viewModel.Tree.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete);

            return viewModel;
        }

        public async Task<List<PageViewModel.TreeModel.TreeItem>> GetTreeModel()
        {
            var repository = RepositoriesFactory.GetDictionariesRepository();
            var viewModel = (await repository.DictionariesList()).Select(Item => new PageViewModel.TreeModel.TreeItem
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

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? dictionaryID, PageViewModel.TreeModel.TreeItem submitModel)
        {
            var repository = RepositoriesFactory.GetDictionariesRepository();
            await repository.DictionariesIUD(
                databaseAction: DatabaseAction,
                dictionaryID: dictionaryID,
                dictionaryParentID: submitModel.DictionaryParentID,
                dictionaryCaption: submitModel.DictionaryCaption,
                dictionaryCaptionEng: submitModel.DictionaryCaptionEng,
                dictionaryStringCode: submitModel.DictionaryStringCode ?? Constants.NullValueFor.String,
                dictionaryIntCode: submitModel.DictionaryIntCode ?? Constants.NullValueFor.Numeric,
                dictionaryDecimalValue: submitModel.DictionaryDecimalValue ?? Constants.NullValueFor.Numeric,
                dictionaryCode: submitModel.DictionaryCode,
                dictionarySortIndex: submitModel.DictionarySortIndex ?? Constants.NullValueFor.Numeric
            );

            if (repository.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        public async Task DeleteRecursive(int? DictionaryID)
        {
            var repository = RepositoriesFactory.GetDictionariesRepository();
            await repository.DictionariesDeleteRecursive(DictionaryID);
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
                public TreeListBuilder<TreeItem> Render(IHtmlHelper html)
                {
                    var tree = GetTreeWithStartupValues<TreeItem>(html: html, keyFieldName: nameof(TreeItem.DictionaryID), parentFieldName: nameof(TreeItem.DictionaryParentID));

                    tree
                    .ID("DictionariesTree")
                    .OnInitialized("dictionariesModel.onTreeInit")
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