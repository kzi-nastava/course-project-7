using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
	static class EquipmentRepository
	{
		internal class EquipmentReferenceConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Equipment);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				var equipmentID = serializer.Deserialize<int>(reader);
				return Hospital.Instance.Equipment.First(eq => eq.Id == equipmentID);
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
					Equipment eq = Hospital.Instance.Equipment.First(eq => eq.Id == kv.Key);
					result[eq] = kv.Value;
				}
				return result;
			}

			public override bool CanConvert(Type objectType)
			{
				return objectType == typeof(Dictionary<Equipment, V>);
			}
		}

		internal static void Load(string fullFilename, JsonSerializerSettings settings)
		{
			Hospital.Instance.Equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(fullFilename), settings);
		}

		internal static void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(Hospital.Instance.Equipment, Formatting.Indented, settings));
		}
	}
}
