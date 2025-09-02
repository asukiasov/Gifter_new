using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static class EnumerableExtensions
    {
        #region Methods
        public static bool HasElements<T>(this IEnumerable<T> collection)
        {
            return collection?.Any() == true;
        }
        #endregion
    }
}
