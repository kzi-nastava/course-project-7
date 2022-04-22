using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Backend.Repository
{
	internal static class RoomRepository
	{
		public static void Load(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			hospital.Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(fullFilename), settings);
		}

		public static void Save(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(hospital.Rooms, Formatting.Indented, settings));
		}

		public static void AddEquipment(Room room, Equipment equipment, int amount = 1)
		{
			if (room.Equipment.ContainsKey(equipment))
			{
				room.Equipment[equipment] += amount;
			}
			else
			{
				room.Equipment[equipment] = amount;
			}
		}
	}
}
