using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.RoomModel.RenovationModel
{
	public interface IRenovationService
	{
		public Renovation Add(Renovation r);
		public void Remove(Renovation r);
		public IEnumerable<Renovation> GetAll();
		public IEnumerable<Renovation> GetAll(Room r);
		bool IsRenovating(Room room, DateTime start, DateTime end);
	}
}
