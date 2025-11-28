using DevExtreme.AspNet.Mvc;
using DevExtreme.AspNet.Mvc.Builders;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System.Collections.Generic;

namespace SixtyThreeBits.Web.Domain.Libraries.DevExtreme
{          
    public static class DevExtremeExtensions63
    {
        #region Methods
        public static DataGridColumnBuilder<T> InitCheckboxColumn<T>(this DataGridColumnBuilder<T> column, bool allowNull = false, bool defaultValue = false)
        {
            column.DataType(GridColumnDataType.Boolean);
            column.TrueText(Resources.TextYes);
            column.FalseText(Resources.TextNo);
            if (!allowNull)
            {
                column.CalculateCellValue($"function(e){{ var DataField = this.dataField;  var Value = e[DataField]; if ($.isEmptyObject(e)) {{ e[DataField] = {defaultValue.ToString().ToLower()}; }} else if(Value == null){{e[DataField] = false;}}  return e[DataField]; }}");
            }
            return column;
        }

        public static DataGridColumnBuilder<T> InitDateColumn<T>(this DataGridColumnBuilder<T> column, DateColumnFormat format)
        {
            if (format == DateColumnFormat.DateTime)
            {
                column.DataType(GridColumnDataType.DateTime);
                column.Format(Constants.Formats.DateTime);
            }
            else
            {
                column.DataType(GridColumnDataType.Date);
                column.Format(Constants.Formats.Date);
            }
            return column;
        }

        public static DateBoxBuilder InitDateBox(this DateBoxBuilder dateBox, DateColumnFormat format)
        {            
            if (format == DateColumnFormat.DateTime)
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

        public static DataGridColumnBuilder<T> InitNumberColumn<T>(this DataGridColumnBuilder<T> column, NumberColumnFormat format = NumberColumnFormat.Default)
        {
            switch (format)
            {
                case NumberColumnFormat.Money:
                    {
                        column.Format(options =>
                        {
                            options.Type(Format.FixedPoint);
                            options.Precision(2);
                        });
                        break;
                    }
                case NumberColumnFormat.Quantity:
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

        public static DataGridBuilder<T> SetOnRowUpdatingSendAllColumnsData<T>(this DataGridBuilder<T> grid)
        {
            grid.OnRowUpdating("globals.devexpress.onRowUpdatingSendAllColumnsData");
            return grid;
        }
        #endregion

        #region Enums
        public enum NumberColumnFormat
        {
            Default,
            Money,
            Quantity
        }

        public enum DateColumnFormat
        {
            Date,
            DateTime
        }
        #endregion
    }
}