using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace SixtyThreeBits.Core.Libraries.Database
{
    internal static class SqlParameterConverter
    {
        #region Methods
        static SqlParameter GetSqlParameter(object parameter, string parameterName, SqlDbType sqlDbType, bool isOutput)
        {
            var parameterValue = parameter == null ? DBNull.Value :
            (
                parameter.GetType() == typeof(DateTime) ? string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-ddTHH:mm:ss}", parameter) : parameter
            );


            var sqlParameter = new SqlParameter(parameterName, parameterValue);

            sqlParameter.SqlDbType = sqlDbType;

            if (parameter != null && parameter.GetType() == typeof(string))
            {
                if (isOutput)
                {
                    //-1 stands for MAX, for example varchar(max) or nvarchar(max)
                    sqlParameter.Size = -1;
                }
                else
                {
                    sqlParameter.Size = (parameter as string)?.Length ?? 3000;
                }
            }

            if (isOutput)
            {
                sqlParameter.Direction = ParameterDirection.InputOutput;
            }

            return sqlParameter;
        }

        public static SqlParameter ToSqlParameter(this object parameter, SqlDbType sqlDbType, [CallerArgumentExpression(nameof(parameter))] string parameterName = null)
        {
            return GetSqlParameter(parameter: parameter, parameterName: parameterName, sqlDbType: sqlDbType, isOutput: false);
        }
       
        public static SqlParameter ToSqlParameterOutput(this object parameter, SqlDbType sqlDbType, [CallerArgumentExpression(nameof(parameter))] string parameterName = null)
        {
            return GetSqlParameter(parameter: parameter, parameterName: parameterName, sqlDbType: sqlDbType, isOutput: true);
        }
        #endregion
    }
}
