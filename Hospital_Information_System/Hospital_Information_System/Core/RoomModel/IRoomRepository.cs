using System.Collections.Generic;
using System.Linq;
using HospitalIS.Core.Util;

namespace HospitalIS.Core.RoomModel
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
			return Get().Where(r => r.Type != RoomType.WAREHOUSE);
		}

		public Room GetWarehouse()
		{
			return Get().FirstOrException(r => r.Type == RoomType.WAREHOUSE);
		}
	}
}