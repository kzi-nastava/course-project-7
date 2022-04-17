using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
	public abstract class Entity
	{
		public bool Deleted { get; set; } = false;
	}
}
