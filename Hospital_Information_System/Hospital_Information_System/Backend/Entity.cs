using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
	public abstract class Entity
	{
		public int Id { get; set; } = -1;
		public bool Deleted { get; set; } = false;
	}
}
