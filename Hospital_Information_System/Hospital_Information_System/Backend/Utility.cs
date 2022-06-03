﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend
{
	static class Utility
	{
		public static List<T> GetEnumValues<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>().ToList();
		}
	}

	public class JSONRepoReferenceNullException : Exception
	{
		public JSONRepoReferenceNullException() { }
		public JSONRepoReferenceNullException(string message) : base(message) { }
		public JSONRepoReferenceNullException(string message, Exception inner) : base(message, inner) { }
		protected JSONRepoReferenceNullException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}