using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;

namespace HospitalIS.Backend.Repository
{
	internal class EquipmentJSONDictionaryConverter<V> : JsonConverter
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
