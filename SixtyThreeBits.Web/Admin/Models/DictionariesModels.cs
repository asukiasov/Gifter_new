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
    public class DictionariesModel : WebProjectModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel()
        {
            var ViewModel = new PageViewModel();
            ViewModel.ShowAddNewButton = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);

            ViewModel.Tree = new PageViewModel.TreeModel();            
            ViewModel.Tree.AllowAdd = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);
            ViewModel.Tree.AllowUpdate = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate);
            ViewModel.Tree.AllowDelete = User.HasPermission(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete);
            ViewModel.Tree.UrlLoad = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTree);
            ViewModel.Tree.UrlAddNew = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeAdd);
            ViewModel.Tree.UrlUpdate = ViewModel.UrlUpdate = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeUpdate);
            ViewModel.Tree.UrlDelete = Url.RouteUrl(ControllerActionRouteNames.Admin.Dictionaries.DictionariesTreeDelete);

            return ViewModel;
        }

        public async Task<List<PageViewModel.TreeModel.TreeItem>> GetTreeModel()
        {            
            var ViewModel = (await DataAccessFactory.Dictionaries.ListDictionaries()).Select(Item => new PageViewModel.TreeModel.TreeItem
            {
                DictionaryID = Item.DictionaryID,
                DictionaryParentID = Item.DictionaryParentID,
                DictionaryCaption = Item.DictionaryCaption,
                DictionaryCaptionEng = Item.DictionaryCaptionEng,
                DictionaryCaptionRus = Item.DictionaryCaptionRus,
                DictionaryStringCode = Item.DictionaryStringCode,
                DictionaryIntCode = Item.DictionaryIntCode,
                DictionaryDecimalValue = Item.DictionaryDecimalValue,
                DictionaryCode = Item.DictionaryCode,
                DictionarySortIndex = Item.DictionarySortIndex
            }).ToList();
            return ViewModel;
        }

        public async Task CRUD(Enums.DatabaseActions DatabaseAction, int? DictionaryID, PageViewModel.TreeModel.TreeItem SubmitModel)
        {
            await DataAccessFactory.Dictionaries.DictionariesIUD(
                DatabaseAction: DatabaseAction,
                DictionaryID: DictionaryID,
                DictionaryParentID: SubmitModel.DictionaryParentID,
                DictionaryCaption: SubmitModel.DictionaryCaption,
                DictionaryCaptionEng: SubmitModel.DictionaryCaptionEng,
                DictionaryCaptionRus: SubmitModel.DictionaryCaptionRus,
                DictionaryStringCode: SubmitModel.DictionaryStringCode,
                DictionaryIntCode: SubmitModel.DictionaryIntCode,
                DictionaryDecimalValue: SubmitModel.DictionaryDecimalValue,
                DictionaryCode: SubmitModel.DictionaryCode,
                DictionarySortIndex: SubmitModel.DictionarySortIndex
            );

            if (DataAccessFactory.Dictionaries.IsError)
            {
                Form.AddError(Resources.TextError);
            }
        }

        public async Task DeleteRecursive(int? DictionaryID)
        {
            await DataAccessFactory.Dictionaries.DeleteRecursive(DictionaryID);
            if (DataAccessFactory.Dictionaries.IsError)
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
            public TreeModel Tree { get; set; }
            public string UrlUpdate { get; set; }
            #endregion

            #region Sub Classes
            public class TreeModel : DevExtremeGridViewModelBase, IDevExtremeTreeModel<TreeModel.TreeItem>
            {
                #region Methods
                public TreeListBuilder<TreeItem> Render(IHtmlHelper Html)
                {
                    var Tree = GetTreeWithStartupValues<TreeItem>(Html: Html, KeyFieldName: nameof(TreeItem.DictionaryID), ParentFieldName: nameof(TreeItem.DictionaryParentID));

                    Tree
                    .ID("DictionariesTree")
                    .OnToolbarPreparing("DictionariesModel.OnDictionariesTreeToolbarPreparing")
                    .OnInitialized("DictionariesModel.OnDictionariesTreeInit")
                    .RowDragging(Options =>
                    {
                        if (AllowUpdate)
                        {
                            Options.AllowDropInsideItem(true);
                            Options.AllowReordering(false);
                            Options.ShowDragIcons(true);
                            Options.OnReorder("DictionariesModel.OnDictionariesTreeReorder");
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
                        Columns.AddFor(m => m.DictionaryCaption).Caption("Caption").Width(200).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        });
                        Columns.AddFor(m => m.DictionaryCaptionEng).Caption("Caption Eng").Width(200);
                        Columns.AddFor(m => m.DictionaryCaptionRus).Caption("Caption Rus").Width(200);
                        Columns.AddFor(m => m.DictionaryStringCode).Caption("String Code").Width(200);
                        Columns.AddFor(m => m.DictionaryIntCode).Caption("Int Code").DataType(GridColumnDataType.Number).Width(100);
                        Columns.AddFor(m => m.DictionaryCode).Caption("Dic. Code").DataType(GridColumnDataType.Number).Width(100).ValidationRules(Options =>
                        {
                            Options.AddRequired();
                        }); 
                        Columns.AddFor(m => m.DictionarySortIndex).Caption("Sort Index").Width(100);
                        

                        Columns.AddFor(m => m.DictionaryID).Caption("ID").EditCellTemplate($"<%= data.{nameof(TreeItem.DictionaryID)} %>");

                        Columns.Add();
                    });


                    return Tree;
                }
                #endregion


                #region Sub CLasses
                public class TreeItem
                {
                    #region Properties
                    public int? DictionaryID { get; set; }
                    public int? DictionaryParentID { get; set; }
                    public string DictionaryCaption { get; set; }
                    public string DictionaryCaptionEng { get; set; }
                    public string DictionaryCaptionRus { get; set; }
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