namespace SixtyThreeBits.Core.Infrastructure.Utilities
{
    public class ValueWrapper<T> where T : struct
    {
        #region Properties
        private T _value;
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        #endregion                
    }
}
