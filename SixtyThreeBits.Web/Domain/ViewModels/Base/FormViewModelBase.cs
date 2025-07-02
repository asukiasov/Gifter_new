using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.ViewModels.Base
{
    public class FormViewModelBase
    {
        #region Properties        
        readonly ValidationResult63 _validationResult = new ValidationResult63();

        public string ErrorMessage => HasErrors ? string.Join("<br />", _validationResult.GetErrors().Select(Item => Item.Value)) : null;
        public bool HasErrors => _validationResult?.Count > 0;
        public bool IsValid => !HasErrors;
        public string ErrorsJson => _validationResult.ErrorsJson;
        
        public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
        #endregion

        #region Methods
        public void AddError(string errorKey, string errorMessage)
        {

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                _validationResult.AddError(new Error63(Key: errorKey, Value: errorMessage));
            }
        }

        public void AddError(string errorMessage)
        {
            AddError(errorKey: null, errorMessage: errorMessage);
        }

        public void AddError(Error63 error)
        {
            if (error != null)
            {
                AddError(errorKey: error.Key, errorMessage: error.Value);
            }
        }
        #endregion
    }
}