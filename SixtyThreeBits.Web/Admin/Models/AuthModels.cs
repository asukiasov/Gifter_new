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
        public LoginPageViewModel GetPageViewModel()
        {
            var Model = new LoginPageViewModel();        
            return Model;
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
                CookieAssistance.Set(Constants.Cookies.User, User.UserID, DateTime.Now.AddHours(12));
            }

            return IsAuthenticated;
        }

        public void Logout()
        {
            SessionAssistance.Clear();
        }

        public async Task ReloginUser()
        {
            var User = SessionAssistance.Get<User>(Constants.Session.User);
            User = await DataAccessFactory.Users.GetSingleUserByID(User.UserID);
            if (User != null && User.UserIsActive)
            {
                SessionAssistance.Set(Constants.Session.User, User);
            }
        }
        #endregion

        #region Sub Classes
        public class LoginPageViewModel
        {            
            #region Properties            
            public string Username { get; set; }
            public string Password { get; set; }            
            public string ErrorMessage { get; set; } = Resources.ValidationUserInvalidUsernameOrPassword;
            public bool IsLoginFailed { get; set; }
            #endregion
        } 
        #endregion
    }
}