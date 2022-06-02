using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Util
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
			throw new EntityNotFoundException();
		}
	}
}