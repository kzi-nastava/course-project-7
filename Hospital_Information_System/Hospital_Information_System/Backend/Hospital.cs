using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend
{
	internal class Hospital : Entity
	{
		private static readonly JsonSerializerSettings settings;
		private static readonly string fnameRooms = "rooms.json";
		private static readonly string fnameEquipment = "equipment.json";
		private static readonly string fnameRoomHasEquipment = "roomHasEquipment.json";

		private List<Room> _rooms = new List<Room>();
		private List<Equipment> _equipment = new List<Equipment>();
		public IReadOnlyList<Room> Rooms => _rooms;
		public IReadOnlyList<Equipment> Equipment => _equipment;

		static Hospital()
		{
			settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
		}
		public void Save(string directory)
		{
			File.WriteAllText(Path.Combine(directory, fnameRooms), JsonConvert.SerializeObject(Rooms, Formatting.Indented, settings));
			File.WriteAllText(Path.Combine(directory, fnameEquipment), JsonConvert.SerializeObject(Equipment, Formatting.Indented, settings));

			// RoomHasEquipment

			Dictionary<int, List<int>> roomHasEquipment = new Dictionary<int, List<int>>();
			foreach (var r in Rooms)
			{
				roomHasEquipment[r.Id] = (from eq in r.Equipment select eq.Id).ToList();
			}
			File.WriteAllText(Path.Combine(directory, fnameRoomHasEquipment), JsonConvert.SerializeObject(roomHasEquipment, Formatting.Indented, settings));
		}
		public void Load(string directory)
		{
			_rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(Path.Combine(directory, fnameRooms)), settings);
			_equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(Path.Combine(directory, fnameEquipment)), settings);

			// RoomHasEquipment

			var roomHasEquipment = JsonConvert.DeserializeObject<Dictionary<int, List<int>>>(File.ReadAllText(Path.Combine(directory, fnameRoomHasEquipment)), settings);
			foreach (var kv in roomHasEquipment)
			{
				_rooms[kv.Key].Equipment = (from id in kv.Value select _equipment[id]).ToList();
			}
		}
		public void Add(Room room)
		{
			room.Id = Rooms.Count > 0 ? Rooms.Last().Id + 1 : 0;
			_rooms.Add(room);
		}

		public void Add(Equipment equipment)
		{
			equipment.Id = Equipment.Count > 0 ? Equipment.Last().Id + 1 : 0;
			_equipment.Add(equipment);
		}
	}
}
