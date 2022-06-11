using System.Collections.Generic;

namespace HIS.Core.RoomModel.RenovationModel
{
	public interface IRenovationRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Renovation> GetAll();
		IEnumerable<Renovation> Get(Room whichRoom);
		public Renovation Get(int id);
		public Room Add(Renovation obj);
		public void Remove(Renovation obj);
	}
}
