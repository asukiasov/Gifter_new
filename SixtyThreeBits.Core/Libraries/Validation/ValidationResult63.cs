using SixtyThreeBits.Libraries.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SixtyThreeBits.Core.Libraries.Validation
{
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
                _errors.Add(new Error63 { Key = errorKey, Value = errorMessage });
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
}