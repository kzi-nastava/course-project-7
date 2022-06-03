using HospitalIS.Backend.Util;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Room
{
	public interface IRoomRepository
	{
		public void Save();
		public IEnumerable<Room> Get();
		public Room Get(int id);
		public void Add(Room obj);
		public void Remove(Room obj);

		public IEnumerable<Room> GetModifiable()
		{
			return Get().Where(r => r.Type != Room.RoomType.WAREHOUSE);
		}

		public Room GetWarehouse()
		{
			return Get().FirstOrException(r => r.Type == Room.RoomType.WAREHOUSE);
		}
	}
}