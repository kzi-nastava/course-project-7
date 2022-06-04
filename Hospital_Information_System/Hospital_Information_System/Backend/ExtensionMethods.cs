using System.Collections.Generic;
using HospitalIS.Backend.Foundation;
using System.Linq;

namespace HospitalIS.Backend
{
	public static class ExtensionMethods
	{
		public static T FirstOrException<T>(this IEnumerable<T> collection, System.Func<T, bool> predicate) where T : Entity
		{
			foreach (T obj in collection)
			{
				if (predicate(obj))
					return obj;
			}
			throw new EntityNotFoundException();
		}

		public static IEnumerable<T> UnionNullSafe<T>(this IEnumerable<T> c1, IEnumerable<T> c2)
		{
			return c1 ?? Enumerable.Empty<T>().Union(c2 ?? Enumerable.Empty<T>());
		}
	}
}