using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HospitalIS.Backend.Repository;
using System.Threading;

namespace HospitalIS.Backend
{
	internal class WarehouseNotFoundException : Exception
	{
	}
	internal class Hospital : Entity
	{
		private static Hospital _hospital;
		public static Hospital Instance { get
			{
				if (_hospital == null)
				{
					_hospital = new Hospital();
				}
				return _hospital;
			} 
		}

		private static readonly JsonSerializerSettings settings;
		private static readonly string fnameRooms = "rooms.json";
		private static readonly string fnameEquipment = "equipment.json";
		private static readonly string fnameEquipmentRelocation = "equipmentRelocation.json";

		public List<Room> Rooms = new List<Room>();
		public List<Equipment> Equipment = new List<Equipment>();
		public List<EquipmentRelocation> EquipmentRelocations = new List<EquipmentRelocation>();
		public List<Thread> EquipmentRelocationTasks = new List<Thread>();

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
			File.WriteAllText(Path.Combine(directory, fnameEquipment), JsonConvert.SerializeObject(Equipment, Formatting.Indented, settings));
			File.WriteAllText(Path.Combine(directory, fnameRooms), JsonConvert.SerializeObject(Rooms, Formatting.Indented, settings));
			EquipmentRelocationRepository.Save(this, Path.Combine(directory, fnameEquipmentRelocation), settings);
		}
		public void Load(string directory)
		{
			Equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(Path.Combine(directory, fnameEquipment)), settings);
			Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(Path.Combine(directory, fnameRooms)), settings);
			EquipmentRelocations = EquipmentRelocationRepository.Load(this, Path.Combine(directory, fnameEquipmentRelocation), settings);

			var now = DateTime.Now;
			foreach (var relocation in EquipmentRelocations)
			{
				AddEquipmentRelocationTask(relocation);
			}
		}
		public void Add(Room room)
		{
			room.Id = Rooms.Count > 0 ? Rooms.Last().Id + 1 : 0;
			Rooms.Add(room);
		}

		public void Add(Equipment equipment)
		{
			equipment.Id = Equipment.Count > 0 ? Equipment.Last().Id + 1 : 0;
			Equipment.Add(equipment);
		}

		public void Add(EquipmentRelocation equipmentRelocation)
		{
			equipmentRelocation.Id = EquipmentRelocations.Count > 0 ? EquipmentRelocations.Last().Id + 1 : 0;
			EquipmentRelocations.Add(equipmentRelocation);

			AddEquipmentRelocationTask(equipmentRelocation);
		}

		public void Remove(Equipment equipment)
		{
			equipment.Deleted = true;

			// Remove all equipment relocations that move this equipment.
			EquipmentRelocations.ForEach(er => { if (er.Equipment == equipment) { Remove(er); } });
		}

		public void Remove(Room room)
		{
			room.Deleted = true;

			// Move all equipment from this room to the warehouse.
			foreach (var kv in room.Equipment)
			{
				RoomRepository.AddEquipment(GetWarehouse(), kv.Key, kv.Value);
			}
			room.Equipment.Clear();

			// Remove all equipment relocations that move some equipment to this room.
			EquipmentRelocations.ForEach(er => { if (er.RoomNew == room) { Remove(er); } });
		}

		public void Remove(EquipmentRelocation equipmentRelocation)
		{
			equipmentRelocation.Deleted = true;
		}

		protected void AddEquipmentRelocationTask(EquipmentRelocation equipmentRelocation)
		{
			Thread t = new Thread(new ThreadStart(() => EquipmentRelocationRepository.PerformRelocation(this, equipmentRelocation)));
			EquipmentRelocationTasks.Add(t);
			t.Start();
		}
	}
}
