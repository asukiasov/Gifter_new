using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    internal static class SqlParameterConverter
    {
        #region Methods
        static SqlParameter GetSqlParameter(object parameter, string parameterName, SqlDbType sqlDbType, bool isOutput)
        {
            var parameterValue = parameter == null ? DBNull.Value : parameter;
            var sqlParameter = new SqlParameter(parameterName, parameterValue);

            sqlParameter.SqlDbType = sqlDbType;

            if (sqlDbType is SqlDbType.VarChar or SqlDbType.NVarChar)
            {
                var length = parameter?.ToString().Length;
                sqlParameter.Size = length > 0 ? length.Value : 3000;
            }            

            if (isOutput)
            {
                sqlParameter.Direction = ParameterDirection.InputOutput;
            }

            return sqlParameter;
        }

        public static SqlParameter ToSqlParameter(this object parameter, string parameterName, SqlDbType sqlDbType)
        {
            return GetSqlParameter(parameter: parameter, parameterName: parameterName, sqlDbType: sqlDbType, isOutput: false);
        }

        public static SqlParameter ToSqlParameterOutput(this object parameter, string parameterName, SqlDbType sqlDbType)
        {
            return GetSqlParameter(parameter: parameter, parameterName: parameterName, sqlDbType: sqlDbType, isOutput: true);
        }
        #endregion
    }
}
