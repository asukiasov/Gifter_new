using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Reusables.Core
{
    public interface IDevExtremeGridModel<T> where T : class
    {
        #region Properties
        DataGridBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }

    public interface IDevExtremeTreeModel<T> where T : class
    {
        #region Properties
        TreeListBuilder<T> Render(IHtmlHelper Html);
        #endregion
    }

    public class DevExtremeGridFilterItem
    {
        #region Properties
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        #endregion
    }

    public class DevExtremeGridSortItem
    {
        #region Properties
        public string FieldName { get; set; }
        public bool IsDescending { get; set; }
        #endregion
    }

    public class DevExtremeGridViewModelBase
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
        #endregion        

        #region Methods
        public DataGridBuilder<T> GetGridWithStartupValues<T>(IHtmlHelper Html, string KeyFieldName)
        {
            return Html.DevExtreme().DataGrid<T>()
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .Toolbar(Options =>
            {
                Options.Visible(false);
            })
            .Scrolling(Options =>
            {
                Options.Mode(GridScrollingMode.Standard);
                Options.ShowScrollbar(ShowScrollbarMode.Always);
            })            
            .FilterRow(Options =>
            {
                Options.Visible(true);
                Options.ApplyFilter(GridApplyFilterMode.Auto);
                Options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .DataSource(Options =>
            {
                var OptionsResult = Options.RemoteController();
                OptionsResult.Key(KeyFieldName);
                OptionsResult.LoadUrl(UrlLoad);
                OptionsResult.InsertUrl(UrlAddNew);
                OptionsResult.UpdateUrl(UrlUpdate);
                OptionsResult.DeleteUrl(UrlDelete);

                if (!string.IsNullOrWhiteSpace(BeforeSendJSFunction))
                {
                    OptionsResult.OnBeforeSend(BeforeSendJSFunction);
                }
                if (LoadParams != null)
                {
                    OptionsResult.LoadParams(LoadParams);
                }

                return OptionsResult;
            })
            .Editing(Options =>
            {
                Options.Mode(GridEditMode.Row);
                //Options.Mode(GridEditMode.Cell);
                Options.AllowAdding(AllowAdd);
                Options.AllowUpdating(AllowUpdate);
                Options.AllowDeleting(AllowDelete);
                Options.Texts(OptionsTexts =>
                {
                    OptionsTexts.ConfirmDeleteMessage(Resources.TextConfirmDelete);

                });

            })
            .Pager(Options =>
            {
                Options.AllowedPageSizes(new[] { 30, 50, 100 });
                Options.ShowInfo(true);
                Options.ShowPageSizeSelector(true);
                Options.Visible(true);
            })
            .Paging(Options =>
            {
                Options.Enabled(true);
                Options.PageSize(30);
            })
            .Columns(Columns =>
            {
                if (AllowAdd || AllowUpdate || AllowDelete)
                {
                    var Width = 60;
                    //var Width = 30;
                    Columns.Add().Type(GridCommandColumnType.Buttons).Alignment(HorizontalAlignment.Center).Width(Width).Visible(AllowDelete).Buttons(b =>
                    {
                        b.Add().Name(GridColumnButtonName.Edit).Icon("fas fa-pencil-alt").Text(Resources.TextUpdate);
                        b.Add().Name(GridColumnButtonName.Delete).Icon("fas fa-trash-alt").Text(Resources.TextDelete);
                        b.Add().Name(GridColumnButtonName.Save).Icon("fas fa-check").Text(Resources.TextSave);
                        b.Add().Name(GridColumnButtonName.Cancel).Icon("fas fa-minus-circle").Text(Resources.TextCancel);
                    });
                }
            });
        }

        public TreeListBuilder<T> GetTreeWithStartupValues<T>(IHtmlHelper Html, string KeyFieldName, string ParentFieldName)
        {
            return Html.DevExtreme().TreeList<T>()
            .KeyExpr(KeyFieldName)
            .ParentIdExpr(ParentFieldName)
            .Width("100%")
            .ShowBorders(true)
            .ShowRowLines(true)
            .FocusedRowEnabled(true)
            .FocusedRowIndex(0)
            .AutoExpandAll(true)
            .RootValue(null)
            .Toolbar(Options =>
            {
                Options.Visible(false);
            })
            .Scrolling(Options =>
            {
                Options.Mode(TreeListScrollingMode.Standard);
                Options.ShowScrollbar(ShowScrollbarMode.Always);
            })
            .FilterRow(Options =>
            {
                Options.Visible(true);
                Options.ApplyFilter(GridApplyFilterMode.Auto);
                Options.ShowAllText(Resources.TextAllDevexpressGridFilterRaw);
            })
            .DataSource(Options =>
                Options.RemoteController()
                .LoadUrl(UrlLoad)
                .InsertUrl(UrlAddNew)
                .UpdateUrl(UrlUpdate)
                .DeleteUrl(UrlDelete)
                .Key(KeyFieldName)
            )
            .Editing(Options =>
            {
                //Options.Mode(GridEditMode.Cell);
                Options.Mode(GridEditMode.Row);
                Options.AllowAdding(AllowAdd);
                Options.AllowUpdating(AllowUpdate);
                Options.AllowDeleting(AllowDelete);
                Options.Texts(OptionsTexts =>
                {
                    OptionsTexts.ConfirmDeleteMessage(Resources.TextConfirmDelete);
                });

            })
            .Pager(Options =>
            {
                Options.AllowedPageSizes(new[] { 30, 50, 100 });
                Options.ShowInfo(true);
                Options.ShowPageSizeSelector(true);
                Options.Visible(true);
            })
            .Paging(Options =>
            {
                Options.Enabled(true);
                Options.PageSize(30);
            })
            .Columns(Columns =>
            {
                if (AllowAdd || AllowUpdate || AllowDelete)
                {
                    var Width = (AllowDelete && !AllowAdd && !AllowUpdate) ? 60 : 90;
                    Columns.Add().Alignment(HorizontalAlignment.Center).Type(TreeListCommandColumnType.Buttons).Width(Width).Buttons(b =>
                    {
                        b.Add().Name(TreeListColumnButtonName.Add).Icon("fas fa-plus").Text(Resources.TextAdd);
                        b.Add().Name(TreeListColumnButtonName.Edit).Icon("fas fa-pencil-alt").Text(Resources.TextUpdate);
                        b.Add().Name(TreeListColumnButtonName.Delete).Icon("fas fa-trash-alt").Text(Resources.TextDelete);
                        b.Add().Name(TreeListColumnButtonName.Save).Icon("fas fa-check").Text(Resources.TextSave);
                        b.Add().Name(TreeListColumnButtonName.Cancel).Icon("fas fa-minus-circle").Text(Resources.TextCancel);
                    });
                }
            });
        }
        #endregion
    }

    public static class DevExtremeBuilderCustomExtensions
    {
        #region Methods
        public static DataGridColumnBuilder<T> InitCheckboxColumn<T>(this DataGridColumnBuilder<T> Column, bool AllowNull = false, bool DefaultValue = false)
        {
            Column.TrueText(Resources.TextYes);
            Column.FalseText(Resources.TextNo);
            if (!AllowNull)
            {
                Column.CalculateCellValue($"function(e){{ var DataField = this.dataField;  var Value = e[DataField]; if ($.isEmptyObject(e)) {{ e[DataField] = {DefaultValue.ToString().ToLower()}; }} else if(Value == null){{e[DataField] = false;}}  return e[DataField]; }}");
            }
            return Column;
        }

        public static DataGridColumnBuilder<T> InitDateColumn<T>(this DataGridColumnBuilder<T> Column, bool FormatDateTime = false)
        {
            if (FormatDateTime)
            {
                Column.Format(Constants.Formats.DateTime);
            }
            else
            {
                Column.Format(Constants.Formats.Date);
            }
            return Column;
        }

        public static DataGridColumnBuilder<T1> InitLookupColumn<T1, T2, T3>(this DataGridColumnBuilder<T1> Column, IEnumerable<SimpleKeyValue<T2, T3>> Data, bool IsRequired = false, bool AllowNull = false)
        {
            Column.Lookup(Options =>
            {
                Options.DataSource(d => d.Array().Data(Data).Key(nameof(SimpleKeyValue<T2, T3>.Key))).ValueExpr(nameof(SimpleKeyValue<T2, T3>.Key)).DisplayExpr(nameof(SimpleKeyValue<T2, T3>.Value));
                Options.AllowClearing(AllowNull);
            });

            if (IsRequired)
            {
                Column.ValidationRules(Options =>
                {
                    Options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                });
            }
            return Column;
        }

        public static DataGridColumnBuilder<T> InitNumberColumn<T>(this DataGridColumnBuilder<T> Column, NumberColumnFormatType Format = NumberColumnFormatType.Default)
        {
            switch (Format)
            {
                case NumberColumnFormatType.Money:
                    {
                        Column.Format(Options =>
                        {
                            Options.Type(DevExtreme.AspNet.Mvc.Format.FixedPoint);
                            Options.Precision(2);
                        });
                        break;
                    }
                case NumberColumnFormatType.Quantity:
                    {
                        Column.Format(Options =>
                        {
                            Options.Type(DevExtreme.AspNet.Mvc.Format.FixedPoint);
                            Options.Precision(0);
                        });
                        break;
                    }
            }
            return Column;
        }

        public static DataGridColumnBuilder<T> InitTextboxColumn<T>(this DataGridColumnBuilder<T> Column, bool IsRequired = false, bool ShouldValidateEmailFormat = false, int? MaxLength = null)
        {
            Column.ValidationRules(Options =>
            {
                if (IsRequired)
                {
                    Options.AddRequired().Message(Resources.ValidationRequired).Trim(true);
                }
                if (ShouldValidateEmailFormat)
                {
                    Options.AddEmail().Message(Resources.ValidationEmailFormatInvalid);
                }
                if (MaxLength > 0)
                {
                    Options.AddStringLength().Min(1).Max(MaxLength.Value).Message(string.Format(Resources.ValidationTextMaxLength, MaxLength));
                }
            });
            return Column;
        }

        public static DateBoxBuilder InitDateBox(this DateBoxBuilder DateBox, bool FormatDateTime = false)
        {
            if (FormatDateTime)
            {
                DateBox.DisplayFormat(Constants.Formats.DateTime);
            }
            else
            {
                DateBox.DisplayFormat(Constants.Formats.Date);
            }

            return DateBox;
        }
        
        public static DataGridColumnBuilder<T> InitDetailsUrlCellTemplate<T>(this DataGridColumnBuilder<T> Column,string UrlPropertyName)
        {
            return Column.CellTemplate($"<a href=\"<%-data.{UrlPropertyName}%>\"><i class=\"fas fa-info-circle\"></i></a>");
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