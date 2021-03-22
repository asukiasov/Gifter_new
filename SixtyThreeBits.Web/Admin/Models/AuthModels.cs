using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables.Core;
using System;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class AuthModel : WebProjectModelBase
    {
        #region Methods
        public LoginPageViewModel GetPageViewModel(LoginPageViewModel ViewModel = null)
        {
            if (ViewModel == null)
            {
                ViewModel = new LoginPageViewModel();
            }
            ViewModel.PluginsClient = PluginsClient;
            return ViewModel;
        }

        public bool IsUserLoggedIn()
        {
            var IsLoggedIn = SessionAssistance.Get<User>(Constants.Session.User) != null;
            return IsLoggedIn;
        }

        public async Task<bool> AuthenticateUser(LoginPageViewModel ViewModel)
        {
            bool IsAuthenticated = false;

            var User = await DataAccessFactory.Users.GetSingleUserByEmailAndPassword(ViewModel.Username, ViewModel.Password);
            if (User == null)
            {
                ViewModel.IsLoginFailed = true;
            }
            else
            {
                IsAuthenticated = true;
                SessionAssistance.Set(Constants.Session.User, User);
                if (ViewModel.IsRememberMeChecked)
                {
                    CookieAssistance.Set(Constants.Cookies.User, User.UserID, DateTime.Now.AddDays(30));
                }
            }

            return IsAuthenticated;
        }

        public void Logout()
        {
            SessionAssistance.Clear();
            CookieAssistance.Remove(Constants.Cookies.User);
        }

        public async Task ReloginUser()
        {
            var SessionUser = SessionAssistance.Get<User>(Constants.Session.User);
            if (SessionUser != null)
            {
                var User = await DataAccessFactory.Users.GetSingleUserByID(SessionUser.UserID);
                if (User != null && User.UserIsActive)
                {
                    SessionAssistance.Set(Constants.Session.User, User);
                }
            }
        }
        #endregion

        #region Sub Classes
        public class LoginPageViewModel
        {
            #region Properties         
            public PluginsClient PluginsClient { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public bool IsRememberMeChecked { get; set; }
            public bool IsLoginFailed { get; set; }
            public readonly string ErrorMessage = Resources.ValidationUserInvalidUsernameOrPassword;
            public readonly string ProjectName = Constants.ProjectName;
            #endregion
        }
        #endregion
    }
}