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
                public const string Login = "~/Admin/Views/Auth/Login.cshtml";
                #endregion
            }

            public static class Blog
            {
                #region Properties
                public const string Page = "~/Admin/Views/Blog/Blog.cshtml";
                public const string BlogPostProperties = "~/Admin/Views/Blog/BlogPostProperties.cshtml";
                #endregion
            }

            public static class Brands
            {
                #region Properties
                public const string Page = "~/Admin/Views/Brands/Brands.cshtml";
                public const string BrandProperties = "~/Admin/Views/Brands/BrandProperties.cshtml";
                #endregion                
            }

            public static class Carousel
            {
                #region Properties
                public const string Page = "~/Admin/Views/Carousel/Carousel.cshtml";
                public const string CarouselItem = "~/Admin/Views/Carousel/CarouselItem.cshtml";
                #endregion               
            }

            public static class ProductCategories
            {
                #region Properties
                public const string Page = "~/Admin/Views/ProductCategories/ProductCategories.cshtml";
                public const string ProductCategoryProperties = "~/Admin/Views/ProductCategories/ProductCategoryProperties.cshtml";
                #endregion                
            }

            public static class Dictionaries
            {
                #region Properties
                public const string Page = "~/Admin/Views/Dictionaries/Dictionaries.cshtml";
                #endregion
            }

            public static class DiscountCoupons
            {
                #region Properties
                public const string Page = "~/Admin/Views/DiscountCoupons/DiscountCoupons.cshtml";
                #endregion
            }

            public static class EmailTemplates
            {
                #region Properties
                public const string Page = "~/Admin/Views/EmailTemplates/EmailTemplates.cshtml";
                #endregion

                #region Nested Classes
                public static class EmailTemplate
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/EmailTemplates/EmailTemplateProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class FileManager
            {
                #region Properties
                public const string Page = "~/Admin/Views/FileManager/FileManager.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Index = "~/Admin/Views/Home/Index.cshtml";
                #endregion
            }

            public static class News
            {
                #region Properties
                public const string Page = "~/Admin/Views/News/News.cshtml";
                public const string NewsProperties = "~/Admin/Views/News/NewsProperties.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string Tree = "~/Admin/Views/Pages/PagesTree.cshtml";
                #endregion

                #region Nested Classes
                public static class Page
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/Pages/Page/PageProperties.cshtml";
                    public const string Builder = "~/Admin/Views/Pages/Page/PageBuilder.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class Partners
            {
                #region Properties
                public const string Page = "Admin/Views/Partners/Partners.cshtml";
                public const string Partner = "Admin/Views/Partners/PartnerProperties.cshtml";
                #endregion
            }

            public static class Products
            {
                #region Properties
                public const string Page = "~/Admin/Views/Products/Products.cshtml";
                public const string ProductProperties = "~/Admin/Views/Products/ProductProperties.cshtml";
                #endregion                
            }            

            public static class Redirects
            {
                #region Properties
                public const string Page = "~/Admin/Views/Redirects/Redirects.cshtml";
                #endregion               
            }

            public static class ServiceLogs
            {
                #region Properties
                public const string Page = "~/Admin/Views/ExternalCommunicationServiceLog/ExternalCommunicationServiceLog.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Admin/Views/Shared/Layout.cshtml";
                public const string NotFound = "~/Admin/Views/Shared/NotFound.cshtml";
                public const string PopupLayout = "~/Admin/Views/Shared/PopupLayout.cshtml";
                public const string Tabs = "~/Admin/Views/Shared/Tabs.cshtml";
                public const string SuccessErrorPartialView = "~/Admin/Views/Shared/SuccessErrorPartialView.cshtml";

                public const string ButtonAddNew = "~/Admin/Views/Shared/ButtonAddNew.cshtml";
                public const string ButtonSave = "~/Admin/Views/Shared/ButtonSave.cshtml";
                #endregion

                #region Nested Classes
                public static class FileTreeEditor
                {
                    #region Properties
                    public const string Editor = "~/Admin/Views/Shared/FileTreeEditor/FileTreeEditor.cshtml";
                    public const string File = "~/Admin/Views/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class SystemProperties
            {
                #region Properties
                public const string Page = "~/Admin/Views/SystemProperties/SystemProperties.cshtml";
                #endregion
            }

            public static class TeamMembers
            {
                #region Properties
                public const string TeamMembersPage = "~/Admin/Views/TeamMembers/TeamMembers.cshtml";
                public const string TeamMemberProperties = "~/Admin/Views/TeamMembers/TeamMemberProperties.cshtml";
                #endregion
            }

            public static class UserManagement
            {
                #region Properties
                public const string Users = "~/Admin/Views/UserManagement/Users.cshtml";
                public const string MyAccount = "~/Admin/Views/UserManagement/MyAccount.cshtml";
                public const string Roles = "~/Admin/Views/UserManagement/Roles.cshtml";
                public const string Permissions = "~/Admin/Views/UserManagement/Permissions.cshtml";
                public const string RolePermissions = "~/Admin/Views/UserManagement/RolePermissions.cshtml";
                #endregion

                #region Nested Classes
                public static class User
                {
                    #region Properties
                    public const string Layout = "~/Admin/Views/UserManagement/User/UserLayout.cshtml";
                    public const string Properties = "~/Admin/Views/UserManagement/User/UserProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public static class Utilities
            {
                #region Properties
                public const string Page = "~/Admin/Views/Utilities/Utilities.cshtml";
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
                public const string Pdf = "~/Views/PdfViewer/PdfViewer.cshtml";
                #endregion
            }

            public static class Home
            {
                #region Properties
                public const string Page = "~/Views/Home/Index.cshtml";
                #endregion
            }

            public static class Test
            {
                #region Properties
                public const string Page = "~/Views/Test/Test.cshtml";
                #endregion
            }

            public static class Pages
            {
                #region Properties
                public const string Page = "~/Views/Pages/Page.cshtml";
                #endregion
            }

            public static class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Shared/Layout.cshtml";
                public const string Pager = "~/Views/Shared/Pager.cshtml";
                #endregion
            }
            #endregion
        }

        public static class Shared
        {
            #region Properties
            public const string NotFound = "~/Views/Shared/NotFound.cshtml";
            #endregion

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
