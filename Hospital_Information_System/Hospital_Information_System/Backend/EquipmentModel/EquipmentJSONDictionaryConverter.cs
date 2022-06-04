using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.EquipmentModel
{
	internal class EquipmentJSONDictionaryConverter<T> : JsonConverter
	{
		internal static IEquipmentRepository Repo { get; set; }

		public EquipmentJSONDictionaryConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var writableDict = new Dictionary<int, T>();
			foreach (var kv in (Dictionary<Equipment, T>)value)
			{
				writableDict[kv.Key.Id] = kv.Value;
			}

			serializer.Serialize(writer, writableDict);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
		{
			var readableDict = serializer.Deserialize<Dictionary<int, T>>(reader);
			var result = new Dictionary<Equipment, T>();
			foreach (var kv in readableDict)
			{
				Equipment eq = Repo.Get().First(eq => eq.Id == kv.Key);
				result[eq] = kv.Value;
			}
			return result;
		}

		public override bool CanConvert(System.Type objectType)
		{
			return objectType == typeof(Dictionary<Equipment, T>);
		}
	}
}
