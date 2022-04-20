using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Backend
{
	internal class WarehouseNotFoundException : Exception
	{
	}
	internal class Hospital : Entity
	{
		private static readonly JsonSerializerSettings settings;
		private static readonly string fnameRooms = "rooms.json";
		private static readonly string fnameEquipment = "equipment.json";
		private static readonly string fnameRoomHasEquipment = "roomHasEquipment.json";
		private static readonly string fnameEquipmentRelocation = "equipmentRelocation.json";

		private List<Room> _rooms = new List<Room>();
		private List<Equipment> _equipment = new List<Equipment>();
		private List<EquipmentRelocation> _equipmentRelocations = new List<EquipmentRelocation>();
		public IReadOnlyList<Room> Rooms => (from o in _rooms where !o.Deleted select o).ToList();
		public IReadOnlyList<Equipment> Equipment => (from o in _equipment where !o.Deleted select o).ToList();
		public IReadOnlyList<EquipmentRelocation> EquipmentRelocations => (from o in _equipmentRelocations where !o.Deleted select o).ToList();

		public IReadOnlyList<Room> RoomsAll() { return _rooms; }
		public IReadOnlyList<Equipment> EquipmentAll() { return _equipment; }
		public IReadOnlyList<EquipmentRelocation> EquipmentRelocationsAll() { return _equipmentRelocations; }

		public Room GetWarehouse()
		{
			foreach (var r in Rooms)
			{
				if (r.Type == Room.RoomType.WAREHOUSE)
					return r;
			}
			throw new WarehouseNotFoundException();
		}

		static Hospital()
		{
			settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
		}
		public void Save(string directory)
		{
			File.WriteAllText(Path.Combine(directory, fnameRooms), JsonConvert.SerializeObject(_rooms, Formatting.Indented, settings));
			File.WriteAllText(Path.Combine(directory, fnameEquipment), JsonConvert.SerializeObject(_equipment, Formatting.Indented, settings));
			EquipmentRelocationRepository.Save(this, Path.Combine(directory, fnameEquipmentRelocation), settings);
			RoomHasEquipmentRepository.Save(this, Path.Combine(directory, fnameRoomHasEquipment), settings);
		}
		public void Load(string directory)
		{
			_rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(Path.Combine(directory, fnameRooms)), settings);
			_equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(Path.Combine(directory, fnameEquipment)), settings);
			_equipmentRelocations = EquipmentRelocationRepository.Load(this, Path.Combine(directory, fnameEquipmentRelocation), settings);
			RoomHasEquipmentRepository.Load(this, Path.Combine(directory, fnameRoomHasEquipment), settings);
		}
		public void Add(Room room)
		{
			room.Id = _rooms.Count > 0 ? _rooms.Last().Id + 1 : 0;
			_rooms.Add(room);
		}

		public void Add(Equipment equipment)
		{
			equipment.Id = _equipment.Count > 0 ? _equipment.Last().Id + 1 : 0;
			_equipment.Add(equipment);
		}

		public void Add(EquipmentRelocation equipmentRelocation)
		{
			equipmentRelocation.Id = _equipmentRelocations.Count > 0 ? _equipmentRelocations.Last().Id + 1 : 0;
			_equipmentRelocations.Add(equipmentRelocation);
		}
	}
}
