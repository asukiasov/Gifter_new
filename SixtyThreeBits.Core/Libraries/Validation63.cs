using SixtyThreeBits.Core.Properties;
using SixtyThreeBits.Core.Utilities;
using SixtyThreeBits.Libraries.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SixtyThreeBits.Core.Libraries
{
    public class Validation63
    {
        #region Methods
        public static bool IsEmailFormatValid(string email)
        {
            var IsValid = false;
            if (!string.IsNullOrWhiteSpace(email))
            {
                IsValid = Regex.IsMatch(email, Constants.RegularExpressions.Email);
            }
            return IsValid;
        }

        public static Error63 GetError(string errorKey, string errorMessage)
        {
            return new Error63(Key: errorKey, Value: errorMessage);
        }

        public static Error63 Validate(Func<bool> errorAction, string errorKey, string errorMessage)
        {
            Error63 Error = null;
            if (errorAction())
            {
                Error = GetError(errorKey, errorMessage);
            }

            return Error;
        }

        public static async Task<Error63> ValidateAsync(Func<Task<bool>> errorAction, string errorKey, string errorMessage)
        {
            Error63 Error = null;
            if (await errorAction())
            {
                Error = GetError(errorKey, errorMessage);
            }

            return Error;
        }

        public async static Task<Error63> ValidateEmail(string errorKey, string userEmail, bool validateRequired, bool validateUnique = false, Func<Task<bool>> validationPredicateReturnTrueWhenError = null)
        {
            Error63 error = null;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                if (validateRequired)
                {
                    error = GetError(errorKey, Resources.ValidationRequired);
                }
            }
            else
            {
                if (!Regex.IsMatch(userEmail, Constants.RegularExpressions.Email))
                {
                    error = GetError(errorKey, Resources.ValidationEmailFormatInvalid);
                }
                else if (validateUnique)
                {
                    var isError = await validationPredicateReturnTrueWhenError();
                    if (isError)
                    {
                        error = GetError(errorKey, Resources.ValidationUserEmailNotUniq);
                    }
                }
            }
            return error;
        }

        public static Error63 ValidatePassword(string errorKey, string password)
        {
            var error = ValidateRequired(errorKey: errorKey, valueToValidate: password);

            if (error == null)
            {
                if (password.Length < 8)
                {
                    error = GetError(errorKey, Resources.ValidationPasswordLength);
                }
                else if (!password.Any(char.IsLetter))
                {

                    error = GetError(errorKey, Resources.ValidationPasswordStrength);
                }
                else if (!password.Any(char.IsDigit))
                {
                    error = GetError(errorKey, Resources.ValidationPasswordStrength);
                }
            }

            return error;
        }

        public static Error63 ValidatePasswordRepeat(string errorKey, string password, string passwordRepeat)
        {
            Error63 error = null;
            if (password != passwordRepeat)
            {
                error = GetError(errorKey, Resources.ValidationPasswordsNotMatch);
            }

            return error;
        }

        public static Error63 ValidateOldPassword(string errorKey, string userPassword, string oldPassword)
        {
            var error = ValidateRequired(errorKey, oldPassword);
            if (error == null)
            {
                if (userPassword != oldPassword.MD5Encrypt())
                {
                    error = GetError(errorKey, Resources.ValidationPasswordOldNotMatch);
                }
            }

            return error;
        }

        public static Error63 ValidateRequired(string errorKey, object valueToValidate)
        {
            Error63 error = null;


            if (valueToValidate == null)
            {
                error = GetError(errorKey, Resources.ValidationRequired);
            }
            else if (valueToValidate.GetType() == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(valueToValidate as string))
                {
                    error = GetError(errorKey, Resources.ValidationRequired);
                }
            }

            return error;
        }

        public static string GetJQueryClassSelectorFor(string key)
        {
            return $".{key}";
        }
        public static string GetJQueryIDSelectorFor(string key)
        {
            return $"#{key}";
        }
        public static string GetJQueryNameSelectorFor(string key)
        {
            return $"[name=\"{key}\"]";
        }
        #endregion
    }

    public class ValidationResult63
    {
        #region Properties
        readonly List<Error63> _errors = [];
        public IReadOnlyList<Error63> Errors => _errors.AsReadOnly();
        public int Count => _errors.Count;
        public bool HasErrors => _errors.Any();
        public string ErrorsJson => _errors.ToJson();
        public string ErrorMessage => string.Join(", ", _errors.Select(item => item.Value));
        #endregion

        #region Methods
        public void AddError(string errorKey, string errorMessage)
        {

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _errors.Add(new Error63(Key: errorKey, Value: errorMessage));
            }
        }

        public void AddError(string errorMessage)
        {
            AddError(errorKey: null, errorMessage: errorMessage);
        }

        public void AddError(Error63 errorItem)
        {
            if (errorItem != null)
            {
                AddError(errorKey: errorItem.Key, errorMessage: errorItem.Value);
            }
        }

        public ReadOnlyCollection<Error63> GetErrors()
        {
            return _errors.AsReadOnly();
        }
        #endregion
    }

    public record Error63(string Key, string Value)
    {

    }
}