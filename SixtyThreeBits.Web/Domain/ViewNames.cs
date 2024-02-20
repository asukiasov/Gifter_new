namespace SixtyThreeBits.Web.Domain
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
                public const string Login = "~/Views/Admin/Auth/Login.cshtml";
                #endregion
            }

            public static class Blog
            {
                #region Properties
                public const string Page = "~/Views/Admin/Blog/Blog.cshtml";
                public const string BlogPostProperties = "~/Views/Admin/Blog/BlogPostProperties.cshtml";
                #endregion
            }

            public static class Brands
            {
                #region Properties
                public const string Page = "~/Views/Admin/Brands/Brands.cshtml";
                public const string BrandProperties = "~/Views/Admin/Brands/BrandProperties.cshtml";
                #endregion                
            }

            public static class Carousel
            {
                #region Properties
                public const string Page = "~/Views/Admin/Carousel/Carousel.cshtml";
                public const string CarouselItem = "~/Views/Admin/Carousel/CarouselItem.cshtml";
                #endregion               
            }
            
            public static class Dictionaries
            {
                #region Properties
                public const string Page = "~/Views/Admin/Dictionaries/Dictionaries.cshtml";
                #endregion
            }

            public static class DiscountCoupons
            {
                #region Properties
                public const string Page = "~/Views/Admin/DiscountCoupons/DiscountCoupons.cshtml";
                #endregion
            }

            public static class EmailTemplates
            {
                #region Properties
                public const string Page = "~/Views/Admin/EmailTemplates/EmailTemplates.cshtml";
                #endregion

                #region Nested Classes
                public static class EmailTemplate
                {
                    #region Properties
                    public const string Properties = "~/Views/Admin/EmailTemplates/EmailTemplateProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class FileManager
            {
                #region Properties
                public const string Page = "~/Views/Admin/FileManager/FileManager.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Index = "~/Views/Admin/Home/Index.cshtml";
                #endregion
            }

            public static class News
            {
                #region Properties
                public const string Page = "~/Views/Admin/News/News.cshtml";
                public const string NewsProperties = "~/Views/Admin/News/NewsProperties.cshtml";
                #endregion
            }

            public static class NotFound
            {
                #region Properties
                public const string Page = "~/Views/Website/NotFound/NotFound.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string Tree = "~/Views/Admin/Pages/PagesTree.cshtml";
                #endregion

                #region Nested Classes
                public static class Page
                {
                    #region Properties
                    public const string Properties = "~/Views/Admin/Pages/Page/PageProperties.cshtml";
                    public const string Builder = "~/Views/Admin/Pages/Page/PageBuilder.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class Partners
            {
                #region Properties
                public const string Page = "Views/Admin/Partners/Partners.cshtml";
                public const string Partner = "Views/Admin/Partners/PartnerProperties.cshtml";
                #endregion
            }

            public static class ProductCategories
            {
                #region Properties
                public const string Page = "~/Views/Admin/ProductCategories/ProductCategories.cshtml";
                public const string ProductCategoryProperties = "~/Views/Admin/ProductCategories/ProductCategoryProperties.cshtml";
                #endregion                
            }

            public static class Products
            {
                #region Properties
                public const string Page = "~/Views/Admin/Products/Products.cshtml";
                public const string ProductProperties = "~/Views/Admin/Products/ProductProperties.cshtml";
                #endregion                
            }            

            public static class Permissions
            {
                #region Properties
                public const string Page = "~/Views/Admin/Permissions/Permissions.cshtml";
                #endregion
            }

            public static class Redirects
            {
                #region Properties
                public const string Page = "~/Views/Admin/Redirects/Redirects.cshtml";
                #endregion               
            }

            public static class Roles
            {
                #region Properties
                public const string Page = "~/Views/Admin/Roles/Roles.cshtml";
                #endregion
            }

            public static class RolesPermissions
            {
                #region Properties
                public const string Page = "~/Views/Admin/RolesPermissions/RolesPermissions.cshtml";
                #endregion
            }

            public static class ServiceLogs
            {
                #region Properties
                public const string Page = "~/Views/Admin/ExternalCommunicationServiceLog/ExternalCommunicationServiceLog.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Admin/Shared/Layout.cshtml";
                public const string NotFound = "~/Views/Admin/Shared/NotFound.cshtml";
                public const string PopupLayout = "~/Views/Admin/Shared/PopupLayout.cshtml";
                public const string Tabs = "~/Views/Admin/Shared/Tabs.cshtml";
                public const string SuccessErrorToastPartialView = "~/Views/Admin/Shared/SuccessErrorToastPartialView.cshtml";

                public const string ButtonAddNew = "~/Views/Admin/Shared/ButtonAddNew.cshtml";
                public const string ButtonSave = "~/Views/Admin/Shared/ButtonSave.cshtml";
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
                public const string Page = "~/Views/Admin/SystemProperties/SystemProperties.cshtml";
                #endregion
            }

            public static class TeamMembers
            {
                #region Properties
                public const string TeamMembersPage = "~/Views/Admin/TeamMembers/TeamMembers.cshtml";
                public const string TeamMemberProperties = "~/Views/Admin/TeamMembers/TeamMemberProperties.cshtml";
                #endregion
            }

            public static class Users
            {
                #region Properties
                public const string Page = "~/Views/Admin/Users/Users.cshtml";                
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string Layout = "~/Views/Admin/UserManagement/User/UserLayout.cshtml";
                    public const string Properties = "~/Views/Admin/UserManagement/User/UserProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class Utilities
            {
                #region Properties
                public const string Page = "~/Views/Admin/Utilities/Utilities.cshtml";
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
                public const string Pdf = "~/Views/Website/PdfViewer/PdfViewer.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Page = "~/Views/Website/Home/Index.cshtml";
                #endregion
            }

            public static class NotFound
            {
                #region Properties
                public const string Page = "~/Views/Website/NotFound/NotFound.cshtml";
                #endregion
            }

            public static class Test
            {
                #region Properties
                public const string Page = "~/Views/Website/Test/Test.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string Page = "~/Views/Website/Pages/Page.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Website/Shared/Layout.cshtml";
                public const string Pager = "~/Views/Website/Shared/Pager.cshtml";
                #endregion
            }
            #endregion
        }

        public static class Shared
        {            
            #region Nested Classes            
            public static class FileTree
            {
                #region Properties
                public const string Tree = "~/Views/Shared/FileTree/FileTree.cshtml";
                public const string Folder = "~/Views/Shared/FileTree/FileTreeFolder.cshtml";
                public const string File = "~/Views/Shared/FileTree/FileTreeFile.cshtml";
                #endregion
            }

            public static class PluginsClient
            {
                #region Properties
                public const string Footer = "~/Views/Shared/PluginsClient/PluginsClientFooter.cshtml";
                public const string Header = "~/Views/Shared/PluginsClient/PluginsClientHeader.cshtml";
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
