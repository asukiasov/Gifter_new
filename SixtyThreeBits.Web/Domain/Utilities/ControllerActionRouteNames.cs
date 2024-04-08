namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ControllerActionRouteNames
    {
        public static class Admin
        {
            #region Nested Classes
            public static class Auth
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";
                public const string Relogin = "AdminAuthRelogin";
                #endregion
            }

            public static class Blog
            {
                #region Properties
                public const string Page = "AdminBlog";
                public const string Grid = "AdminBlogGrid";
                public const string GridAdd = "AdminBlogGridAdd";
                public const string GridUpdate = "AdminBlogGridUpdate";
                public const string GridDelete = "AdminBlogGridDelete";
                public const string PostProperties = "AdminBlogPostProperties";
                public const string PostPropertiesDeleteImage = "AdminBlogPostPropertiesDeleteImage";
                #endregion
            }

            public static class Brands
            {
                #region Properties
                public const string Index = "AdminBrands";
                public const string BrandsGrid = "AdminBrandsGrid";
                public const string BrandsGridAdd = "AdminBrandsGridAdd";
                public const string BrandsGridUpdate = "AdminBrandsGridUpdate";
                public const string BrandsGridDelete = "AdminBrandsGridDelete";
                #endregion

                #region Nested Classes
                public static class Brand
                {
                    #region Properties
                    public const string Root = "AdminBrandsBrand";
                    public const string Properties = "AdminBrandsBrandProperties";
                    public const string DeleteCoverImage = "AdminBrandsBrandPropertiesDeleteCoverImage";
                    #endregion
                }
                #endregion
            }

            public static class Carousel
            {
                #region Properties
                public const string Page = "AdminCarousel";
                public const string Grid = "AdminCarouselGrid";
                public const string GridAdd = "AdminCarouselGridAdd";
                public const string GridUpdate = "AdminCarouselGridUpdate";
                public const string GridDelete = "AdminCarouselGridDelete";
                public const string GridSyncSortIndexes = "AdminCarouselGridSyncSortIndexes";
                #endregion

                #region Nested Classes
                public static class CarouselItem
                {
                    #region Properties
                    public const string DeleteImage = "AdminCarouselCarouselItemDeleteImage";
                    public const string Properties = "AdminCarouselCarouselItemProperties";
                    #endregion
                }
                #endregion
            }

            public static class ChangeLanguage
            {
                #region Properties
                public const string Page = "AdminChangeLanguage";
                #endregion
            }

            public static class Dictionaries
            {
                #region Properties
                public const string Page = "AdminDictionaries";
                public const string DictionariesTree = "AdminDictionariesTree";
                public const string DictionariesTreeAdd = "AdminDictionariesTreeAdd";
                public const string DictionariesTreeUpdate = "AdminDictionariesTreeUpdate";
                public const string DictionariesTreeDelete = "AdminDictionariesTreeDelete";
                #endregion
            }

            public static class EmailTemplates
            {
                #region Properties
                public const string Page = "AdminEmailTemplates";
                public const string Grid = "AdminEmailTemplatesGrid";
                #endregion

                #region Nested Classes
                public static class EmailTemplate
                {
                    #region Properties
                    public const string Properties = "AdminEmailTemplatesEmailTemplateProperties";
                    #endregion
                }
                #endregion
            }

            public static class FileManager
            {
                #region Properties
                public const string Page = "AdminFileManager";
                public const string Files = "AdminFileManagerFiles";
                public const string Upload = "AdminFileManagerUpload";
                public const string Delete = "AdminFileManagerDelete";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Page = "AdminHomeIndex";
                #endregion
            }

            public static class News
            {
                #region Properties
                public const string Page = "AdminNews";
                public const string Grid = "AdminNewsGrid";
                public const string GridAdd = "AdminNewsGridAdd";
                public const string GridUpdate = "AdminNewsGridUpdate";
                public const string GridDelete = "AdminNewsGridDelete";
                public const string NewsItem = "AdminNewsNewsItem";
                public const string NewsItemDeleteImage = "AdminNewsNewsItemDeleteImage";
                #endregion
            }

            public static class PagesManagemet
            {
                #region Properties
                public const string Root = "AdminPagesManagement";
                #endregion

                #region Nested Classes
                public static class Pages
                {
                    #region Properties
                    public const string Index = "AdminPages";
                    public const string Data = "AdminPagesData";
                    public const string Grid = "AdminPagesGrid";
                    public const string GridAdd = "AdminPagesGridAdd";
                    public const string GridUpdate = "AdminPagesGridUpdate";
                    public const string GridDelete = "AdminPagesGridDelete";
                    #endregion

                    #region Nested Classes
                    public static class Page
                    {
                        #region Properties
                        public const string Root = "AdminPagesPage";
                        public const string Data = "AdminPagesPageData";
                        public const string Properties = "AdminPagesPageProperties";
                        public const string PropertiesDeleteImage = "AdminPagesPagePropertiesDeleteImage";

                        public const string Builder = "AdminPagesPageBuilder";
                        public const string BuilderLanguage = "AdminPagesPageBuilderLanguage";
                        #endregion
                    }
                    #endregion
                }

                public static class MenuHeader
                {
                    #region Properties
                    public const string Page = "AdminMenuHeader";
                    public const string Add = "AdminMenuHeaderAdd";
                    public const string Update = "AdminMenuHeaderUpdate";
                    public const string Delete = "AdminMenuHeaderDelete";
                    public const string Sort = "AdminMenuHeaderSort";
                    public const string Get = "AdminMenuHeaderGet";
                    #endregion
                }

                public static class MenuFooter
                {
                    #region Properties
                    public const string Page = "AdminMenuFooter";
                    public const string Add = "AdminMenuFooterAdd";
                    public const string Update = "AdminMenuFooterUpdate";
                    public const string Delete = "AdminMenuFooterDelete";
                    public const string Sort = "AdminMenuFooterSort";
                    public const string Get = "AdminMenuFooterGet";
                    #endregion
                }
                #endregion
            }

            public static class Permissions
            {
                #region Properties
                public const string Page = "AdminPermissions";
                public const string Tree = "AdminPermissionsTree";
                public const string TreeAdd = "AdminPermissionsTreeAdd";
                public const string TreeUpdate = "AdminPermissionsTreeUpdate";
                public const string TreeDelete = "AdminPermissionsTreeDelete";
                public const string TreeUpdateParent = "AdminPermissionsTreeUpdateParent";
                #endregion
            }

            public static class ProductCategories
            {
                #region Properties
                public const string Index = "AdminProductCategories";
                public const string Add = "AdminProductCategoriesAdd";
                public const string Delete = "AdminProductCategoriesDelete";
                public const string Sort = "AdminProductCategoriesSort";
                #endregion

                #region Nested Classes
                public static class ProductCategory
                {
                    #region Properties                    
                    public const string Root = "AdminProductCategoriesProductCategoryRoot";
                    public const string Properties = "AdminProductCategoriesProductCategoryProperties";
                    public const string ImageDelete = "AdminProductCategoriesProductCategoryPropertiesImageDelete";
                    #endregion
                }
                #endregion
            }

            public static class Products
            {
                #region Properties
                public const string Index = "AdminProducts";
                public const string Grid = "AdminProductsGrid";
                public const string GridAdd = "AdminProductsGridAdd";
                public const string GridUpdate = "AdminProductsGridUpdate";
                public const string GridDelete = "AdminProductsGridDelete";
                public const string ExcelDownload = "AdminProductsExcelDownload";
                public const string ExcelUpload = "AdminProductsExcelUpload";
                #endregion

                #region Nested Classes
                public static class Product
                {
                    #region Properties
                    public const string Root = "AdminProductsProduct";
                    public const string Properties = "AdminProductsProductProperties";
                    public const string PropertiesImageDelete = "AdminProductsProductPropertiesImageDelete";
                    public const string PropertiesImagesUpload = "AdminProductsProductPropertiesImagesUpload";
                    public const string PropertiesImagesSort = "AdminProductsProductPropertiesImagesSort";
                    public const string PropertiesImagesDelete = "AdminProductsProductPropertiesImagesDelete";
                    #endregion                    
                }
                #endregion
            }

            public static class Redirects
            {
                #region Properties
                public const string Index = "AdminRedirects";
                public const string Grid = "AdminRedirectsGrid";
                public const string GridAdd = "AdminRedirectsGridAdd";
                public const string GridUpdate = "AdminRedirectsGridUpdate";
                public const string GridDelete = "AdminRedirectsGridDelete";
                #endregion
            }

            public static class Roles
            {
                #region Properties
                public const string Page = "AdminRoles";
                public const string Grid = "AdminRolesGrid";
                public const string GridAdd = "AdminRolesGridAdd";
                public const string GridUpdate = "AdminRolesGridUpdate";
                public const string GridDelete = "AdminRolesGridDelete";
                #endregion
            }

            public static class RolesPermissions
            {
                #region Properties
                public const string Page = "AdminRolesPermissions";
                public const string RolesGrid = "AdminRolesPermissionsRolesGrid";
                public const string PermissionsTree = "AdminRolesPermissionsPermissionsTree";
                public const string PermissionsGetByRole = "AdminRolesPermissionsPermissionsGetByRole";
                public const string Save = "AdminRolesPermissionsSave";
                #endregion
            }

            public static class ServiceLogs
            {
                #region Properties
                public const string Page = "AdminServiceLogs";
                public const string Grid = "AdminServiceLogsGrid";
                #endregion
            }

            public static class SystemProperties
            {
                #region Properties
                public const string Page = "AdminSystemProperties";
                public const string TestEmailSmtp = "AdminSystemPropertiesTestEmailSmtp";
                public const string TestEmailMailgun = "AdminSystemPropertiesTestEmailMailgun";
                public const string TestEmailOffice365 = "AdminSystemPropertiesTestEmailOffice365";
                public const string TestAws = "AdminSystemPropertiesTestAws";
                #endregion
            }

            public static class TeamMembers
            {
                #region Properties
                public const string TeamMembersPage = "AdminTeamMembers";
                public const string TeamMembersGrid = "AdminTeamMembersGrid";
                public const string TeamMembersGridAdd = "AdminTeamMembersGridAdd";
                public const string TeamMembersGridUpdate = "AdminTeamMembersGridUpdate";
                public const string TeamMembersGridDelete = "AdminTeamMembersGridDelete";
                public const string TeamMembersGridSort = "AdminTeamMembersGridSort";
                #endregion

                #region Nested Classes
                public static class TeamMember
                {
                    #region Properties
                    public const string Properties = "AdminTeamMembersTeamMemberProperties";
                    public const string PropertiesDeleteImage = "AdminTeamMembersTeamMemberPropertiesDeleteImage";
                    #endregion
                }
                #endregion
            }

            public static class Users
            {
                #region Properties
                public const string Page = "AdminUsers";
                public const string Grid = "AdminUsersGrid";
                public const string GridAdd = "AdminUsersGridAdd";
                public const string GridUpdate = "AdminUsersGridUpdate";
                public const string GridDelete = "AdminUsersGridDelete";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string Root = "AdminUsersUser";
                    public const string Properties = "AdminUsersUserProperties";
                    #endregion
                }
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
            public static class Checkout
            {
                #region Properties
                public const string Page = "WebsiteCheckout";
                public const string Success = "WebsiteCheckoutSuccess";
                public const string Fail = "WebsiteCheckoutFail";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Index = "WebsiteHomeIndex";
                public const string IndexCulture = "WebsiteHomeIndexCulture";

                #endregion
            }

            public static class FileViewer
            {
                #region Properties
                public const string Pdf = "FileViewerPdf";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string Page = "WebsitePagesPage";
                public const string PageCulture = "WebsitePagesPageCulture";
                #endregion
            }
            #endregion
        }
    }
}
