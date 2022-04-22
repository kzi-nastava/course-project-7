using System;

namespace HospitalIS.Backend
{
	public class EntityNotFoundException : Exception
	{
	}
	public abstract class Entity
	{
		public int Id { get; set; } = -1;
		public bool Deleted { get; set; } = false;
	}
}
