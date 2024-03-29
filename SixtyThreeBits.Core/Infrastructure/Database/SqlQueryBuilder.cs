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
        readonly DbContext _dbContext;
        readonly string _databaseObjectName;        
        readonly SqlParameter[] _sqlParameters;

        Type _resultType;
        string _parametersString;

        List<SqlParameter> _outputParameters;
        bool hasOutputParameters;
        int outputParametersCount = 0;
        int outputParameterToGetIndex = 0;
        #endregion

        #region Constructors        
        public SqlQueryBuilder(DbContext dbContext, string databaseObjectName = null, params SqlParameter[] sqlParameters)
        {
            _dbContext = dbContext;
            _databaseObjectName = databaseObjectName;
            _sqlParameters = sqlParameters;

            buildParameters();
            
            if (sqlParameters?.Any() == true)
            {
                _outputParameters = sqlParameters.Where(item => item.Direction == ParameterDirection.Output || item.Direction == ParameterDirection.InputOutput).ToList();
                outputParametersCount = _outputParameters.Count;
                hasOutputParameters = outputParametersCount > 0;
            }
        }

        public SqlQueryBuilder(DbContext dbContext, params SqlParameter[] sqlParameters)
        {
            _dbContext = dbContext;
            _sqlParameters = sqlParameters;

            buildParameters();

            if (sqlParameters?.Any() == true)
            {
                _outputParameters = sqlParameters.Where(item => item.Direction == ParameterDirection.Output || item.Direction == ParameterDirection.InputOutput).ToList();
                outputParametersCount = _outputParameters.Count;
                hasOutputParameters = outputParametersCount > 0;
            }
        }
        #endregion

        #region Methods
        public async Task<T> ExecuteScalarValuedFunction<T>()
        {
            buildScalarValuedFunctionSqlScript();
            var iQueryableResult = _dbContext.Database.SqlQueryRaw<T>(SqlQuery, _sqlParameters);
            var queryResult = (await iQueryableResult.ToListAsync());
            var result = queryResult.FirstOrDefault();            
            return result;
        }

        public async Task<ExecuteCommandResult> ExecuteStoredProcedure()
        {
            buildStoredProcedureSqlScript();
            var result = new ExecuteCommandResult();
            await _dbContext.Database.ExecuteSqlRawAsync(SqlQuery, _sqlParameters);
            return result;
        }

        public IQueryable<T> ExecuteTableValuedFunction<T>() where T : class
        {
            _resultType = typeof(T);
            buildTableValuedFunctionSqlScript();
            var result = _dbContext.Database.SqlQueryRaw<T>(SqlQuery, _sqlParameters);
            return result;
        }

        public async Task<ExecuteCommandResult> ExecuteSqlScriptCommand(string sqlScriptCommand)
        {
            var result = new ExecuteCommandResult();
            await _dbContext.Database.ExecuteSqlRawAsync(sqlScriptCommand, _sqlParameters);
            return result;
        }

        public async Task<T> ExecuteSqlScriptScalarValued<T>(string sqlScriptScalarValued)
        {
            var iQueryableResult = _dbContext.Database.SqlQueryRaw<T>(sqlScriptScalarValued, _sqlParameters);
            var queryResult = (await iQueryableResult.ToListAsync());
            var result = queryResult.FirstOrDefault();
            return result;            
        }

        public IQueryable<T> ExecuteSqlScriptTableValued<T>(string sqlScriptTableValued)
        {
            _resultType = typeof(T);            
            var result = _dbContext.Database.SqlQueryRaw<T>(sqlScriptTableValued, _sqlParameters);
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

        #region Private Methods
        void buildScalarValuedFunctionSqlScript()
        {
            SqlQuery = $"SELECT dbo.{_databaseObjectName}({_parametersString})";
        }

        void buildStoredProcedureSqlScript()
        {
            SqlQuery = $"EXEC dbo.{_databaseObjectName} {_parametersString}";
        }

        void buildTableValuedFunctionSqlScript()
        {
            var propertyNames = _resultType.GetProperties().Select(item => item.Name);
            var propertiesString = string.Join(", ", propertyNames);
            SqlQuery = $"SELECT {propertiesString} FROM dbo.{_databaseObjectName}({_parametersString})";
        }
        
        void buildParameters()
        {
            var parametersStringBuilder = new StringBuilder();

            if (_sqlParameters.Length > 0)
            {
                foreach (var P in _sqlParameters)
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
}