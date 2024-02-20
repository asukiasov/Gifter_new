namespace SixtyThreeBits.Web.Domain.SharedViewModels
{
    public class SuccessErrorPartialViewModel
    {
        #region Properties
        public bool IsTop { set; get; }
        public string Message { set; get; }
        public bool ShowError { set; get; }
        public bool ShowSuccess { set; get; }
        #endregion
    }
}
