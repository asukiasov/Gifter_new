using SixtyThreeBits.Core.Infrastructure.Repositories.DTO;
using SixtyThreeBits.Web.Domain.Utilities;
using SixtyThreeBits.Web.Models.Base;
using System;
using System.Threading.Tasks;
using SixtyThreeBits.Libraries.Extensions;

namespace SixtyThreeBits.Web.Models.Website.Account
{
    public class AccountModel : ModelBase
    {
        #region Methods

        public async Task<UserDTO> AuthenticateGoogleUserAsync(
            string googleID,
            string email,
            string fullName,
            string firstName,
            string lastName,
            string pictureUrl)
        {
            var usersRepository = RepositoriesFactory.CreateUsersRepository();
            var user = await usersRepository.UsersGetSingleByGoogleID(googleID);

            if (user == null)
            {
                user = await usersRepository.UsersGetSingleByEmail(email);

                if (user != null)
                {
                    await usersRepository.UsersUpdateGoogleID(user.UserID.Value, googleID, pictureUrl);
                    user = await usersRepository.UsersGetSingleByID(user.UserID.Value);
                }
                else
                {
                    var userID = await usersRepository.UsersCreateFromGoogle(
                        googleID: googleID,
                        userEmail: email,
                        userFullname: fullName ?? email,
                        userFirstname: firstName ?? "",
                        userLastname: lastName ?? "",
                        avatarUrl: pictureUrl
                    );
                    user = await usersRepository.UsersGetSingleByID(userID);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pictureUrl) && user.UserAvatarFilename != pictureUrl)
                {
                    await usersRepository.UsersUpdateAvatar(user.UserID.Value, pictureUrl);
                    user = await usersRepository.UsersGetSingleByID(user.UserID.Value);
                }
            }

            return user;
        }

        public void StoreUserSession(UserDTO user)
        {
            SessionAssistance.Set(WebConstants.SessionKeys.User, user);
            // Compatibility with BaseFilter: store AES encrypted UserID in cookie
            if (user.UserID.HasValue)
            {
                CookieAssistance.Set(WebConstants.Cookies.User, user.UserID.Value.ToString().AesEncryptString(), DateTime.Now.AddDays(30));
            }
        }

        public void ClearUserSession()
        {
            SessionAssistance.Remove(WebConstants.SessionKeys.User);
            CookieAssistance.Remove(WebConstants.Cookies.User);
        }

        #endregion
    }
}
