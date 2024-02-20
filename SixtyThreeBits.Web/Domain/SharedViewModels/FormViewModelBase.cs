using SixtyThreeBits.Core.Libraries;
using SixtyThreeBits.Core.Properties;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class FormViewModelBase
    {
        #region Properties        
        readonly Errors _errors = new Errors();

        public string ErrorMessage => HasErrors ? string.Join("<br />", _errors.GetErrors().Select(Item => Item.Value)) : null;
        public bool HasErrors => _errors?.Count > 0;
        public bool IsValid => !HasErrors;
        public string ErrorsJson => _errors.ErrorsJson;


        public bool IsSaved { get; set; }
        public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
        #endregion

        #region Methods
        public void AddError(string errorKey, string errorMessage)
        {

            if (!string.IsNullOrWhiteSpace(errorKey) && !string.IsNullOrWhiteSpace(errorMessage))
            {
                _errors.AddError(new ErrorItem(Key: errorKey, Value: errorMessage));
            }
        }

        public void AddError(string errorMessage)
        {
            AddError(errorKey: null, errorMessage: errorMessage);
        }

        public void AddError(ErrorItem error)
        {
            if (error != null)
            {
                AddError(errorKey: error.Key, errorMessage: error.Value);
            }
        }
        #endregion
    }
}