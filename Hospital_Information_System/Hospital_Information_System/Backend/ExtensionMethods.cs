using System.Collections.Generic;
using HospitalIS.Backend.Foundation;

namespace HospitalIS.Backend.Util
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
	}
}