using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Infrastructure.Database
{
    internal class SqlQueryBuilder
    {
        #region Properties
        public string SqlQuery { get; set; }
        public SqlParameter[] SqlParameters { get; set; }

        string _parametersString;

        readonly DbContext _dbContext;
        readonly DatabaseObjectTypes _databaseObjectType;
        readonly string _databaseObjectName;
        readonly Type _resultType;

        List<SqlParameter> _outputParameters;
        bool hasOutputParameters;
        int outputParametersCount = 0;
        int outputParameterToGetIndex = 0;
        #endregion

        #region Constructors        
        public SqlQueryBuilder(DbContext dbContext, DatabaseObjectTypes databaseObjectType, string databaseObjectName, Type itemType, params SqlParameter[] sqlParameters)
        {
            _dbContext = dbContext;
            _databaseObjectType = databaseObjectType;
            _databaseObjectName = databaseObjectName;
            _resultType = itemType;
            SqlParameters = sqlParameters;

            BuildParameters();

            switch (databaseObjectType)
            {
                case DatabaseObjectTypes.SCALAR_VALUED_FUNCTION:
                    {
                        BuildScalarValuedFunction();
                        break;
                    }
                case DatabaseObjectTypes.STORED_PROCEDURE:
                    {
                        BuildStoredProcedure();
                        break;
                    }
                case DatabaseObjectTypes.TABLE_VALUED_FUNCTION:
                    {
                        BuildTableValuedFunction();
                        break;
                    }
            }

            if (sqlParameters != null)
            {
                _outputParameters = sqlParameters.Where(item => item.Direction == ParameterDirection.Output || item.Direction == ParameterDirection.InputOutput).ToList();
                outputParametersCount = _outputParameters.Count;
                hasOutputParameters = outputParametersCount > 0;
            }
        }

        void BuildScalarValuedFunction()
        {
            //FunctionResult<T> - providing object to generic type doesn't play any role. In this case, class is used only for grabbing name of it's Value property.
            SqlQuery = $"SELECT dbo.{_databaseObjectName}({_parametersString}) as {nameof(DBQueriesDataContext.ScalarFunctionResultEntity<object>.Value)}";
        }

        void BuildStoredProcedure()
        {
            SqlQuery = $"EXEC dbo.{_databaseObjectName} {_parametersString}";
        }

        void BuildTableValuedFunction()
        {
            var propertiesStringBuilder = new StringBuilder();

            var propertyNames = _resultType.GetProperties().Select(item => item.Name);
            var propertiesString = string.Join(", ", propertyNames);

            SqlQuery = $"SELECT {propertiesString} FROM dbo.{_databaseObjectName}({_parametersString})";
        }

        void BuildParameters()
        {
            var parametersStringBuilder = new StringBuilder();

            if (SqlParameters.Length > 0)
            {
                foreach (var P in SqlParameters)
                {
                    parametersStringBuilder.Append($", @{P.ParameterName}");
                    if (P.Direction == ParameterDirection.InputOutput)
                    {
                        parametersStringBuilder.Append(" OUTPUT");
                    }
                }
                parametersStringBuilder.Remove(0, 2);
            }
            _parametersString = parametersStringBuilder.ToString();
        }

        #endregion

        #region Methods
        public IQueryable<T> ExecuteQuery<T>() where T : class
        {
            var result = _dbContext.Database.SqlQueryRaw<T>(SqlQuery, SqlParameters);
            return result;
        }

        public async Task<T> ExecuteQueryScalar<T>()
        {
            var queryResult = ExecuteQuery<DBQueriesDataContext.ScalarFunctionResultEntity<T>>();
            var firstOrDefault = await queryResult.FirstOrDefaultAsync();
            var result = firstOrDefault.Value;
            return result;
        }

        public async Task<ExecuteCommandResult> ExecuteCommand()
        {
            var result = new ExecuteCommandResult();
            await _dbContext.Database.ExecuteSqlRawAsync(SqlQuery, SqlParameters);
            return result;
        }

        public T GetNextOutputParameterValue<T>()
        {
            var result = default(T);
            if (hasOutputParameters)
            {
                if (outputParameterToGetIndex < outputParametersCount)
                {
                    var value = _outputParameters[outputParameterToGetIndex].Value;
                    result = (T)value;
                }
            }
            return result;
        }
        #endregion

        #region Nested Classes        
        public record ExecuteCommandResult
        {
            #region Properties
            public List<object> OutputParameters { get; set; }
            public bool HasOutputParameters => OutputParameters?.Any() == true;
            #endregion
        }
        #endregion
    }

    #region Enums
    internal enum DatabaseObjectTypes
    {
        #region Properties
        STORED_PROCEDURE,
        TABLE_VALUED_FUNCTION,
        SCALAR_VALUED_FUNCTION
        #endregion
    }
    #endregion
}
