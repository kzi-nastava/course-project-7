using System;
using Newtonsoft.Json;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
	internal class EquipmentJSONReferenceConverter : JsonConverter
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
}
