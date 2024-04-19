namespace SixtyThreeBits.Web.Domain.Utilities
{
    public static class ViewNames
    {
        #region Nested Classes
        public static class Admin
        {
            #region Nested Classes
            public static class Auth
            {
                #region Properties
                public const string LoginView = "~/Views/Admin/Auth/Login.cshtml";
                #endregion
            }

            public static class BlogPosts
            {
                #region Properties
                public const string BlogPostsView = "~/Views/Admin/BlogPosts/BlogPosts.cshtml";
                public const string BlogPostPropertiesView = "~/Views/Admin/BlogPosts/BlogPostProperties.cshtml";
                #endregion
            }

            public static class Brands
            {
                #region Properties
                public const string BrandsView = "~/Views/Admin/Brands/Brands.cshtml";
                public const string BrandPropertiesView = "~/Views/Admin/Brands/BrandProperties.cshtml";
                #endregion                
            }

            public static class Dictionaries
            {
                #region Properties
                public const string DictionariesView = "~/Views/Admin/Dictionaries/Dictionaries.cshtml";
                #endregion
            }

            public static class EmailTemplates
            {
                #region Properties
                public const string EmailTemplatesView = "~/Views/Admin/EmailTemplates/EmailTemplates.cshtml";
                public const string EmailTemplatePropertiesView = "~/Views/Admin/EmailTemplates/EmailTemplateProperties.cshtml";
                #endregion                
            }

            public static class Errors
            {
                #region Properties
                public const string NotFoundView = "~/Views/Admin/Errors/NotFound.cshtml";
                #endregion
            }

            public static class FileManager
            {
                #region Properties
                public const string FileManagerView = "~/Views/Admin/FileManager/FileManager.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string IndexView = "~/Views/Admin/Home/Index.cshtml";
                #endregion
            }

            public static class MenuFooter
            {
                #region Properties
                public const string Page = "~/Views/Admin/MenuFooter/MenuFooter.cshtml";
                public const string MenuFooterTreeNodePartialView = "~/Views/Admin/MenuFooter/MenuFooterTreeNodePartialView.cshtml";
                #endregion
            }

            public static class MenuHeader
            {
                #region Properties
                public const string Page = "~/Views/Admin/MenuHeader/MenuHeader.cshtml";
                public const string MenuHeaderTreeNodePartialView = "~/Views/Admin/MenuHeader/MenuHeaderTreeNodePartialView.cshtml";
                #endregion
            }

            public static class News
            {
                #region Properties
                public const string NewsView = "~/Views/Admin/News/News.cshtml";
                public const string NewsPropertiesView = "~/Views/Admin/News/NewsProperties.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string PagesView = "~/Views/Admin/Pages/Pages.cshtml";
                #endregion

                #region Nested Classes
                public static class Page
                {
                    #region Properties
                    public const string PagePropertiesView = "~/Views/Admin/Pages/Page/PageProperties.cshtml";
                    public const string PageBuilderView = "~/Views/Admin/Pages/Page/PageBuilder.cshtml";
                    #endregion
                }
                #endregion
            }            

            public static class ProductCategories
            {
                #region Properties
                public const string ProductCategoriesView = "~/Views/Admin/ProductCategories/ProductCategories.cshtml";
                public const string ProductCategoryPropertiesView = "~/Views/Admin/ProductCategories/ProductCategoryProperties.cshtml";
                #endregion                
            }

            public static class Products
            {
                #region Properties
                public const string ProductsView = "~/Views/Admin/Products/Products.cshtml";
                public const string ProductPropertiesView = "~/Views/Admin/Products/ProductProperties.cshtml";
                #endregion                
            }

            public static class Permissions
            {
                #region Properties
                public const string PermissionsView = "~/Views/Admin/Permissions/Permissions.cshtml";
                #endregion
            }

            public static class Redirects
            {
                #region Properties
                public const string RedirectsView = "~/Views/Admin/Redirects/Redirects.cshtml";
                #endregion               
            }

            public static class Roles
            {
                #region Properties
                public const string RolesView = "~/Views/Admin/Roles/Roles.cshtml";
                #endregion
            }

            public static class RolesPermissions
            {
                #region Properties
                public const string RolesPermissionsView = "~/Views/Admin/RolesPermissions/RolesPermissions.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string LayoutView = "~/Views/Admin/Shared/Layout.cshtml";
                public const string PopupLayoutView = "~/Views/Admin/Shared/PopupLayout.cshtml";
                public const string TabsView = "~/Views/Admin/Shared/Tabs.cshtml";
                public const string SuccessErrorToastPartialView = "~/Views/Admin/Shared/SuccessErrorToastPartialView.cshtml";

                public const string ButtonAddNewPartialView = "~/Views/Admin/Shared/ButtonAddNewPartialView.cshtml";
                public const string ButtonDeletePartialView = "~/Views/Admin/Shared/ButtonDeletePartialView.cshtml";
                public const string ButtonSavePartialView = "~/Views/Admin/Shared/ButtonSavePartialView.cshtml";
                #endregion

                #region Nested Classes
                public static class FileTreeEditor
                {
                    #region Properties
                    public const string Editor = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditor.cshtml";
                    public const string File = "~/Views/Admin/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class SystemProperties
            {
                #region Properties
                public const string SystemPropertiesView = "~/Views/Admin/SystemProperties/SystemProperties.cshtml";
                #endregion
            }

            public static class TeamMembers
            {
                #region Properties
                public const string TeamMembersView = "~/Views/Admin/TeamMembers/TeamMembers.cshtml";
                public const string TeamMemberPropertiesView = "~/Views/Admin/TeamMembers/TeamMemberProperties.cshtml";
                #endregion
            }

            public static class Users
            {
                #region Properties
                public const string UsersView = "~/Views/Admin/Users/Users.cshtml";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string UserLayoutView = "~/Views/Admin/Users/User/UserLayout.cshtml";
                    public const string UserPropertiesView = "~/Views/Admin/Users/User/UserProperties.cshtml";
                    #endregion
                }
                #endregion
            }           
            #endregion
        }

        public static class Website
        {
            #region Nested Classes
            public static class FileViewer
            {
                #region Properties
                public const string PdfViewerView = "~/Views/Website/PdfViewer/PdfViewer.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string IndexView = "~/Views/Website/Home/Index.cshtml";
                #endregion
            }

            public static class Errors
            {
                #region Properties
                public const string NotFoundView = "~/Views/Website/Errors/NotFound.cshtml";
                #endregion
            }

            public static class Test
            {
                #region Properties
                public const string TestView = "~/Views/Website/Test/Test.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string PageView = "~/Views/Website/Pages/Page.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string LayoutView = "~/Views/Website/Shared/Layout.cshtml";
                public const string PagerPartialView = "~/Views/Website/Shared/PagerPartialView.cshtml";
                #endregion
            }
            #endregion
        }

        public static class Shared
        {
            #region Nested Classes            
            public static class PluginsClient
            {
                #region Properties
                public const string PluginsClientFooter = "~/Views/Shared/PluginsClient/PluginsClientFooter.cshtml";
                public const string PluginsClientHeader = "~/Views/Shared/PluginsClient/PluginsClientHeader.cshtml";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
