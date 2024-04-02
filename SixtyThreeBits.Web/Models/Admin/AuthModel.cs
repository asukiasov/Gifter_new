using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain;
using SixtyThreeBits.Web.Domain.SharedViewModels;
using SixtyThreeBits.Web.Models.Shared;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class LoginModel : ModelBase
    {
        #region Methods
        public PageViewModel GetPageViewModel(PageViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new PageViewModel();
            }
            viewModel.PluginsClient = PluginsClient;
            viewModel.ProjectName = SystemProperties.ProjectName;
            return viewModel;
        }

        public bool IsUserLoggedIn()
        {
            var isLoggedIn = SessionAssistance.Get<UserDTO>(key: WebConstants.Session.User) != null;
            return isLoggedIn;
        }

        public async Task<bool> AuthenticateUser(PageViewModel viewModel) 
        {
            bool isAuthenticated = false;

            var repository = RepositoriesFactory.GetUsersRepository();
            var user = await repository.UsersGetSingleByEmailAndPassword(userEmail: viewModel.Username, userPassword: viewModel.Password);
            if (user == null)
            {
                viewModel.IsLoginFailed = true;
            }
            else
            {
                isAuthenticated = true;
                SessionAssistance.Set(key: WebConstants.Session.User, value: user);
                if (viewModel.IsRememberMeChecked)
                {
                    CookieAssistance.Set(
                        key: WebConstants.Cookies.User, 
                        value: user.UserID.ToString().AesEncryptString(), 
                        expirationDate: DateTime.Now.AddDays(30)
                    );
                }
            }

            return isAuthenticated;
        }

        public async Task ReloginUser()
        {
            var sessionUser = SessionAssistance.Get<UserDTO>(WebConstants.Session.User);
            if (sessionUser != null)
            {
                var repository = RepositoriesFactory.GetUsersRepository();
                var user = await repository.UsersGetSingleByID(sessionUser.UserID);
                if (user != null && user.UserIsActive)
                {
                    SessionAssistance.Set(WebConstants.Session.User, user);
                }
            }
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties         
            public PluginsClient PluginsClient { get; set; }
            public string ProjectName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsRememberMeChecked { get; set; }
            public bool IsLoginFailed { get; set; }
            public readonly string TextAdminWelcomeTitle = Resources.TextAdminWelcomeTitle;
            public readonly string TextAdminWelcomeSubTitle = Resources.TextAdminWelcomeSubTitle;
            public readonly string TextUsername = Resources.TextUsername;
            public readonly string TextPassword = Resources.TextPassword;
            public readonly string TextRememberMe = Resources.TextRememberMe;
            public readonly string TextLogin = Resources.TextLogin;
            public readonly string ValidationUserInvalidUsernameOrPassword = Resources.ValidationUserInvalidUsernameOrPassword;
            #endregion
        }
        #endregion
    }

    public class LogoutModel : ModelBase
    {
        #region Methods
        public void Logout()
        {
            SessionAssistance.Clear();
            CookieAssistance.Remove(WebConstants.Cookies.User);
        }
        #endregion

        #region Nested Classes
        public class PageViewModel
        {
            #region Properties         
            public PluginsClient PluginsClient { get; set; }
            public string ProjectName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsRememberMeChecked { get; set; }
            public bool IsLoginFailed { get; set; }
            public readonly string TextAdminWelcomeTitle = Resources.TextAdminWelcomeTitle;
            public readonly string TextAdminWelcomeSubTitle = Resources.TextAdminWelcomeSubTitle;
            public readonly string TextUsername = Resources.TextUsername;
            public readonly string TextPassword = Resources.TextPassword;
            public readonly string TextRememberMe = Resources.TextRememberMe;
            public readonly string TextLogin = Resources.TextLogin;
            public readonly string ValidationUserInvalidUsernameOrPassword = Resources.ValidationUserInvalidUsernameOrPassword;
            #endregion
        }
        #endregion
    }
}