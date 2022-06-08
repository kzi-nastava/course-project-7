using System.Collections.Generic;

namespace HIS.Core.RoomModel.RenovationModel
{
	public interface IRenovationRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Renovation> Get();
		IEnumerable<Renovation> Get(Room r);
		public Renovation Get(int id);
		public void Add(Renovation obj);
		public void Remove(Renovation obj);
	}
}
