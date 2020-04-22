 using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
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
        #region PagesGetSingleByID
        internal virtual DbSet<ScalarFunctionResult<string>> PagesGetSingleByIDResult { get; set; }
        public async Task<string> PagesGetSingleByID(int? PageID, bool? PageIsPublished)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(PagesGetSingleByID),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    PageID.ToSqlParameter(nameof(PageID), SqlDbType.Int),
                    PageIsPublished.ToSqlParameter(nameof(PageIsPublished), SqlDbType.Bit)
                }
            );
            var DBResult = PagesGetSingleByIDResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

        #region PagesGetSingleBySlug
        internal virtual DbSet<ScalarFunctionResult<string>> PagesGetSingleBySlugResult { get; set; }
        public async Task<string> PagesGetSingleBySlug(string PageSlug, bool? PageIsPublished)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(PagesGetSingleBySlug),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    PageSlug.ToSqlParameter(nameof(PageSlug), SqlDbType.NVarChar),
                    PageIsPublished.ToSqlParameter(nameof(PageIsPublished), SqlDbType.Bit)
                }
            );
            var DBResult = PagesGetSingleBySlugResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

        #region PagesIsSlugUniq
        internal virtual DbSet<ScalarFunctionResult<bool>> PagesIsSlugUniqResult { get; set; }
        public async Task<bool> PagesIsSlugUniq(string PageSlug, int? PageID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(PagesIsSlugUniq),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    PageSlug.ToSqlParameter(nameof(PageSlug), SqlDbType.NVarChar),
                    PageID.ToSqlParameter(nameof(PageID), SqlDbType.Int)
                }
            );
            var DBResult = PagesIsSlugUniqResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value == true;
        }
        #endregion

        #region PagesListForDeleteRecursive        
        public class PagesListForDeleteRecursiveResultItem
        {
            #region Properties
            public int? PageID { get; set; }
            
            #endregion
        }
        internal virtual DbSet<PagesListForDeleteRecursiveResultItem> PagesListForDeleteRecursiveResult { get; set; }
        public IQueryable<PagesListForDeleteRecursiveResultItem> PagesListForDeleteRecursive(int? PageID)
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(PagesListForDeleteRecursive),
              ResultItemType: typeof(PagesListForDeleteRecursiveResultItem),
              SqlParameters: new SqlParameter[]
              {
                  PageID.ToSqlParameter(nameof(PageID), SqlDbType.Int),                  
              }
            );
            var DBResult = PagesListForDeleteRecursiveResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region SystemPropertiesGet
        internal virtual DbSet<ScalarFunctionResult<string>> SystemPropertiesGetResult { get; set; }
        public async Task<string> SystemPropertiesGet()
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(SystemPropertiesGet),
                ResultItemType: typeof(ScalarFunctionResult<string>)
            );
            var DBResult = SystemPropertiesGetResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

        #region UsersGetSingleUserByUserID
        internal virtual DbSet<ScalarFunctionResult<string>> UsersGetSingleUserByIDResult { get; set; }
        public async Task<string> UsersGetSingleUserByUserID(int? UserID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(UsersGetSingleUserByUserID),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    UserID.ToSqlParameter(nameof(UserID), SqlDbType.Int)
                }
            );
            var DBResult = UsersGetSingleUserByIDResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion
        
        #region UsersGetSingleUserByEmailAndPassword
        internal virtual DbSet<ScalarFunctionResult<string>> UsersGetSingleUserByEmailAndPasswordResult { get; set; }
        public async Task<string> UsersGetSingleUserByEmailAndPassword(string Email, string Password)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(UsersGetSingleUserByEmailAndPassword),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    Email.ToSqlParameter(nameof(Email), SqlDbType.VarChar),
                    Password.ToSqlParameter(nameof(Password), SqlDbType.NVarChar)
                }
            );
            var DBResult = UsersGetSingleUserByEmailAndPasswordResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

        #region UsersIsEmailUnique
        internal virtual DbSet<ScalarFunctionResult<bool>> UsersIsEmailUniqueResult { get; set; }
        public async Task<bool> UsersIsEmailUnique(string Email, int? UserID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(UsersIsEmailUnique),
                ResultItemType: typeof(ScalarFunctionResult<bool>),
                SqlParameters: new SqlParameter[]
                {
                    Email.ToSqlParameter(nameof(Email), SqlDbType.NVarChar),
                    UserID.ToSqlParameter(nameof(UserID), SqlDbType.Int)
                }
            );
            var DBResult = UsersIsEmailUniqueResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value == true;
        }
        #endregion
        #endregion

        #region Stored Procedures        
        public async Task DictionariesDeleteRecursive(int? DictionaryID)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(DictionariesDeleteRecursive),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 DictionaryID.ToSqlParameter(nameof(DictionaryID),SqlDbType.Int)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
        }

        public async Task PagesDeleteRecursive(int? PageID)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(PagesDeleteRecursive),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 PageID.ToSqlParameter(nameof(PageID),SqlDbType.Int)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
        }

        public async Task PagesSyncParentsAndSortIndexes(string ParentsAndSortIndexesXml)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(PagesSyncParentsAndSortIndexes),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 ParentsAndSortIndexesXml.ToSqlParameter(nameof(ParentsAndSortIndexesXml),SqlDbType.Xml)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
        }

        public async Task PermissionsDeleteRecursive(int? PermissionID)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(PermissionsDeleteRecursive),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 PermissionID.ToSqlParameter(nameof(PermissionID),SqlDbType.Int)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
        }

        public async Task RolePermissionsUpdate(int? RoleID, string PermissionsXml)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(RolePermissionsUpdate),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 RoleID.ToSqlParameter(nameof(RoleID),SqlDbType.TinyInt),
                 PermissionsXml.ToSqlParameter(nameof(PermissionsXml),SqlDbType.Xml)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);            
        }
        
        public async Task<int?> UsersIUD(Enums.DatabaseActions iud, int? UserID, string UserEmail, string UserPassword, string UserFirstname, string UserLastname, int? UserRoleID, DateTime? UserBirthdate, string UserPhoneNumberMobile, string UserPersonalNumber, string UserAvatarFilename, bool? UserIsActive)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(UsersIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                 UserID.ToSqlParameter(nameof(UserID),SqlDbType.Int,true),
                 UserEmail.ToSqlParameter(nameof(UserEmail),SqlDbType.VarChar),
                 UserPassword.ToSqlParameter(nameof(UserPassword),SqlDbType.NVarChar),
                 UserFirstname.ToSqlParameter(nameof(UserFirstname),SqlDbType.NVarChar),
                 UserLastname.ToSqlParameter(nameof(UserLastname),SqlDbType.NVarChar),
                 UserRoleID.ToSqlParameter(nameof(UserRoleID),SqlDbType.Int),
                 UserBirthdate.ToSqlParameter(nameof(UserBirthdate),SqlDbType.Date),
                 UserPhoneNumberMobile.ToSqlParameter(nameof(UserPhoneNumberMobile),SqlDbType.VarChar),
                 UserPersonalNumber.ToSqlParameter(nameof(UserPersonalNumber),SqlDbType.VarChar),
                 UserAvatarFilename.ToSqlParameter(nameof(UserAvatarFilename),SqlDbType.NVarChar),
                 UserIsActive.ToSqlParameter(nameof(UserIsActive),SqlDbType.Bit)
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);

            UserID = PR.SqlParameters[1].Value?.ToString().ToInt();

            return UserID;
        }
        #endregion

        partial void OnModelCreatingPartial(ModelBuilder ModelBuilder)
        {
            ModelBuilder.Entity<ScalarFunctionResult<string>>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<ScalarFunctionResult<bool>>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<PagesListForDeleteRecursiveResultItem>(Entity => { Entity.HasNoKey(); });       
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
            #endregion

            #region Constructors
            public PrepareQueryExecution(DatabaseObjectTypes DatabaseObjectType, string DatabaseObjectName, Type ResultItemType, params SqlParameter[] SqlParameters)
            {
                this.DatabaseObjectType = DatabaseObjectType;
                this.DatabaseObjectName = DatabaseObjectName;
                this.SqlParameters = SqlParameters;
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

                if (SqlParameters.Length > 0)
                {
                    foreach(var P in SqlParameters)
                    {
                        ParametersStringBuilder.Append($", @{P.ParameterName}");
                        if(P.Direction == System.Data.ParameterDirection.InputOutput)
                        {
                            ParametersStringBuilder.Append(" OUTPUT");
                        }
                    }
                    ParametersStringBuilder.Remove(0, 2);
                }
                ParametersString = ParametersStringBuilder.ToString();
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

    static class SqlParameterConverter
    {
        #region Methods
        public static SqlParameter ToSqlParameter(this object Parameter, string ParameterName,SqlDbType SqlDbType,bool IsOutput = false)
        {            
            var ParameterValue = Parameter == null ? DBNull.Value : Parameter;
            var P = new SqlParameter(ParameterName, ParameterValue);
            
            P.SqlDbType = SqlDbType;

            if (Parameter != null && Parameter.GetType() == typeof(string))
            {
                P.Size = (Parameter as string).Length;
            }

            if(IsOutput)
            {
                P.Direction = ParameterDirection.InputOutput;
            }

            return P;
        }        
        #endregion
    }
}
