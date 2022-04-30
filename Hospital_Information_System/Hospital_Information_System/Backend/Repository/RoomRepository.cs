using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
	internal class RoomRepository : IRepository<Room>
	{
		public void Add(Room entity)
		{
			List<Room> Rooms = IS.Instance.Hospital.Rooms;

			entity.Id = Rooms.Count > 0 ? Rooms.Last().Id + 1 : 0;
			Rooms.Add(entity);
		}

		public Room GetById(int id)
		{
			return IS.Instance.Hospital.Rooms.First(e => e.Id == id);
		}

		public void Load(string fullFilename, JsonSerializerSettings settings)
		{
			IS.Instance.Hospital.Rooms = JsonConvert.DeserializeObject<List<Room>>(File.ReadAllText(fullFilename), settings);
		}

		public void Remove(Room entity)
		{
			// Move all my equipment to the warehouse.
			foreach (var kv in entity.Equipment)
			{
				IS.Instance.RoomRepo.Add(GetWarehouse(), kv.Key, kv.Value);
			}
			entity.Equipment.Clear();

			// Cancel relocations that would go here.
			IS.Instance.EquipmentRelocationRepo.Remove(relocation => relocation.RoomNew == entity);
			
			entity.Deleted = true;
		}

		public void Remove(Func<Room, bool> condition)
		{
			IS.Instance.Hospital.Rooms.ForEach(entity => { if (condition(entity)) Remove(entity); });
		}

		public void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Rooms, Formatting.Indented, settings));
		}

		public void Add(Room room, Equipment equipment, int amount = 1)
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

		public void Remove(Room room, Equipment equipment)
		{
			room.Equipment[equipment]--;
			if (room.Equipment[equipment] == 0)
			{
				room.Equipment.Remove(equipment);
			}
		}

		public Room GetWarehouse()
		{
			foreach (var r in IS.Instance.Hospital.Rooms)
			{
				if (r.Type == Room.RoomType.WAREHOUSE)
					return r;
			}
			throw new WarehouseNotFoundException();
		}

		internal class RoomReferenceConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Room);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var id = serializer.Deserialize<int>(reader);
				return IS.Instance.Hospital.Rooms.First(room => room.Id == id);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, ((Room)value).Id);
			}
		}
	}
}
