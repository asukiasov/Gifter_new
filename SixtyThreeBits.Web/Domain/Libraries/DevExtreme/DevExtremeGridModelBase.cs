using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;

namespace SixtyThreeBits.Web.Domain.Libraries.DevExtreme
{   
    public abstract class DevExtremeGridModelBase<T> where T : class
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
        public DataGridBuilder<T> CreateGridWithStartupValues(IHtmlHelper html, string keyFieldName)
        {
            return html.DevExtreme().DataGrid<T>()
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .SyncLookupFilterValues(false)
            .AllowColumnResizing(true)
            .Toolbar(options =>
            {
                options.Visible(false);
            })
            .Scrolling(options =>
            {
                options.Mode(GridScrollingMode.Standard);
                options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .FilterRow(options =>
            {
                options.Visible(true);
                options.ApplyFilter(GridApplyFilterMode.Auto);
                options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .HeaderFilter(options =>
            {
                options.Visible(true);
            })
            .DataSource(options =>
            {
                var optionsResult = options.RemoteController();
                optionsResult.Key(keyFieldName);
                optionsResult.LoadUrl(UrlLoad);
                optionsResult.InsertUrl(UrlAddNew);
                optionsResult.UpdateUrl(UrlUpdate);
                optionsResult.DeleteUrl(UrlDelete);

                if (!string.IsNullOrWhiteSpace(BeforeSendJSFunction))
                {
                    optionsResult.OnBeforeSend(BeforeSendJSFunction);
                }
                if (LoadParams != null)
                {
                    optionsResult.LoadParams(LoadParams);
                }

                return optionsResult;
            })
            .Editing(options =>
            {
                options.Mode(GridEditMode.Row);
                //options.Mode(GridEditMode.Cell);
                options.AllowAdding(AllowAdd);
                options.AllowUpdating(AllowUpdate);
                options.AllowDeleting(AllowDelete);
                options.Texts(optionsTexts =>
                {
                    optionsTexts.ConfirmDeleteMessage(TextConfirmDelete);

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
                    var width = isAllowedAddOrUpdate ? 60 : 30;
                    var commandColumn = Columns.Add();
                    commandColumn
                        .Width(width)
                        .Type(GridCommandColumnType.Buttons)
                        .Alignment(HorizontalAlignment.Center)
                        .Buttons(b =>
                        {
                            if (AllowUpdate)
                            {
                                b.Add().Name(GridColumnButtonName.Edit).Icon("fa-solid fa-pencil").Text(Resources.TextUpdate);
                            }
                            if (AllowDelete)
                            {
                                b.Add().Name(GridColumnButtonName.Delete).Icon("fa-light fa-trash-can").Text(Resources.TextDelete);
                            }
                            if (isAllowedAddOrUpdate)
                            {
                                b.Add().Name(GridColumnButtonName.Save).Icon("fa-solid fa-check").Text(Resources.TextSave);
                                b.Add().Name(GridColumnButtonName.Cancel).Icon("fa-solid fa-circle-minus").Text(Resources.TextCancel);
                            }

                        });
                }
            });
        }

        public abstract DataGridBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }    
}