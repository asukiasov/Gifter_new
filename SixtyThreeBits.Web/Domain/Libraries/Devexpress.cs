using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.Libraries
{
    public class DevExtremeGridFilterItem
    {
        #region Properties
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool IsNegation { get; set; }
        #endregion
    }

    public class DevExtremeGridSortItem
    {
        #region Properties
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }
        #endregion
    }

    public abstract class DevExtremeGridViewModelBase<T> where T : class
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

    public abstract class DevExtremeTreeViewModelBase<T> where T : class
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

    public static class DevExtremeBuilderCustomExtensions
    {
        #region Methods
        public static DataGridColumnBuilder<T> InitCheckboxColumn<T>(this DataGridColumnBuilder<T> column, bool allowNull = false, bool defaultValue = false)
        {
            column.TrueText(Resources.TextYes);
            column.FalseText(Resources.TextNo);
            if (!allowNull)
            {
                column.CalculateCellValue($"function(e){{ var DataField = this.dataField;  var Value = e[DataField]; if ($.isEmptyObject(e)) {{ e[DataField] = {defaultValue.ToString().ToLower()}; }} else if(Value == null){{e[DataField] = false;}}  return e[DataField]; }}");
            }
            return column;
        }

        public static DataGridColumnBuilder<T> InitDateColumn<T>(this DataGridColumnBuilder<T> column, bool formatDateTime = false)
        {
            if (formatDateTime)
            {
                column.Format(Constants.Formats.DateTime);
            }
            else
            {
                column.Format(Constants.Formats.Date);
            }
            return column;
        }

        public static DateBoxBuilder InitDateBox(this DateBoxBuilder dateBox, bool formatDateTime = false)
        {
            if (formatDateTime)
            {
                dateBox.DisplayFormat(Constants.Formats.DateTime);
            }
            else
            {
                dateBox.DisplayFormat(Constants.Formats.Date);
            }

            return dateBox;
        }

        public static DataGridColumnBuilder<T> InitDetailsUrlCellTemplate<T>(this DataGridColumnBuilder<T> column, string urlPropertyName)
        {
            return column.Alignment(HorizontalAlignment.Center).CellTemplate($"<a href=\"<%-data.{urlPropertyName}%>\"><i class=\"fa-solid fa-circle-info\"></i></a>");
        }

        public static DataGridColumnBuilder<T1> InitLookupColumn<T1, T2, T3>(this DataGridColumnBuilder<T1> column, IEnumerable<KeyValueTuple<T2, T3>> data, bool isRequired = false, bool allowNull = false)
        {
            column.Lookup(options =>
            {
                options.DataSource(d => d.Array().Data(data).Key(nameof(KeyValueTuple<T2, T3>.Key))).ValueExpr(nameof(KeyValueTuple<T2, T3>.Key)).DisplayExpr(nameof(KeyValueTuple<T2, T3>.Value));
                options.AllowClearing(allowNull);
            });

            if (isRequired)
            {
                column.ValidationRules(options =>
                {
                    options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                });
            }
            return column;
        }

        public static DataGridColumnBuilder<T> InitNumberColumn<T>(this DataGridColumnBuilder<T> column, NumberColumnFormatType format = NumberColumnFormatType.Default)
        {
            switch (format)
            {
                case NumberColumnFormatType.Money:
                    {
                        column.Format(options =>
                        {
                            options.Type(Format.FixedPoint);
                            options.Precision(2);
                        });
                        break;
                    }
                case NumberColumnFormatType.Quantity:
                    {
                        column.Format(options =>
                        {
                            options.Type(Format.FixedPoint);
                            options.Precision(0);
                        });
                        break;
                    }
            }
            return column;
        }

        public static DataGridColumnBuilder<T> InitTextboxColumn<T>(this DataGridColumnBuilder<T> column, bool isRequired = false, bool shouldValidateEmailFormat = false, int? maxLength = null)
        {
            column.ValidationRules(options =>
            {
                if (isRequired)
                {
                    options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                }
                if (shouldValidateEmailFormat)
                {
                    options.AddEmail().Message(Resources.ValidationEmailFormatInvalid);
                }
                if (maxLength > 0)
                {
                    options.AddStringLength().Min(1).Max(maxLength.Value).Message(string.Format(Resources.ValidationTextMaxLength, maxLength));
                }
            });
            return column;
        }        
        #endregion

        #region Enums
        public enum NumberColumnFormatType
        {
            Default,
            Money,
            Quantity
        }
        #endregion
    }
}