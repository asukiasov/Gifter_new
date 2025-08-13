using System;

namespace SixtyThreeBits.Core.Utilities
{
    public static partial class Constants
    {
        public static class NullValueFor
        {
            #region Properties
            public const string String = "";
            public const int Numeric = -1;
            public static readonly DateTime Date = new(1900, 1, 1);
            #endregion
        }
    }
}
