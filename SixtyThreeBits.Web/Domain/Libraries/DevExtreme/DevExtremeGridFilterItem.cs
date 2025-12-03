namespace SixtyThreeBits.Web.Domain.Libraries.DevExtreme
{
    public class DevExtremeGridFilterItem
    {
        #region Properties
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public bool IsNegation { get; set; }
        #endregion
    }    
}