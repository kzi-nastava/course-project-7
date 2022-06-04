﻿using System;

namespace HospitalIS.Backend.Foundation
{
	public abstract class Entity
	{
		public int Id { get; set; } = -1;
		public bool Deleted { get; set; }
	}

	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException() { }
		public EntityNotFoundException(string message) : base(message) { }
		public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected EntityNotFoundException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
