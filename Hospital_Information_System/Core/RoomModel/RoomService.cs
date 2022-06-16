using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.EquipmentModel;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;

namespace HIS.Core.RoomModel
{
	public class RoomService : IRoomService
	{
		private readonly IRoomRepository _repo;

		public RoomService(IRoomRepository repo)
		{
			_repo = repo;
		}

		public Room Add(Room r)
		{
			_repo.Add(r);
			return r;
		}

		public IEnumerable<Room> GetAll()
		{
			return _repo.GetAll();
		}

		public IEnumerable<Room> GetModifiable()
		{
			return _repo.GetModifiable();
		}

		public Room GetWarehouse()
		{
			return _repo.GetWarehouse();
		}

		public void Remove(Room r)
		{
			// todo: Remove relocations from/to this room...?
			Room warehouse = _repo.GetWarehouse();
			for (int i = 0; i < r.Equipment.Count; i++)
			{
				var kv = r.Equipment.ElementAt(i);
				Move(kv.Key, kv.Value, r, warehouse);
			}

			r.Deleted = true;
		}

		public void Move(Equipment equipment, int amount, Room src, Room dest)
		{
			if (dest.Equipment.ContainsKey(equipment))
			{
				dest.Equipment[equipment] += amount;
			}
			else
			{
				dest.Equipment[equipment] = amount;
			}
			if (src != null)
				src.Equipment[equipment] -= amount;
		}

		public void Copy(Room src, Room dest, IEnumerable<RoomProperty> properties)
		{
			if (properties.Contains(RoomProperty.NAME)) dest.Name = src.Name;
			if (properties.Contains(RoomProperty.FLOOR)) dest.Floor = src.Floor;
			if (properties.Contains(RoomProperty.TYPE)) dest.Type = src.Type;
			if (properties.Contains(RoomProperty.EQUIPMENT)) dest.Equipment = src.Equipment.ToDictionary(e => e.Key, e => e.Value);
		}

		public IEnumerable<Room> GetOtherModifiableOnSameFloor(Room comparedTo)
		{
			return _repo.GetOtherModifiableOnSameFloor(comparedTo);
		}

        public IEnumerable<Room> GetExaminationRooms()
		{
			return _repo.GetExaminationRooms();
		}
    }
}
