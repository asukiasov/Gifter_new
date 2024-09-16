using SixtyThreeBits.Core.DTO;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Libraries.Extensions;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Domain.ViewModels.Shared;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Models.Admin
{
    public class AuthModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel(ViewModel viewModel = null)
        {
            if (viewModel == null)
            {
                viewModel = new ViewModel();
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

        public async Task<bool> AuthenticateUser(ViewModel viewModel) 
        {
            bool isAuthenticated = false;

            var repository = RepositoriesFactory.CreateUsersRepository();
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
                var repository = RepositoriesFactory.CreateUsersRepository();
                var user = await repository.UsersGetSingleByID(sessionUser.UserID);
                if (user != null && user.UserIsActive)
                {
                    SessionAssistance.Set(WebConstants.Session.User, user);
                }
            }
        }

        public void Logout()
        {
            SessionAssistance.Clear();
            CookieAssistance.Remove(WebConstants.Cookies.User);
        }
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties         
            public PluginsClientViewModel PluginsClient { get; set; }
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