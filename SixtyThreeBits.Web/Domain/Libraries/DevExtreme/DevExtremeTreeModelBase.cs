using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Domain.Libraries.DevExtreme
{      
    public abstract class DevExtremeTreeModelBase<T> where T : class
    {
        #region Properties        
        public bool AllowAdd { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }

        public string UrlLoad { get; set; }
        public object LoadParams { get; set; }
        public string UrlAddNew { get; set; }
        public string UrlUpdate { get; set; }
        public string UrlDelete { get; set; }

        public string BeforeSendJSFunction { get; set; }

        public bool IsError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
        public string TextConfirmDelete { get; set; } = Resources.TextConfirmDelete;
        #endregion

        #region Methods
        public TreeListBuilder<T> CreateTreeWithStartupValues(IHtmlHelper html, string keyFieldName, string parentFieldName)
        {
            return html.DevExtreme().TreeList<T>()
            .KeyExpr(keyFieldName)
            .ParentIdExpr(parentFieldName)
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .SyncLookupFilterValues(false)
            .AutoExpandAll(true)
            .RootValue(null)
            .Toolbar(options =>
            {
                options.Visible(false);
            })
            .Scrolling(options =>
            {
                options.Mode(TreeListScrollingMode.Standard);
                options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .FilterRow(options =>
            {
                options.Visible(true);
                options.ApplyFilter(GridApplyFilterMode.Auto);
                options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .DataSource(options =>
                options.RemoteController()
                .LoadUrl(UrlLoad)
                .InsertUrl(UrlAddNew)
                .UpdateUrl(UrlUpdate)
                .DeleteUrl(UrlDelete)
                .Key(keyFieldName)
            )
            .Editing(options =>
            {
                //options.Mode(GridEditMode.Cell);
                options.Mode(GridEditMode.Row);
                options.AllowAdding(AllowAdd);
                options.AllowUpdating(AllowUpdate);
                options.AllowDeleting(AllowDelete);
                options.Texts(optionsTexts =>
                {
                    optionsTexts.ConfirmDeleteMessage(Resources.TextConfirmDelete);
                });

            })
            .Pager(options =>
            {
                options.AllowedPageSizes(new[] { 30, 50, 100 });
                options.ShowInfo(true);
                options.ShowPageSizeSelector(true);
                options.Visible(true);
            })
            .Paging(options =>
            {
                options.Enabled(true);
                options.PageSize(30);
            })
            .Columns(Columns =>
            {
                if (AllowAdd || AllowUpdate || AllowDelete)
                {                    
					var isAllowedAddOrUpdate = AllowAdd || AllowUpdate;
                    var isAllowedAll = AllowAdd && AllowUpdate && AllowDelete;
                    var width = isAllowedAll ? 90 : (isAllowedAddOrUpdate ? 60 : 30);
                    var commandColumn = Columns.Add();
                    commandColumn
                        .Width(width)
                        .Type(TreeListCommandColumnType.Buttons)
                        .Alignment(HorizontalAlignment.Center)
                        .Buttons(b =>
                        {
                            if (AllowAdd)
                            {
                                b.Add().Name(TreeListColumnButtonName.Add).Icon("fa-solid fa-plus").Text(Resources.TextAdd);
                            }
                            if (AllowUpdate)
                            {
                                b.Add().Name(TreeListColumnButtonName.Edit).Icon("fa-solid fa-pencil").Text(Resources.TextUpdate);
                            }
                            if (AllowDelete)
                            {
                                b.Add().Name(TreeListColumnButtonName.Delete).Icon("fa-light fa-trash-can").Text(Resources.TextDelete);
                            }
                            if (isAllowedAddOrUpdate)
                            {
                                b.Add().Name(TreeListColumnButtonName.Save).Icon("fa-solid fa-check").Text(Resources.TextSave);
                                b.Add().Name(TreeListColumnButtonName.Cancel).Icon("fa-solid fa-circle-minus").Text(Resources.TextCancel);
                            }
                        });
                }
            });
        }

        public abstract TreeListBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }
}