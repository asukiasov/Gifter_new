 using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.DB
{
    public partial class DBCoreDataContext
    {
        #region Sub Classes        
        public class ScalarFunctionResult<T>
        {
            #region Properties
            public T Value { get; set; }
            #endregion
        }
        #endregion

        #region Functions
        internal virtual DbSet<ScalarFunctionResult<string>> UsersGetSingleUserByEmailAndPasswordResult { get; set; }
        public async Task<string> UsersGetSingleUserByEmailAndPassword(string Email, string Password)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(UsersGetSingleUserByEmailAndPassword),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                Parameters: new object[] { Email, Password }
            );            
            var DBResult = UsersGetSingleUserByEmailAndPasswordResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        } 

        public class PermissionsListPermissionsWithRoleMarkDBItem
        {
            #region Properties
            public int? PermissionID { get; set; }
            public int? PermissionParentID { get; set; }
            public string PermissionCaption { get; set; }
            public int? PermissionSortIndex { get; set; }
            public bool PermissionIsSelected { get; set; } 
            #endregion
        }
        internal virtual DbSet<PermissionsListPermissionsWithRoleMarkDBItem> PermissionsListPermissionsWithRoleMarkResult { get; set; }
        public IQueryable<PermissionsListPermissionsWithRoleMarkDBItem> PermissionsListPermissionsWithRoleMark(int? RoleID)
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(PermissionsListPermissionsWithRoleMark),
              ResultItemType: typeof(PermissionsListPermissionsWithRoleMarkDBItem),
              Parameters: new object[] { RoleID }
            );
            var DBResult = PermissionsListPermissionsWithRoleMarkResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion


        partial void OnModelCreatingPartial(ModelBuilder ModelBuilder)
        {
            ModelBuilder.Entity<ScalarFunctionResult<string>>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<PermissionsListPermissionsWithRoleMarkDBItem>(Entity => { Entity.HasNoKey(); });
        }

        #region Query Preparation
        class PrepareQueryExecution
        {
            #region Properties
            public string SqlQuery { get; set; }
            public SqlParameter[] SqlParameters { get; set; }

            string ParametersString;

            readonly DatabaseObjectTypes DatabaseObjectType;
            readonly string DatabaseObjectName;
            readonly Type ResultType;
            readonly object[] Parameters;
            #endregion

            #region Constructors
            public PrepareQueryExecution(DatabaseObjectTypes DatabaseObjectType, string DatabaseObjectName, Type ResultItemType, params object[] Parameters)
            {
                this.DatabaseObjectType = DatabaseObjectType;
                this.DatabaseObjectName = DatabaseObjectName;
                this.Parameters = Parameters;
                this.ResultType = ResultItemType;

                BuildParameters();

                switch (DatabaseObjectType)
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
            }

            void BuildScalarValuedFunction()
            {
                //FunctionResult<T> - providing object to generic type doesn't play any role. In this case, class is used only for grabbing name of it's Value property.
                SqlQuery = $"SELECT dbo.{DatabaseObjectName}({ParametersString}) as {nameof(ScalarFunctionResult<object>.Value)}";
            }

            void BuildStoredProcedure()
            {
                SqlQuery = $"EXEC dbo.{DatabaseObjectName} {ParametersString}";
            }

            void BuildTableValuedFunction()
            {
                var PropertiesStringBuilder = new StringBuilder();

                var PropertyNames = ResultType.GetProperties().Select(Item => Item.Name);
                var PropertiesString = string.Join(", ", PropertyNames);

                SqlQuery = $"SELECT {PropertiesString} FROM dbo.{DatabaseObjectName}({ParametersString})";                
            }

            void BuildParameters()
            {
                var ParametersStringBuilder = new StringBuilder();
                var ParametersCount = Parameters.Length;
                SqlParameters = Parameters.Select((Parameter, Index) => new SqlParameter($"P{Index}", Parameter)).ToArray();
                ParametersString = string.Join(", ", Enumerable.Range(0, ParametersCount).Select(Item => $"@P{Item}"));                
            }

            #endregion

            #region Enums
            public enum DatabaseObjectTypes
            {
                #region Properties
                STORED_PROCEDURE,
                TABLE_VALUED_FUNCTION,
                SCALAR_VALUED_FUNCTION 
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
