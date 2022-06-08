using System.Collections.Generic;
using System.Linq;
using HIS.Core.Util;

namespace HIS.Core.RoomModel
{
	public interface IRoomRepository
	{
		public void Save();
		public IEnumerable<Room> Get();
		public Room Get(int id);
		public void Add(Room obj);
		public void Remove(Room obj);

		public IEnumerable<Room> GetModifiable();

		int GetNextId();

		public Room GetWarehouse()
		{
			return Get().FirstOrException(r => r.Type == RoomType.WAREHOUSE);
		}

		IEnumerable<Room> GetOtherModifiableOnSameFloor(Room comparedTo);
	}
}