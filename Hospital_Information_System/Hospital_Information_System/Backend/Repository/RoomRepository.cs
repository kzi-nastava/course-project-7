using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
	internal static class RoomRepository
	{
		internal class RoomReferenceConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Room);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var roomID = serializer.Deserialize<int>(reader);
				return Hospital.Instance.Rooms.First(room => room.Id == roomID);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, ((Room)value).Id);
			}
		}

		public static void Load(string fullFilename, JsonSerializerSettings settings)
		{
			Hospital.Instance.Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(fullFilename), settings);
		}

		public static void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(Hospital.Instance.Rooms, Formatting.Indented, settings));
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

		public static void RemoveEquipment(Room room, Equipment equipment)
		{
			room.Equipment[equipment]--;
			if (room.Equipment[equipment] == 0)
			{
				room.Equipment.Remove(equipment);
			}
		}
	}
}
