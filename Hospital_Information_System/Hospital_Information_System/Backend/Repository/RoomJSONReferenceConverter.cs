using System;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
	internal class RoomJSONReferenceConverter : JsonConverter
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
