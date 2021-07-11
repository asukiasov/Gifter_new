namespace SixtyThreeBits.Web.Reusables.Core
{
    public class ViewNames
    {
        #region Sub Classes
        public class Admin
        {
            #region Sub Classes
            public class Auth
            {
                #region Properties
                public const string Login = "~/Admin/Views/Auth/Login.cshtml";
                #endregion
            }

            public class Blog
            {
                #region Properties
                public const string Page = "~/Admin/Views/Blog/Blog.cshtml";
                public const string BlogItem = "~/Admin/Views/Blog/BlogItem.cshtml";
                #endregion
            }

            public class Brands
            {
                #region Properties
                public const string Page = "~/Admin/Views/Brands/Brands.cshtml";
                #endregion

                #region Sub Classes
                public class Brand
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/Brands/Brand/BrandProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public class Carousel
            {
                #region Properties
                public const string Page = "~/Admin/Views/Carousel/Carousel.cshtml";
                public const string CarouselItem = "~/Admin/Views/Carousel/CarouselItem.cshtml";
                #endregion               
            }

            public class Categories
            {
                #region Properties
                public const string Tree = "~/Admin/Views/Categories/CategoriesTree.cshtml";
                public const string Category = "~/Admin/Views/Categories/Category.cshtml";
                public const string CategoryProperties = "~/Admin/Views/Categories/CategoryProperties.cshtml";
                #endregion                
            }

            public class Dictionaries
            {
                #region Properties
                public const string Page = "~/Admin/Views/Dictionaries/Dictionaries.cshtml";
                #endregion
            }

            public class FacebookGalleries
            {
                #region Properties
                public const string Page = "~/Admin/Views/FacebookGalleries/FacebookGalleries.cshtml";
                #endregion

                #region Sub Classes
                public class Gallery
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/FacebookGalleries/FacebookGallery/FacebookGalleryProperties.cshtml";
                    #endregion
                }
                #endregion
            }

            public class Faq
            {
                #region Properties
                public const string Page = "~/Admin/Views/Faq/Faq.cshtml";
                #endregion
            }

            public class FileManager
            {
                #region Properties
                public const string Page = "~/Admin/Views/FileManager/FileManager.cshtml";                
                #endregion
            }

            public class Home
            {
                #region Properties
                public const string Index = "~/Admin/Views/Home/Index.cshtml";
                #endregion
            }

            public class News
            {
                #region Properties
                public const string Page = "~/Admin/Views/News/News.cshtml";
                public const string NewsItem = "~/Admin/Views/News/NewsItem.cshtml";
                #endregion
            }

            public class Pages
            {
                #region Properties
                public const string Tree = "~/Admin/Views/Pages/PagesTree.cshtml";                
                #endregion

                #region Sub Classes
                public class Page
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/Pages/Page/PageProperties.cshtml";
                    public const string Builder = "~/Admin/Views/Pages/Page/PageBuilder.cshtml";
                    public const string Text = "~/Admin/Views/Pages/Page/PageText.cshtml";
                    #endregion
                }
                #endregion
            }

            public class Products
            {
                #region Properties
                public const string Page = "~/Admin/Views/Products/Products.cshtml";
                public const string Product = "~/Admin/Views/Products/Product.cshtml";
                #endregion                
            }

            public class Projects
            {
                #region Properties
                public const string Page = "~/Admin/Views/Projects/Projects.cshtml";
                #endregion

                #region Sub Classes
                public class Project
                {
                    #region Properties
                    public const string Properties = "~/Admin/Views/Projects/Project/ProjectProperties.cshtml";
                    public const string Gallery = "~/Admin/Views/Projects/Project/ProjectGallery.cshtml";
                    #endregion
                }
                #endregion
            }

            public class ServiceLogs
            {
                #region Properties
                public const string Page = "~/Admin/Views/ExternalCommunicationServiceLog/ExternalCommunicationServiceLog.cshtml";
                #endregion
            }

            public class Shared
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

                #region Sub Classes
                public class FileTreeEditor
                {
                    #region Properties
                    public const string Editor = "~/Admin/Views/Shared/FileTreeEditor/FileTreeEditor.cshtml";                    
                    public const string File = "~/Admin/Views/Shared/FileTreeEditor/FileTreeEditorFile.cshtml";
                    #endregion
                }
                #endregion
            }

            public class SystemProperties
            {
                #region Properties
                public const string Page = "~/Admin/Views/SystemProperties/SystemProperties.cshtml"; 
                #endregion
            }

            public class UserManagement
            {
                #region Properties
                public const string Users = "~/Admin/Views/UserManagement/Users.cshtml";
                public const string MyAccount = "~/Admin/Views/UserManagement/MyAccount.cshtml";
                public const string Roles = "~/Admin/Views/UserManagement/Roles.cshtml";
                public const string Permissions = "~/Admin/Views/UserManagement/Permissions.cshtml";
                public const string RolePermissions = "~/Admin/Views/UserManagement/RolePermissions.cshtml";
                #endregion
            }

            public class Utilities
            {
                #region Properties
                public const string Page = "~/Admin/Views/Utilities/Utilities.cshtml";
                #endregion
            }
            #endregion
        }

        public class Website
        {
            #region Sub Classes
            public class Home
            {
                #region Properties
                public const string Page = "~/Views/Home/Index.cshtml";                
                #endregion
            } 

            public class FileViewer
            {
                #region Properties
                public const string Pdf = "~/Views/PdfViewer/PdfViewer.cshtml"; 
                #endregion
            }

            public class Pages
            {
                #region Properties
                public const string Page = "~/Views/Pages/Page.cshtml"; 
                #endregion
            }

            public class Shared
            {
                #region Properties
                public const string Layout = "~/Views/Shared/Layout.cshtml"; 
                #endregion
            }
            #endregion
        }

        public class Shared
        {
            #region Properties
            public const string NotFound = "~/Views/Shared/NotFound.cshtml";
            #endregion

            #region Sub Classes            
            public class FileTree
            {
                #region Properties
                public const string Tree = "~/Views/Shared/FileTree/FileTree.cshtml";
                public const string Folder = "~/Views/Shared/FileTree/FileTreeFolder.cshtml";
                public const string File = "~/Views/Shared/FileTree/FileTreeFile.cshtml";
                #endregion
            }

            public class PluginsClient
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
