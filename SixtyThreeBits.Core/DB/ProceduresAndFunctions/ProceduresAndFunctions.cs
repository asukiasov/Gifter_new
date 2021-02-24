using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Data;
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
        #region BlogsList
        public class BlogsListResultItem
        {
            #region Properties
            public int? BlogID { get; set; }
            public string BlogTitle { get; set; }
            public string BlogAuthorName { get; set; }
            public DateTime? BlogDate { get; set; }
            #endregion
        }
        internal virtual DbSet<BlogsListResultItem> BlogsListResult { get; set; }
        public IQueryable<BlogsListResultItem> BlogList()
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(BlogList),
              ResultItemType: typeof(BlogsListResultItem)
            );
            var DBResult = BlogsListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region BlogsGetSingleByID
        internal virtual DbSet<ScalarFunctionResult<string>> BlogsGetSingleByIDResult { get; set; }
        public async Task<string> BlogGetSingleByID(int? BlogsID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(BlogGetSingleByID),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    BlogsID.ToSqlParameter(nameof(BlogsID), SqlDbType.Int),
                }
            );
            var DBResult = BlogsGetSingleByIDResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

        #region BlogsIsSlugUniq
        internal virtual DbSet<ScalarFunctionResult<bool>> BlogsIsSlugUniqResult { get; set; }
        public async Task<bool> BlogIsSlugUniq(string Blogslug, int? BlogsID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(BlogIsSlugUniq),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    Blogslug.ToSqlParameter(nameof(Blogslug), SqlDbType.NVarChar),
                    BlogsID.ToSqlParameter(nameof(BlogsID), SqlDbType.Int)
                }
            );
            var DBResult = BlogsIsSlugUniqResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value == true;
        }
        #endregion

        #region DictionariesList
        public class DictionariesListResultItem
        {
            #region Properties
            public int? DictionaryID { get; set; }
            public string DictionaryCaption { get; set; }
            public string DictionaryCaptionEng { get; set; }
            public string DictionaryCaptionRus { get; set; }
            public int? DictionaryParentID { get; set; }
            public int? DictionaryLevel { get; set; }
            public string DictionaryStringCode { get; set; }
            public int? DictionaryIntCode { get; set; }
            public decimal? DictionaryDecimalValue { get; set; }
            public int? DictionaryCode { get; set; }
            public bool DictionaryIsDefault { get; set; }
            public bool DictionaryIsVisible { get; set; }
            public int? DictionarySortIndex { get; set; }
            public DateTime? DictionaryDateCreated { get; set; }
            #endregion            
        }
        internal virtual DbSet<DictionariesListResultItem> DictionariesListResult { get; set; }
        public IQueryable<DictionariesListResultItem> DictionariesList(int? DictionaryLevel, int? DictionaryCode, bool? DictionaryIsVisible)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                DatabaseObjectName: nameof(DictionariesList),
                ResultItemType: typeof(DictionariesListResultItem),
                SqlParameters: new SqlParameter[]
                {
                    DictionaryLevel.ToSqlParameter(nameof(DictionaryLevel), SqlDbType.Int),
                    DictionaryCode.ToSqlParameter(nameof(DictionaryCode), SqlDbType.Int),
                    DictionaryIsVisible.ToSqlParameter(nameof(DictionaryIsVisible), SqlDbType.Bit)
                }
            );
            var DBResult = DictionariesListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region NewsList
        public class NewsListResultItem
        {
            #region Properties
            public int? NewsID { get; set; }
            public string NewsSlug { get; set; }
            public string NewsTitle { get; set; }
            public string NewsTitleEng { get; set; }
            public string NewsTitleRus { get; set; }
            public string NewsText { get; set; }
            public string NewsTextEng { get; set; }
            public string NewsTextRus { get; set; }
            public string NewsShortDescription { get; set; }
            public string NewsShortDescriptionEng { get; set; }
            public string NewsShortDescriptionRus { get; set; }
            public string NewsImageFilename { get; set; }
            public DateTime? NewsDatePublished { get; set; }
            public bool NewsIsPublished { get; set; }
            public DateTime? NewsDateCreated { get; set; }
            #endregion
        }
        internal virtual DbSet<NewsListResultItem> NewsListResult { get; set; }
        public IQueryable<NewsListResultItem> NewsList()
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(NewsList),
              ResultItemType: typeof(NewsListResultItem)
            );
            var DBResult = NewsListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region NewsGetSingleByID
        internal virtual DbSet<ScalarFunctionResult<string>> NewsGetSingleByIDResult { get; set; }
        public async Task<string> NewsGetSingleByID(int? NewsID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(NewsGetSingleByID),
                ResultItemType: typeof(ScalarFunctionResult<string>),
                SqlParameters: new SqlParameter[]
                {
                    NewsID.ToSqlParameter(nameof(NewsID), SqlDbType.Int)
                }
            );
            var DBResult = NewsGetSingleByIDResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            var DBFunctionResult = await DBResult.FirstOrDefaultAsync();
            return DBFunctionResult?.Value;
        }
        #endregion

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

        #region PagesList
        public class PagesListResultItem
        {
            #region Properties
            public int? PageID { get; set; }
            public int? PageParentID { get; set; }
            public string PageSlug { get; set; }
            public string PageTitle { get; set; }
            public string PageTitleEng { get; set; }
            public string PageTitleRus { get; set; }
            public string PageShortDescription { get; set; }
            public string PageShortDescriptionEng { get; set; }
            public string PageShortDescriptionRus { get; set; }
            public string PageImageFilename { get; set; }
            public int? PageCode { get; set; }
            public bool PageIsPublished { get; set; }
            public int? PageSortIndex { get; set; }
            public bool PageIsMenuItem { get; set; }
            public bool PageIsFooterItem { get; set; }
            public bool PageIsExternalUrl { get; set; }
            public string PageExternalUrl { get; set; }
            public DateTime? PageDateCreated { get; set; }
            #endregion
        }
        internal virtual DbSet<PagesListResultItem> PagesListResult { get; set; }
        public IQueryable<PagesListResultItem> PagesList(bool? PageIsPublished, bool? PageIsMenuItem)
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(PagesList),
              ResultItemType: typeof(PagesListResultItem),
              SqlParameters: new SqlParameter[]
              {
                  PageIsPublished.ToSqlParameter(nameof(PageIsPublished), SqlDbType.Bit),
                  PageIsMenuItem.ToSqlParameter(nameof(PageIsMenuItem), SqlDbType.Bit)
              }
            );
            var DBResult = PagesListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
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

        #region PermissionsList
        public class PermissionsListResultItem
        {
            #region Properties
            public int? PermissionID { get; set; }
            public int? PermissionParentID { get; set; }
            public string PermissionCaption { get; set; }
            public string PermissionPagePath { get; set; }
            public string PermissionCodeName { get; set; }
            public string PermissionCode { get; set; }
            public bool PermissionIsMenuItem { get; set; }
            public string PermissionMenuIcon { get; set; }
            public int? PermissionSortIndex { get; set; }
            public DateTime? PermissionDateCreated { get; set; }
            #endregion
        }
        internal virtual DbSet<PermissionsListResultItem> PermissionsListResult { get; set; }
        public IQueryable<PermissionsListResultItem> PermissionsList()
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(PermissionsList),
              ResultItemType: typeof(PermissionsListResultItem)
            );
            var DBResult = PermissionsListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region RolesList
        public class RolesListResultItem
        {
            #region Properties
            public int? RoleID { get; set; }
            public string RoleName { get; set; }
            public int? RoleCode { get; set; }
            public DateTime? RoleDateCreated { get; set; }
            #endregion
        }
        internal virtual DbSet<RolesListResultItem> RolesListResult { get; set; }
        public IQueryable<RolesListResultItem> RolesList()
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(RolesList),
              ResultItemType: typeof(RolesListResultItem)
            );
            var DBResult = RolesListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region RolePermissionsList
        public class RolePermissionsListResultItem
        {
            #region Properties
            public int? PermissionID { get; set; }
            #endregion
        }
        internal virtual DbSet<RolePermissionsListResultItem> RolePermissionsListResult { get; set; }
        public IQueryable<RolePermissionsListResultItem> RolePermissionsList(int? RoleID)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
                DatabaseObjectName: nameof(RolePermissionsList),
                ResultItemType: typeof(RolePermissionsListResultItem),
                SqlParameters: new SqlParameter[]
                {
                    RoleID.ToSqlParameter(nameof(RoleID), SqlDbType.Int)
                }
            );
            var DBResult = RolePermissionsListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
        }
        #endregion

        #region SystemPropertiesGet
        internal virtual DbSet<ScalarFunctionResult<string>> SystemPropertiesGetResult { get; set; }
        public Task<ScalarFunctionResult<string>> SystemPropertiesGet()
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.SCALAR_VALUED_FUNCTION,
                DatabaseObjectName: nameof(SystemPropertiesGet),
                ResultItemType: typeof(ScalarFunctionResult<string>)
            );
            var DBResult = SystemPropertiesGetResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult.FirstOrDefaultAsync();            
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

        #region UsersList
        public class UsersListResultItem
        {
            #region Properties
            public int? UserID { get; set; }
            public string UserEmail { get; set; }
            public string UserPassword { get; set; }
            public string UserFirstname { get; set; }
            public string UserLastname { get; set; }
            public string UserFullname { get; set; }
            public int? UserRoleID { get; set; }
            public DateTime? UserBirthdate { get; set; }
            public string UserPhoneNumberMobile { get; set; }
            public string UserPersonalNumber { get; set; }
            public string UserAvatarFilename { get; set; }
            public bool UserIsActive { get; set; }
            public DateTime? UserDateCreated { get; set; }
            #endregion
        }
        internal virtual DbSet<UsersListResultItem> UsersListResult { get; set; }
        public IQueryable<UsersListResultItem> UsersList()
        {
            var PR = new PrepareQueryExecution(
              DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.TABLE_VALUED_FUNCTION,
              DatabaseObjectName: nameof(UsersList),
              ResultItemType: typeof(UsersListResultItem)
            );
            var DBResult = UsersListResult.FromSqlRaw(PR.SqlQuery, PR.SqlParameters).AsNoTracking();
            return DBResult;
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
        public async Task<int?> BlogIUD(Enums.DatabaseActions iud, int? BlogsID, string Blogslug, string BlogsTitle, string BlogsText, string BlogsAuthorName, string BlogsImageFilename, DateTime? BlogsDate)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(BlogIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                 BlogsID.ToSqlParameter(nameof(BlogsID),SqlDbType.Int,true),
                 Blogslug.ToSqlParameter(nameof(Blogslug),SqlDbType.NVarChar),
                 BlogsTitle.ToSqlParameter(nameof(BlogsTitle),SqlDbType.NVarChar),
                 BlogsText.ToSqlParameter(nameof(BlogsText),SqlDbType.NVarChar),
                 BlogsAuthorName.ToSqlParameter(nameof(BlogsAuthorName),SqlDbType.NVarChar),
                 BlogsImageFilename.ToSqlParameter(nameof(BlogsImageFilename),SqlDbType.NVarChar),
                 BlogsDate.ToSqlParameter(nameof(BlogsDate),SqlDbType.Date),
             }
             );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            BlogsID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return BlogsID;
        }

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

        public async Task<int?> DictionariesIUD(Enums.DatabaseActions iud, int? DictionaryID, string DictionaryCaption, string DictionaryCaptionEng, string DictionaryCaptionRus, int? DictionaryParentID, string DictionaryStringCode, int? DictionaryIntCode, decimal? DictionaryDecimalValue, int? DictionaryCode, bool? DictionaryIsDefault, bool? DictionaryIsVisible, int? DictionarySortIndex)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(DictionariesIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                    iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                    DictionaryID.ToSqlParameter(nameof(DictionaryID),SqlDbType.Int, true),
                    DictionaryCaption.ToSqlParameter(nameof(DictionaryCaption),SqlDbType.NVarChar),
                    DictionaryCaptionEng.ToSqlParameter(nameof(DictionaryCaptionEng),SqlDbType.NVarChar),
                    DictionaryCaptionRus.ToSqlParameter(nameof(DictionaryCaptionRus),SqlDbType.NVarChar),
                    DictionaryParentID.ToSqlParameter(nameof(DictionaryParentID),SqlDbType.Int),
                    DictionaryStringCode.ToSqlParameter(nameof(DictionaryStringCode),SqlDbType.NVarChar),
                    DictionaryIntCode.ToSqlParameter(nameof(DictionaryIntCode),SqlDbType.Int),
                    DictionaryDecimalValue.ToSqlParameter(nameof(DictionaryDecimalValue),SqlDbType.Money),
                    DictionaryCode.ToSqlParameter(nameof(DictionaryCode),SqlDbType.Int),
                    DictionaryIsDefault.ToSqlParameter(nameof(DictionaryIsDefault),SqlDbType.Bit),
                    DictionaryIsVisible.ToSqlParameter(nameof(DictionaryIsVisible),SqlDbType.Bit),
                    DictionarySortIndex.ToSqlParameter(nameof(DictionarySortIndex),SqlDbType.Int),
             }
           );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            DictionaryID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return DictionaryID;
        }

        public async Task<int?> NewsIUD(Enums.DatabaseActions iud, int? NewsID, string NewsSlug, string NewsTitle, string NewsTitleEng, string NewsTitleRus, string NewsText, string NewsTextEng, string NewsTextRus, string NewsShortDescription, string NewsShortDescriptionEng, string NewsShortDescriptionRus, string NewsImageFilename, DateTime? NewsDatePublished, bool NewsIsPublished, DateTime? NewsDateCreated)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(NewsIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                 NewsID.ToSqlParameter(nameof(NewsID),SqlDbType.Int,true),
                 NewsSlug.ToSqlParameter(nameof(NewsSlug),SqlDbType.NVarChar),
                 NewsTitle.ToSqlParameter(nameof(NewsTitle),SqlDbType.NVarChar),
                 NewsTitleEng.ToSqlParameter(nameof(NewsTitleEng),SqlDbType.NVarChar),
                 NewsTitleRus.ToSqlParameter(nameof(NewsTitleRus),SqlDbType.NVarChar),
                 NewsText.ToSqlParameter(nameof(NewsText),SqlDbType.NVarChar),
                 NewsTextEng.ToSqlParameter(nameof(NewsTextEng),SqlDbType.NVarChar),
                 NewsTextRus.ToSqlParameter(nameof(NewsTextRus),SqlDbType.NVarChar),
                 NewsShortDescription.ToSqlParameter(nameof(NewsShortDescription),SqlDbType.NVarChar),
                 NewsShortDescriptionEng.ToSqlParameter(nameof(NewsShortDescriptionEng),SqlDbType.NVarChar),
                 NewsShortDescriptionRus.ToSqlParameter(nameof(NewsShortDescriptionRus),SqlDbType.NVarChar),
                 NewsImageFilename.ToSqlParameter(nameof(NewsImageFilename),SqlDbType.NVarChar),
                 NewsDatePublished.ToSqlParameter(nameof(NewsDatePublished),SqlDbType.DateTime),
                 NewsIsPublished.ToSqlParameter(nameof(NewsIsPublished),SqlDbType.Bit)
             }
             );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            NewsID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return NewsID;
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

        public async Task<int?> PagesIUD(Enums.DatabaseActions iud, int? PageID, int? PageParentID, string PageSlug, string PageTitle, string PageTitleEng, string PageTitleRus, string PageText, string PageTextEng, string PageTextRus, string PageData, string PageDataEng, string PageDataRus, string PageShortDescription, string PageShortDescriptionEng, string PageShortDescriptionRus, string PageImageFilename, int? PageCode, bool? PageIsPublished, int? PageSortIndex, bool? PageIsMenuItem, bool? PageIsFooterItem, bool? PageIsExternalUrl, string PageExternalUrl)
        {
            var PR = new PrepareQueryExecution(
                DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
                DatabaseObjectName: nameof(PagesIUD),
                ResultItemType: null,
                SqlParameters: new SqlParameter[]
                {
                    iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                    PageID.ToSqlParameter(nameof(PageID),SqlDbType.Int, true),
                    PageParentID.ToSqlParameter(nameof(PageParentID),SqlDbType.Int),
                    PageSlug.ToSqlParameter(nameof(PageSlug),SqlDbType.VarChar),
                    PageTitle.ToSqlParameter(nameof(PageTitle),SqlDbType.NVarChar),
                    PageTitleEng.ToSqlParameter(nameof(PageTitleEng),SqlDbType.NVarChar),
                    PageTitleRus.ToSqlParameter(nameof(PageTitleRus),SqlDbType.NVarChar),
                    PageText.ToSqlParameter(nameof(PageText),SqlDbType.NVarChar),
                    PageTextEng.ToSqlParameter(nameof(PageTextEng),SqlDbType.NVarChar),
                    PageTextRus.ToSqlParameter(nameof(PageTextRus),SqlDbType.NVarChar),
                    PageData.ToSqlParameter(nameof(PageData),SqlDbType.NVarChar),
                    PageDataEng.ToSqlParameter(nameof(PageDataEng),SqlDbType.NVarChar),
                    PageDataRus.ToSqlParameter(nameof(PageDataRus),SqlDbType.NVarChar),
                    PageShortDescription.ToSqlParameter(nameof(PageShortDescription),SqlDbType.NVarChar),
                    PageShortDescriptionEng.ToSqlParameter(nameof(PageShortDescriptionEng),SqlDbType.NVarChar),
                    PageShortDescriptionRus.ToSqlParameter(nameof(PageShortDescriptionRus),SqlDbType.NVarChar),
                    PageImageFilename.ToSqlParameter(nameof(PageImageFilename),SqlDbType.NVarChar),
                    PageCode.ToSqlParameter(nameof(PageCode),SqlDbType.Int),
                    PageIsPublished.ToSqlParameter(nameof(PageIsPublished),SqlDbType.Bit),
                    PageSortIndex.ToSqlParameter(nameof(PageSortIndex),SqlDbType.Int),
                    PageIsMenuItem.ToSqlParameter(nameof(PageIsMenuItem),SqlDbType.Bit),
                    PageIsFooterItem.ToSqlParameter(nameof(PageIsFooterItem),SqlDbType.Bit),
                    PageIsExternalUrl.ToSqlParameter(nameof(PageIsExternalUrl),SqlDbType.Bit),
                    PageExternalUrl.ToSqlParameter(nameof(PageExternalUrl),SqlDbType.NVarChar)

                }
            );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            PageID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return PageID;
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

        public async Task<int?> PermissionsIUD(Enums.DatabaseActions iud, int? PermissionID, int? PermissionParentID, string PermissionCaption, string PermissionPagePath, string PermissionCodeName, string PermissionCode, bool? PermissionIsMenuItem, string PermissionMenuIcon, int? PermissionSortIndex)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(PermissionsIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                 PermissionID.ToSqlParameter(nameof(PermissionID),SqlDbType.Int,true),
                 PermissionParentID.ToSqlParameter(nameof(PermissionParentID),SqlDbType.Int),
                 PermissionCaption.ToSqlParameter(nameof(PermissionCaption),SqlDbType.NVarChar),
                 PermissionPagePath.ToSqlParameter(nameof(PermissionPagePath),SqlDbType.NVarChar),
                 PermissionCodeName.ToSqlParameter(nameof(PermissionCodeName),SqlDbType.NVarChar),
                 PermissionCode.ToSqlParameter(nameof(PermissionCode),SqlDbType.VarChar),
                 PermissionIsMenuItem.ToSqlParameter(nameof(PermissionIsMenuItem),SqlDbType.Bit),
                 PermissionMenuIcon.ToSqlParameter(nameof(PermissionMenuIcon),SqlDbType.NVarChar),
                 PermissionSortIndex.ToSqlParameter(nameof(PermissionSortIndex),SqlDbType.Int)
             }
             );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            PermissionID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return PermissionID;
        }

        public async Task<int?> RolesIUD(Enums.DatabaseActions iud, int? RoleID, string RoleName, int? RoleCode)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(RolesIUD),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 iud.ToSqlParameter(nameof(iud),SqlDbType.TinyInt),
                 RoleID.ToSqlParameter(nameof(RoleID),SqlDbType.Int,true),
                 RoleName.ToSqlParameter(nameof(RoleName),SqlDbType.NVarChar),
                 RoleCode.ToSqlParameter(nameof(RoleCode),SqlDbType.Int),
             }
             );

            var DBResult = await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
            RoleID = PR.SqlParameters[1].Value?.ToString().ToInt();
            return RoleID;
        }

        public async Task RolePermissionsUpdate(int? RoleID, string PermissionsXml)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(RolePermissionsUpdate),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 RoleID.ToSqlParameter(nameof(RoleID),SqlDbType.Int),
                 PermissionsXml.ToSqlParameter(nameof(PermissionsXml),SqlDbType.Xml)
             }
           );
            await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
        }

        public async Task SystemPropertiesUpdate(string ContactEmail, string ContactPhone, string ContactAddress, string FacebookUrl, string InstagramUrl, string TwitterUrl, string YoutubeUrl, string LinkedInUrl, string GoogleMapsIFrame, string FooterScripts, string SMTPAddress, int? SMTPPort, string SMTPUsername, string SMTPPassword, bool SMTPUseSSL, string SMTPFrom)
        {
            var PR = new PrepareQueryExecution(
             DatabaseObjectType: PrepareQueryExecution.DatabaseObjectTypes.STORED_PROCEDURE,
             DatabaseObjectName: nameof(SystemPropertiesUpdate),
             ResultItemType: null,
             SqlParameters: new SqlParameter[]
             {
                 ContactEmail.ToSqlParameter(nameof(ContactEmail),SqlDbType.NVarChar),
                 ContactPhone.ToSqlParameter(nameof(ContactPhone),SqlDbType.NVarChar),
                 ContactAddress.ToSqlParameter(nameof(ContactAddress),SqlDbType.NVarChar),
                 FacebookUrl.ToSqlParameter(nameof(FacebookUrl),SqlDbType.NVarChar),
                 InstagramUrl.ToSqlParameter(nameof(InstagramUrl),SqlDbType.NVarChar),
                 TwitterUrl.ToSqlParameter(nameof(TwitterUrl),SqlDbType.NVarChar),
                 YoutubeUrl.ToSqlParameter(nameof(YoutubeUrl),SqlDbType.NVarChar),
                 LinkedInUrl.ToSqlParameter(nameof(LinkedInUrl),SqlDbType.NVarChar),
                 GoogleMapsIFrame.ToSqlParameter(nameof(GoogleMapsIFrame),SqlDbType.NVarChar),
                 FooterScripts.ToSqlParameter(nameof(FooterScripts),SqlDbType.NVarChar),
                 SMTPAddress.ToSqlParameter(nameof(SMTPAddress),SqlDbType.NVarChar),
                 SMTPPort.ToSqlParameter(nameof(SMTPPort),SqlDbType.Int),
                 SMTPUsername.ToSqlParameter(nameof(SMTPUsername),SqlDbType.NVarChar),
                 SMTPPassword.ToSqlParameter(nameof(SMTPPassword),SqlDbType.NVarChar),
                 SMTPUseSSL.ToSqlParameter(nameof(SMTPUseSSL),SqlDbType.Bit),
                 SMTPFrom.ToSqlParameter(nameof(SMTPFrom),SqlDbType.NVarChar),                 
             }
           );
            await Database.ExecuteSqlRawAsync(PR.SqlQuery, PR.SqlParameters);
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
            ModelBuilder.Entity<DictionariesListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<BlogsListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<NewsListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<ScalarFunctionResult<string>>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<ScalarFunctionResult<bool>>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<PagesListForDeleteRecursiveResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<PagesListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<PermissionsListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<RolesListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<RolePermissionsListResultItem>(Entity => { Entity.HasNoKey(); });
            ModelBuilder.Entity<UsersListResultItem>(Entity => { Entity.HasNoKey(); });
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
                    foreach (var P in SqlParameters)
                    {
                        ParametersStringBuilder.Append($", @{P.ParameterName}");
                        if (P.Direction == System.Data.ParameterDirection.InputOutput)
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
        public static SqlParameter ToSqlParameter(this object Parameter, string ParameterName, SqlDbType SqlDbType, bool IsOutput = false)
        {
            var ParameterValue = Parameter == null ? DBNull.Value : Parameter;
            var P = new SqlParameter(ParameterName, ParameterValue);

            P.SqlDbType = SqlDbType;

            if (Parameter != null && Parameter.GetType() == typeof(string))
            {
                P.Size = (Parameter as string).Length;
            }

            if (IsOutput)
            {
                P.Direction = ParameterDirection.InputOutput;
            }

            return P;
        }
        #endregion
    }
}
