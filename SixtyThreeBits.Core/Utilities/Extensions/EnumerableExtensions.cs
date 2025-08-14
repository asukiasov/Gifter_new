using System.Collections.Generic;
using System.Linq;

namespace SixtyThreeBits.Core.Utilities.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool HasElements<T>(this IEnumerable<T> collection)
        {
            return collection?.Any() == true;
        }
    }
}
