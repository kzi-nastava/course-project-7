using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
	class EquipmentRepository : IRepository<Equipment>
	{
		public void Add(Equipment entity)
		{
			List<Equipment> Equipment = IS.Instance.Hospital.Equipment;

			entity.Id = Equipment.Count > 0 ? Equipment.Last().Id + 1 : 0;
			Equipment.Add(entity);
		}

		public Equipment GetById(int id)
		{
			return IS.Instance.Hospital.Equipment.First(e => e.Id == id);
		}

		public void Load(string fullFilename, JsonSerializerSettings settings)
		{
			IS.Instance.Hospital.Equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(fullFilename), settings);
		}

		public void Remove(Equipment entity)
		{
			entity.Deleted = true;

			IS.Instance.EquipmentRelocationRepo.Remove(relocation => relocation.Equipment == entity);
		}

		public void Remove(Func<Equipment, bool> condition)
		{
			IS.Instance.Hospital.Equipment.ForEach(entity => { if (condition(entity)) Remove(entity); });
		}

		public void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Equipment, Formatting.Indented, settings));
		}

		internal class EquipmentReferenceConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Equipment);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var equipmentID = serializer.Deserialize<int>(reader);
				return IS.Instance.Hospital.Equipment.First(eq => eq.Id == equipmentID);
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				serializer.Serialize(writer, ((Equipment)value).Id);
			}
		}

		internal class EquipmentDictionaryConverter<V> : JsonConverter
		{
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				var writableDict = new Dictionary<int, V>();
				foreach (var kv in (Dictionary<Equipment, V>)value)
				{
					writableDict[kv.Key.Id] = kv.Value;
				}

				serializer.Serialize(writer, writableDict);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var readableDict = serializer.Deserialize<Dictionary<int, V>>(reader);
				var result = new Dictionary<Equipment, V>();
				foreach (var kv in readableDict)
				{
					Equipment eq = IS.Instance.Hospital.Equipment.First(eq => eq.Id == kv.Key);
					result[eq] = kv.Value;
				}
				return result;
			}

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Dictionary<Equipment, V>);
			}
		}
	}
}
