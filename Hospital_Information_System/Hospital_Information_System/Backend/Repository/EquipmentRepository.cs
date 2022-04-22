using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
	internal static class EquipmentRepository
	{
		internal class EquipmentDictionary<V> : JsonConverter
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
	}
}
