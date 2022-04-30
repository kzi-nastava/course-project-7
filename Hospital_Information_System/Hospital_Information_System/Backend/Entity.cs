using System;

namespace HospitalIS.Backend
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException() : base()
		{
		}

		public EntityNotFoundException(string message) : base(message)
		{
		}

		public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}

	public abstract class Entity
	{
		public int Id { get; set; } = -1;
		public bool Deleted { get; set; }
	}
}
