using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Room
{
	internal class RoomService
	{
		private readonly IRoomRepository _repo;
		public RoomService(IRoomRepository repo)
		{
			_repo = repo;
		}

		public void Add(Room r)
		{
			r.Id = (_repo.Get().LastOrDefault()?.Id ?? -1) + 1;
			_repo.Add(r);
		}

		public List<Room> Get()
		{
			return _repo.Get().ToList();
		}

		public void Remove(Room r)
		{
			Room warehouse = _repo.GetWarehouse();
			foreach (var eq in r.Equipment)
			{
				Move(eq.Key, eq.Value, r, warehouse);
			}

			// Cancel relocations that would go here
			// Remove all renovations for that room

			r.Deleted = true;
		}

		private void Move(Equipment equipment, int amount, Room src, Room dest)
		{
			if (dest.Equipment.ContainsKey(equipment))
			{
				dest.Equipment[equipment] += amount;
			}
			else
			{
				dest.Equipment[equipment] = amount;
			}
			src.Equipment[equipment] -= amount;
		}

		public List<Room> GetModifiable()
		{
			return _repo.GetModifiable().ToList();
		}

		public void Copy(Room src, Room dest, IEnumerable<Room.RoomProperty> properties)
		{
			if (properties.Contains(Room.RoomProperty.NAME)) dest.Name = src.Name;
			if (properties.Contains(Room.RoomProperty.FLOOR)) dest.Floor = src.Floor;
			if (properties.Contains(Room.RoomProperty.TYPE)) dest.Type = src.Type;
		}
	}
}
