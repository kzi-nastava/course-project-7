using System.Collections.Generic;

namespace HIS.Core.RoomModel
{
	public interface IRoomService
	{
		public void Add(Room r);
		public IEnumerable<Room> Get();
		public void Remove(Room r);
		public IEnumerable<Room> GetModifiable();
		public void Copy(Room src, Room dest, IEnumerable<RoomProperty> properties);
	}
}
