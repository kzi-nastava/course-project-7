using System;
using System.Collections.Generic;
using System.Text;

namespace Hospital_Information_System.Backend
{
	public abstract class Entity
	{
		public bool Deleted { get; set; } = false;
	}
}
