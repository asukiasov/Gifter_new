using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Web.Reusables.Core;
using System.Threading.Tasks;

namespace SixtyThreeBits.Web.Admin.Models
{
    public class AuthModel : WebProjectModelBase
    {        
        #region Methods
        public LoginPageViewModel GetPageViewModel()
        {
            var Model = new LoginPageViewModel();
            //Model.IsLoginFailed = true;
            //var SessionData = SessionAssistance.Get<string>("SomeData");
            //if(SessionData == null)
            //{
            //    SessionData = System.Guid.NewGuid().ToString();
            //    SessionAssistance.Set<string>("SomeData", SessionData);
            //}
            //Model.ErrorMessage = SessionData; 
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
            }

            return IsAuthenticated;
        }

        public void Logout()
        {
            SessionAssistance.Clear();
        }

        public void ReloginUser()
        {
            var User = SessionAssistance.Get<User>(Constants.Session.User);
            //User = UsersDataAccess.GetSingleUserByID(UserID: User?.UserID);
            //if (User != null && User.UserIsActive)
            //{
            //    SessionAssistance.SetUser(Session, User);
            //}
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