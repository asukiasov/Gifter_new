namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ControllerActionRouteNames
    {
        public static class Admin
        {
            #region Nested Classes
            public static class AuthController
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";
                public const string Relogin = "AdminAuthRelogin";
                #endregion
            }

            public static class BlogPostsController
            {
                #region Properties
                public const string BlogPosts = "AdminBlogPostsControllerBlogPosts";
                public const string Grid = "AdminBlogPostsControllerGrid";
                public const string GridAdd = "AdminBlogPostsControllerGridAdd";
                public const string GridUpdate = "AdminBlogPostsControllerGridUpdate";
                public const string GridDelete = "AdminBlogPostsControllerGridDelete";                
                #endregion
            }

            public static class BlogPostPropertiesController
            {
                #region properties
                public const string Properties = "AdminBlogPostPropertiesControllerProperties";
                public const string DeleteImage = "AdminBlogPostPropertiesControllerDeleteImage";
                #endregion
            }

            public static class BrandsController
            {
                #region Properties
                public const string Brands = "AdminBrandsControllerBrands";
                public const string Grid = "AdminBrandsControllerGrid";
                public const string GridAdd = "AdminBrandsControllerGridAdd";
                public const string GridUpdate = "AdminBrandsControllerGridUpdate";
                public const string GridDelete = "AdminBrandsControllerGridDelete";
                #endregion
            }

            public static class BrandPropertiesController
            {
                #region Properties                
                public const string Properties = "AdminBrandPropertiesControllerProperties";
                public const string DeleteImage = "AdminBrandPropertiesControllerDeleteImage";
                #endregion
            }

            public static class ChangeLanguageController
            {
                #region Properties
                public const string ChangeLanguage = "AdminChangeLanguageControllerChangeLanguage";
                #endregion
            }

            public static class DictionariesController
            {
                #region Properties
                public const string Dictionaries = "AdminDictionariesControllerDictionaries";
                public const string Tree = "AdminDictionariesControllerTree";
                public const string TreeAdd = "AdminDictionariesControllerTreeAdd";
                public const string TreeUpdate = "AdminDictionariesControllerTreeUpdate";
                public const string TreeDelete = "AdminDictionariesControllerTreeDelete";
                #endregion
            }

            public static class EmailTemplatesController
            {
                #region Properties
                public const string EmailTemplates = "AdminEmailTemplatesControllerEmailTemplates";
                public const string Grid = "AdminEmailTemplatesControllerGrid";
                #endregion                
            }

            public static class EmailTemplatePropertiesController
            {
                #region Properties
                public const string Properties = "AdminEmailTemplateControllerProperties";
                #endregion
            }

            public static class FileManagerController
            {
                #region Properties
                public const string FileManager = "AdminFileManagerControllerFileManager";
                public const string Files = "AdminFileManagerControllerFiles";
                public const string Upload = "AdminFileManagerControllerUpload";
                public const string Delete = "AdminFileManagerControllerDelete";
                #endregion
            }

            public static class HomeController
            {
                #region Properties
                public const string Index = "AdminHomeControllerIndex";
                #endregion
            }

            public class MenuFooterController
            {
                #region Properties
                public const string MenuFooter = "AdminMenuFooterControllerMenuFooter";
                public const string Add = "AdminMenuFooterControllerAdd";
                public const string Update = "AdminMenuFooterControllerUpdate";
                public const string Delete = "AdminMenuFooterControllerDelete";
                public const string Sort = "AdminMenuFooterControllerSort";
                public const string Get = "AdminMenuFooterControllerGet";
                #endregion
            }

            public static class MenuHeaderController
            {
                #region Properties
                public const string MenuHeader = "AdminMenuHeaderControllerMenuHeader";
                public const string Add = "AdminMenuHeaderControllerAdd";
                public const string Update = "AdminMenuHeaderControllerUpdate";
                public const string Delete = "AdminMenuHeaderControllerDelete";
                public const string Sort = "AdminMenuHeaderControllerSort";
                public const string Get = "AdminMenuHeaderControllerGet";
                #endregion
            }

            public static class NewsController
            {
                #region Properties
                public const string News = "AdminNewsControllerNews";
                public const string Grid = "AdminNewsControllerGrid";
                public const string GridAdd = "AdminNewsControllerGridAdd";
                public const string GridUpdate = "AdminNewsControllerGridUpdate";
                public const string GridDelete = "AdminNewsControllerGridDelete";                
                #endregion
            }

            public static class NewsPropertiesController
            {
                #region Properties
                public const string Properties = "AdminNewsPropertiesControllerProperties";
                public const string DeleteImage = "AdminNewsPropertiesControllerDeleteImage";
                #endregion
            }

            public static class PagesController
            {
                #region Properties
                public const string Pages = "AdminPagesControllerPages";
                public const string Grid = "AdminPagesControllerGrid";
                public const string GridAdd = "AdminPagesControllerGridAdd";
                public const string GridUpdate = "AdminPagesControllerGridUpdate";
                public const string GridDelete = "AdminPagesControllerGridDelete";
                public const string Get = "AdminPagesControllerGet";
                #endregion                
            }

            public static class PageDataController
            {
                #region Properties
                public const string Get = "AdminPageDataControllerGet";
                #endregion
            }

            public static class PagePropertiesController
            {
                #region Properties
                public const string Properties = "AdminPagePropertiesControllerProperties";
                public const string DeleteImage = "AdminPagePropertiesControllerDeleteImage";
                #endregion
            }

            public static class PageBuilderController
            {
                #region Properties
                public const string Builder = "AdminPageBuilderControllerBuilder";
                public const string BuilderLanguage = "AdminPageBuilderControllerBuilderLanguage";
                #endregion
            }

            public static class PagesManagementController
            {
                #region Properties
                public const string RedirectToChild = "AdminPagesManagementControllerRedirectToChild";
                #endregion
            }

            public static class PermissionsController
            {
                #region Properties
                public const string Permissions = "AdminPermissionsControllerPermissions";
                public const string Tree = "AdminPermissionsControllerTree";
                public const string TreeAdd = "AdminPermissionsControllerTreeAdd";
                public const string TreeUpdate = "AdminPermissionsControllerTreeUpdate";
                public const string TreeDelete = "AdminPermissionsControllerTreeDelete";
                #endregion
            }

            public static class ProductCategoriesController
            {
                #region Properties
                public const string Categories = "AdminProductCategoriesControllerCategories";
                public const string Add = "AdminProductCategoriesControllerAdd";
                public const string Delete = "AdminProductCategoriesControllerDelete";
                public const string Sort = "AdminProductCategoriesControllerSort";
                #endregion
            }

            public static class ProductCategoryPropertiesController
            {
                #region Properties                                    
                public const string Properties = "AdminProductCategoryPropertiesControllerProperties";
                public const string DeleteImage = "AdminProductCategoryPropertiesControllerDeleteImage";
                #endregion
            }

            public static class ProductsController
            {
                #region Properties
                public const string Products = "AdminProductsControllerProducts";
                public const string Grid = "AdminProductsControllerGrid";
                public const string GridAdd = "AdminProductsControllerGridAdd";
                public const string GridUpdate = "AdminProductsControllerGridUpdate";
                public const string GridDelete = "AdminProductsControllerGridDelete";
                public const string ExcelDownload = "AdminProductsControllerExcelDownload";
                public const string ExcelUpload = "AdminProductsControllerExcelUpload";
                #endregion                
            }

            public static class ProductsPropertiesController
            {
                #region Properties
                public const string Properties = "AdminProductsPropertiesControllerProperties";
                public const string DeleteImage = "AdminProductsPropertiesControllerDeleteImage";
                public const string ProductImagesUpload = "AdminProductsPropertiesControllerProductImagesUpload";
                public const string ProductImagesSort = "AdminProductsPropertiesControllerProductImagesSort";
                public const string ProductImagesDelete = "AdminProductsPropertiesControllerProductImagesDelete";
                #endregion
            }

            public static class RedirectsController
            {
                #region Properties
                public const string Redirects = "AdminRedirectsControllerRedirects";
                public const string Grid = "AdminRedirectsControllerGrid";
                public const string GridAdd = "AdminRedirectsControllerGridAdd";
                public const string GridUpdate = "AdminRedirectsControllerGridUpdate";
                public const string GridDelete = "AdminRedirectsControllerGridDelete";
                #endregion
            }

            public static class RolesControllers
            {
                #region Properties
                public const string Roles = "AdminRolesControllerRoles";
                public const string Grid = "AdminRolesControllerGrid";
                public const string GridAdd = "AdminRolesControllerGridAdd";
                public const string GridUpdate = "AdminRolesControllerGridUpdate";
                public const string GridDelete = "AdminRolesControllerGridDelete";
                #endregion
            }

            public static class RolePermissionsController
            {
                #region Properties
                public const string RolesPermissions = "AdminRolePermissionsControllerRolesPermissions";
                public const string RolesGrid = "AdminRolePermissionsControllerRolesGrid";
                public const string PermissionsTree = "AdminRolePermissionsControllerPermissionsTree";
                public const string GetPermissionsByRole = "AdminRolePermissionsControllerGetPermissionsByRole";
                public const string Save = "AdminRolePermissionsControllerSave";
                #endregion
            }
            
            public static class SystemPropertiesController
            {
                #region Properties
                public const string SystemProperies = "AdminSystemPropertiesControllerSystemProperties";
                public const string TestEmailSmtp = "AdminSystemPropertiesControllerTestEmailSmtp";
                public const string TestEmailMailgun = "AdminSystemPropertiesControllerTestEmailMailgun";
                public const string TestEmailOffice365 = "AdminSystemPropertiesControllerTestEmailOffice365";
                public const string TestAws = "AdminSystemPropertiesControllerTestAws";
                #endregion
            }

            public static class TeamMembersController
            {
                #region Properties
                public const string TeamMembers = "AdminTeamMembersControllerTeamMembers";
                public const string Grid = "AdminTeamMembersControllerGrid";
                public const string GridAdd = "AdminTeamMembersControllerGridAdd";
                public const string GridUpdate = "AdminTeamMembersControllerGridUpdate";
                public const string GridDelete = "AdminTeamMembersControllerGridDelete";
                public const string GridSort = "AdminTeamMembersControllerGridSort";
                #endregion                
            }

            public static class TeamMembersPropertiesController
            {
                #region Properties
                public const string Properties = "AdminTeamMembersTeamMemberProperties";
                public const string DeleteImage = "AdminTeamMembersTeamMemberDeleteImage";
                #endregion
            }

            public static class UsersController
            {
                #region Properties
                public const string Users = "AdminUsersControllerUsers";
                public const string Grid = "AdminUsersControllerGrid";
                public const string GridAdd = "AdminUsersControllerGridAdd";
                public const string GridUpdate = "AdminUsersControllerGridUpdate";
                public const string GridDelete = "AdminUsersControllerGridDelete";
                #endregion
            }

            public static class UserPropertiesController
            {
                #region Properties                
                public const string Properties = "AdminUserPropertiesControllerProperties";
                #endregion
            }

            public static class Utilities
            {
                #region Properties
                public const string Page = "AdminUtilities";
                public const string UfcCheckTransactionStatus = "AdminUtilitiesUfcCheckTransactionStatus";
                #endregion
            }
            #endregion
        }

        public static class Website
        {
            #region Nested Classes
            public static class ContactController
            {
                #region Properties
                public const string Contact = "WebsiteContactControllerContact";
                public const string ContactCulture = "WebsiteContactControllerContactCulture";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Index = "WebsiteHomeIndex";
                public const string IndexCulture = "WebsiteHomeIndexCulture";

                #endregion
            }

            public static class FileViewerController
            {
                #region Properties
                public const string Pdf = "FileViewerControllerPdf";
                #endregion
            }

            public static class PagesController
            {
                #region Properties
                public const string Page = "WebsitePagesControllerPage";
                public const string PageCulture = "WebsitePagesControllerPageCulture";
                #endregion
            }
            #endregion
        }
    }
}
