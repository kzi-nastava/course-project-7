using HIS.Core.EquipmentModel;
using System.Collections.Generic;

namespace HIS.Core.RoomModel
{
	public interface IRoomService
	{
		public Room Add(Room r);
		public IEnumerable<Room> GetAll();
		public void Remove(Room r);
		public IEnumerable<Room> GetModifiable();
		public IEnumerable<Room> GetOtherModifiableOnSameFloor(Room comparedTo);
		public IEnumerable<Room> GetExaminationRooms();
		public void Copy(Room src, Room dest, IEnumerable<RoomProperty> properties);
		Room GetWarehouse();
		public void Move(Equipment equipment, int amount, Room src, Room dest);
	}
}
