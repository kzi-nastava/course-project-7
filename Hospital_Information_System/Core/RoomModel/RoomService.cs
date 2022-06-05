using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.EquipmentModel;

namespace HIS.Core.RoomModel
{
	public class RoomService : IRoomService
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

		public IEnumerable<Room> Get()
		{
			return _repo.Get();
		}

		public IEnumerable<Room> GetModifiable()
		{
			return _repo.GetModifiable();
		}

		public void Remove(Room r)
		{
			Room warehouse = _repo.GetWarehouse();
			foreach (var eq in r.Equipment)
			{
				Move(eq.Key, eq.Value, r, warehouse);
			}

			// todo Cancel relocations that would go here
			// todo Remove all renovations for that room

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

		public void Copy(Room src, Room dest, IEnumerable<RoomProperty> properties)
		{
			if (properties.Contains(RoomProperty.NAME)) dest.Name = src.Name;
			if (properties.Contains(RoomProperty.FLOOR)) dest.Floor = src.Floor;
			if (properties.Contains(RoomProperty.TYPE)) dest.Type = src.Type;
			if (properties.Contains(RoomProperty.EQUIPMENT)) dest.Equipment = src.Equipment.ToDictionary(e => e.Key, e => e.Value);
		}
	}
}
