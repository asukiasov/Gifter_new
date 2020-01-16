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
            var Model = new LoginPageViewModel
            {
                ProjectName = AppSettings.ProjectName
            };

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
            var DAL = new UsersDataAccess(db);
            var User = await DAL.GetSingleUserByEmailAndPassword(ViewModel.Username, ViewModel.Password);
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
            public string ProjectName { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }            
            public string ErrorMessage { get; set; } = Resources.ValidationUserInvalidUsernameOrPassword;
            public bool IsLoginFailed { get; set; }
            #endregion
        } 
        #endregion
    }
}