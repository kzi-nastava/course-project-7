using System.Collections.Generic;
using System.Linq;
using HIS.Core.Util;

namespace HIS.Core.RoomModel
{
	public interface IRoomRepository
	{
		public void Save();
		public IEnumerable<Room> GetAll();
		public Room Get(int id);
		public Room Add(Room obj);
		public void Remove(Room obj);

		public IEnumerable<Room> GetModifiable();

		int GetNextId();

		public Room GetWarehouse()
		{
			return GetAll().FirstOrException(r => r.Type == RoomType.WAREHOUSE);
		}

		IEnumerable<Room> GetOtherModifiableOnSameFloor(Room comparedTo);
	}
}