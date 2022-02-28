using SixtyThreeBits.Core.Modules;
using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Services
{
    public class Validation
    {
        #region Methods
        public static SimpleKeyValue<string, string> GetError(string ErrorKey, string ErrorMessage)
        {
            return new SimpleKeyValue<string, string> { Key = ErrorKey, Value = ErrorMessage };
        }

        public static SimpleKeyValue<string, string> Validate(Func<bool> ErrorAction, string ErrorKey, string ErrorMessage)
        {
            SimpleKeyValue<string, string> Error = null;
            if (ErrorAction())
            {
                Error = GetError(ErrorKey, ErrorMessage);
            }

            return Error;
        }

        public static async Task<SimpleKeyValue<string, string>> ValidateAsync(Func<Task<bool>> ErrorAction, string ErrorKey, string ErrorMessage)
        {
            SimpleKeyValue<string, string> Error = null;
            if (await ErrorAction())
            {
                Error = GetError(ErrorKey, ErrorMessage);
            }

            return Error;
        }

        public async static Task<SimpleKeyValue<string, string>> ValidateEmail(string ErrorKey, string UserEmail, bool ValidateRequired, bool ValidateUnique, int? UserID, UsersDataAccess DAL)
        {
            SimpleKeyValue<string, string> Error = null;
            if (string.IsNullOrWhiteSpace(UserEmail))
            {
                if (ValidateRequired)
                {
                    Error = GetError(ErrorKey, Resources.ValidationRequired);
                }
            }
            else
            {
                if (!Regex.IsMatch(UserEmail, Constants.RegularExpressions.Email))
                {
                    Error = GetError(ErrorKey, Resources.ValidationEmailFormatInvalid);
                }
                else if (ValidateUnique && !(await DAL.IsUserEmailUniq(UserEmail: UserEmail, UserID: UserID)))
                {
                    Error = GetError(ErrorKey, Resources.ValidationUserEmailNotUniq);
                }
            }
            return Error;
        }

        public static SimpleKeyValue<string, string> ValidatePassword(string ErrorKey, string Password)
        {
            var Error = ValidateRequired(ErrorKey: ErrorKey, ValueToValidate: Password);

            if (Error == null)
            {
                if (Password.Length < 8)
                {
                    Error = GetError(ErrorKey, Resources.ValidationPasswordLength);
                }
                else if (!(Password.Any(char.IsLetter)))
                {

                    Error = GetError(ErrorKey, Resources.ValidationPasswordStrength);
                }
                else if (!(Password.Any(char.IsDigit)))
                {
                    Error = GetError(ErrorKey, Resources.ValidationPasswordStrength);
                }
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidatePasswordRepeat(string ErrorKey, string Password, string PasswordRepeat)
        {
            SimpleKeyValue<string, string> Error = null;
            if (Password != PasswordRepeat)
            {
                Error = GetError(ErrorKey, Resources.ValidationPasswordsNotMatch);
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidateOldPassword(string ErrorKey, string UserPassword, string OldPassword)
        {
            var Error = ValidateRequired(ErrorKey, OldPassword);
            if (Error == null)
            {
                if (UserPassword != OldPassword.MD5Encrypt())
                {
                    Error = GetError(ErrorKey, Resources.ValidationPasswordOldNotMatch);
                }
            }

            return Error;
        }

        public static SimpleKeyValue<string, string> ValidateRequired(string ErrorKey, object ValueToValidate)
        {
            SimpleKeyValue<string, string> Error = null;


            if (ValueToValidate == null)
            {
                Error = GetError(ErrorKey, Resources.ValidationRequired);
            }
            else if (ValueToValidate.GetType() == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(ValueToValidate as string))
                {
                    Error = GetError(ErrorKey, Resources.ValidationRequired);
                }
            }

            return Error;
        }

        public static string GetJQueryClassSelectorFor(string Key)
        {
            return $".{Key}";
        }
        public static string GetJQueryIDSelectorFor(string Key)
        {
            return $"#{Key}";
        }
        public static string GetJQueryNameSelectorFor(string Key)
        {
            return $"[name=\"{Key}\"]";
        }
        #endregion
    }
}