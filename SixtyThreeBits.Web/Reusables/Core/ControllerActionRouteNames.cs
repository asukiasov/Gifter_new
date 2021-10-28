namespace SixtyThreeBits.Web.Reusables.Core
{
    public class ControllerActionRouteNames
    {
        public class Admin
        {
            #region Sub Classes
            public class Auth
            {
                #region Properties
                public const string Login = "AdminAuthLogin";
                public const string Logout = "AdminAuthLogout";
                public const string Relogin = "AdminAuthRelogin";
                #endregion
            }

            public class Blog
            {
                #region Properties
                public const string Page = "AdminBlog";
                public const string Grid = "AdminBlogGrid";
                public const string GridAdd = "AdminBlogGridAdd";
                public const string GridUpdate = "AdminBlogGridUpdate";
                public const string GridDelete = "AdminBlogGridDelete";
                public const string BlogItem = "AdminBlogBlogItem";
                public const string BlogItemDeleteImage = "AdminBlogBlogItemPropertiesDeleteImage";
                #endregion
            }

            public class Brands
            {
                #region Properties
                public const string Index = "AdminBrands";
                public const string BrandsGrid = "AdminBrandsGrid";
                public const string BrandsGridAdd = "AdminBrandsGridAdd";
                public const string BrandsGridUpdate = "AdminBrandsGridUpdate";
                public const string BrandsGridDelete = "AdminBrandsGridDelete";
                #endregion

                #region Sub Classes
                public class Brand
                {
                    #region Properties
                    public const string Parent = "AdminBrandsBrand";
                    public const string Properties = "AdminBrandsBrandProperties";
                    public const string DeleteCoverImage = "AdminBrandsBrandPropertiesDeleteCoverImage";
                    #endregion
                }
                #endregion
            }

            public class Carousel
            {
                #region Properties
                public const string Page = "AdminCarousel";
                public const string Grid = "AdminCarouselGrid";
                public const string GridAdd = "AdminCarouselGridAdd";
                public const string GridUpdate = "AdminCarouselGridUpdate";
                public const string GridDelete = "AdminCarouselGridDelete";
                public const string GridSyncSortIndexes = "AdminCarouselGridSyncSortIndexes";
                #endregion

                #region Sub Classes
                public class CarouselItem
                {
                    #region Properties
                    public const string DeleteImage = "AdminCarouselCarouselItemDeleteImage";
                    public const string Properties = "AdminCarouselCarouselItemProperties";
                    #endregion
                }
                #endregion
            }

            public class Categories
            {
                #region Properties
                public const string Index = "AdminCategories";
                public const string Add = "AdminCategoriesAdd";
                public const string Delete = "AdminCategoriesDelete";
                public const string Sync = "AdminCategoriesSync";
                #endregion

                #region Sub Classes
                public class Category
                {
                    #region Properties                    
                    public const string Properties = "AdminCategoriesCategoryProperties";
                    public const string DeleteImage = "AdminCategoriesCategoryPropertiesDeleteImage";
                    #endregion
                }
                #endregion
            }

            public class Dictionaries
            {
                #region Properties
                public const string Page = "AdminDictionaries";
                public const string DictionariesTree = "AdminDictionariesTree";
                public const string DictionariesTreeAdd = "AdminDictionariesTreeAdd";
                public const string DictionariesTreeUpdate = "AdminDictionariesTreeUpdate";
                public const string DictionariesTreeDelete = "AdminDictionariesTreeDelete";
                #endregion
            }

            public class FacebookGalleries
            {
                #region Properties
                public const string Page = "AdminGalleries";
                public const string GalleriesGrid = "AdminGalleriesGrid";
                public const string GalleriesGridAdd = "AdminGalleriesGridAdd";
                public const string GalleriesGridUpdate = "AdminGalleriesGridUpdate";
                public const string GalleriesGridDelete = "AdminGalleriesGridDelete";
                public const string GalleriesGridSyncSortIndexes = "AdminGalleriesGridSyncSortIndexes";
                #endregion

                #region Sub Classes
                public class Gallery
                {
                    #region Properties
                    public const string Properties = "AdminGalleryItem";
                    public const string PropertiesGetImages = "AdminGalleryItemGetImages";
                    #endregion
                }
                #endregion
            }

            public class Faq
            {
                #region Properties
                public const string Page = "AdminFaq";
                public const string FaqCreate = "AdminFaqCreate";
                public const string FaqUpdate = "AdminFaqUpdate";
                public const string FaqDelete = "AdminFaqDelete";
                public const string FaqSyncSortIndexes = "AdminFaqSyncSortIndexes";
                #endregion
            }

            public class FileManager
            {
                #region Properties
                public const string Index = "AdminFileManager";
                public const string Files = "AdminFileManagerFiles";
                public const string FilesUpload = "AdminFileManagerFilesUpload";
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Page = "AdminHomeIndex";
                #endregion
            }

            public class News
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

            public class Pages
            {
                #region Properties
                public const string Index = "AdminPages";
                public const string AddNew = "AdminPagesAddNew";
                public const string Update = "AdminPagesUpdate";
                public const string Delete = "AdminPagesDelete";
                public const string SyncParentsAndSortIndexes = "AdminPagesSyncParentsAndSortIndexes";
                #endregion

                #region Sub Classes
                public class Page
                {
                    #region Properties
                    public const string Root = "AdminPagesPage";
                    public const string Properties = "AdminPagesPageProperties";
                    public const string DeleteImage = "AdminPagesPagePropertiesDeleteImage";

                    public const string Text = "AdminPagesPageText";
                    public const string TextLanguage = "AdminPagesPageTextLanguage";

                    public const string Builder = "AdminPagesPageBuilder";
                    public const string BuilderLanguage = "AdminPagesPageBuilderLanguage";
                    #endregion
                }
                #endregion
            }

            public class Products
            {
                #region Properties
                public const string Index = "AdminProducts";
                public const string ProductsRemainderSync = "ProductsRemainderSync";
                public const string ProductsGrid = "AdminProductsGrid";
                public const string ProductsGridAdd = "AdminProductsGridAdd";
                public const string ProductsGridUpdate = "AdminProductsGridUpdate";
                public const string ProductsGridDelete = "AdminProductsGridDelete";
                #endregion

                #region Sub Classes
                public class Product
                {
                    #region Properties
                    public const string Parent = "AdminProductsProduct";
                    public const string Properties = "AdminProductsProduct";
                    public const string PropertiesImagesUpload = "AdminProductsProductImagesUpload";
                    public const string PropertiesImagesSort = "AdminProductsProductImagesSort";
                    public const string PropertiesImagesDelete = "AdminProductsProductImagesDelete";
                    #endregion                    
                }
                #endregion

            }

            public class Projects
            {
                #region Properties
                public const string Index = "AdminProjects";
                public const string ProjectsGrid = "AdminProjectsGrid";
                public const string ProjectsGridAdd = "AdminProjectsGridAdd";
                public const string ProjectsGridUpdate = "AdminProjectsGridUpdate";
                public const string ProjectsGridDelete = "AdminProjectsGridDelete";
                public const string ProjectsGridSyncSortIndexes = "AdminProjectsGridSyncSortIndexes";
                #endregion

                #region Sub Classes
                public class Project
                {
                    #region Properties
                    public const string Parent = "AdminProjectsProject";
                    public const string Properties = "AdminProjectsProjectProperties";
                    public const string DeleteCoverImage = "AdminProjectsProjectsPropertiesDeleteCoverImage";

                    #region Gallery
                    public const string Gallery = "AdminProjectsProjectGallery";
                    public const string GalleryUpload = "AdminProjectsProjectGalleryUpload";
                    public const string GalleryDelete = "AdminProjectsProjectGalleryDelete";
                    public const string GallerySyncSortIndexes = "AdminProjectsProjectGalleryGallerySyncSortIndexes";
                    #endregion
                    #endregion
                }
                #endregion
            }

            public class ServiceLogs
            {
                #region Properties
                public const string Page = "AdminServiceLogs";
                public const string Grid = "AdminServiceLogsGrid";
                #endregion
            }

            public class SystemProperties
            {
                #region Properties
                public const string Page = "AdminSystemProperties";
                public const string TestSmtp = "AdminSystemPropertiesTestSmtp";
                #endregion
            }

            public class TeamMembers
            {
                #region Properties
                public const string TeamMembersPage = "AdminTeamMembers";
                public const string TeamMembersGrid = "AdminTeamMembersGrid";
                public const string TeamMembersGridAdd = "AdminTeamMembersGridAdd";
                public const string TeamMembersGridUpdate = "AdminTeamMembersGridUpdate";
                public const string TeamMembersGridDelete = "AdminTeamMembersGridDelete";
                public const string TeamMembersSyncSortIndexes = "AdminTeamMembersSortIndexes";
                #endregion

                #region Sub Classes
                public class TeamMember
                {
                    #region Properties
                    public const string Properties = "AdminTeamMembersTeamMemberProperties";
                    public const string TeamMembersItemDeleteImage = "AdminTeamMembersTeamMemberPropertiesDeleteImage";
                    #endregion
                }
                #endregion
            }

            public class UserManagement
            {
                #region Properties
                public const string Users = "AdminUserManagementUsers";
                public const string UsersGrid = "AdminUserManagementUsersGrid";
                public const string UsersGridAdd = "AdminUserManagementUsersGridAdd";
                public const string UsersGridUpdate = "AdminUserManagementUsersGridUpdate";
                public const string UsersGridDelete = "AdminUserManagementUsersGridDelete";

                public const string Roles = "AdminUserManagementRoles";
                public const string RolesGrid = "AdminUserManagementRolesGrid";
                public const string RolesGridAdd = "AdminUserManagementRolesGridAdd";
                public const string RolesGridUpdate = "AdminUserManagementRolesGridUpdate";
                public const string RolesGridDelete = "AdminUserManagementRolesGridDelete";

                public const string Permissions = "AdminUserManagementPermissions";
                public const string PermissionsTree = "AdminUserManagementPermissionsTree";
                public const string PermissionsTreeAdd = "AdminUserManagementPermissionsTreeAdd";
                public const string PermissionsTreeUpdate = "AdminUserManagementPermissionsTreeUpdate";
                public const string PermissionsTreeDelete = "AdminUserManagementPermissionsTreeDelete";
                public const string PermissionsTreeUpdateParent = "AdminUserManagementPermissionsTreeUpdateParent";

                public const string RolePermissions = "AdminUserManagementRolePermissions";
                public const string RolePermissionsGet = "AdminUserManagementRolePermissionsGet";
                public const string RolePermissionsRolesGrid = "AdminUserManagementRolePermissionsRolesGrid";
                public const string RolePermissionsPermissionsTree = "AdminUserManagementRolePermissionsPermissionsTree";
                public const string RolePermissionsSave = "AdminUserManagementRolePermissionsSave";
                #endregion
            }

            public class Utilities
            {
                #region Properties
                public const string Page = "AdminUtilities";
                public const string UfcCheckTransactionStatus = "AdminUtilitiesUfcCheckTransactionStatus";
                #endregion
            }
            #endregion
        }

        public class Website
        {
            #region Sub Classes
            public class Checkout
            {
                #region Properties
                public const string Page = "WebsiteCheckout";
                public const string Success = "WebsiteCheckoutSuccess";
                public const string Fail = "WebsiteCheckoutFail";
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Index = "WebsiteHomeIndex";
                public const string IndexCulture = "WebsiteHomeIndexCulture";

                #endregion
            }

            public class FileViewer
            {
                #region Properties
                public const string Pdf = "FileViewerPdf";
                #endregion
            }

            public class Pages
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
