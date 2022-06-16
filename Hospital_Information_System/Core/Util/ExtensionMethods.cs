using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.Util
{
	public static class ExtensionMethods
	{
		public static T FirstOrException<T>(this IEnumerable<T> collection, System.Func<T, bool> predicate)
		{
			foreach (T obj in collection)
			{
				if (predicate(obj))
					return obj;
			}
			throw new KeyNotFoundException();
		}

		public static IEnumerable<T> UnionNullSafe<T>(this IEnumerable<T> c1, IEnumerable<T> c2)
		{
			return c1 ?? Enumerable.Empty<T>().Union(c2 ?? Enumerable.Empty<T>());
		}
	}
}