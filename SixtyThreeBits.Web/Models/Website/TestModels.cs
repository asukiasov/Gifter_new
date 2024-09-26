using SixtyThreeBits.Web.Models.Base;

namespace SixtyThreeBits.Web.Models.Website
{
    public class TestModel : ModelBase
    {
        #region Methods
        public ViewModel GetViewModel()
        {
            var viewModel = new ViewModel();            
            return viewModel;
        } 
        #endregion

        #region Nested Classes
        public class ViewModel
        {
            #region Properties
            public string Value { get; set; }
            #endregion
        }
        #endregion
    }
}
