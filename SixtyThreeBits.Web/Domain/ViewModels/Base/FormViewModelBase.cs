using SixtyThreeBits.Core.Libraries.Validation;
using SixtyThreeBits.Core.Properties;
using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Web.Domain.ViewModels.Base
{
    public class FormViewModelBase
    {
        #region Properties        
        readonly ValidationResult63 _formErrors = new ValidationResult63();

        public string FormErrorsJson => _formErrors.ErrorsJson;
        public string FormErrorsMessage => HasFormErrors ? string.Join("<br />", _formErrors.GetErrors().Select(Item => Item.Value)) : null;
        public bool HasFormErrors => _formErrors.Count > 0;

        public string ToastError { get; private set; }
        public bool HasToastError { get; private set; }

        public bool HasErrors => HasFormErrors || HasToastError;

        public readonly string TextConfirmDelete = Resources.TextConfirmDelete;
        #endregion

        #region Methods
        public void AddToastError(string errorMessage)
        {
            ToastError = errorMessage;
            HasToastError = true;
        }

        public void AddFormErrors(IEnumerable<Error63> formErrors)
        {
            foreach (var error in formErrors)
            {
                _formErrors.AddError(error);
            }
        }      
        #endregion
    }
}