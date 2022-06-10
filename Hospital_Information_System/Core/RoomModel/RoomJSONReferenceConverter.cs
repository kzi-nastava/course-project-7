using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.RoomModel
{
	public class RoomJSONReferenceConverter : JsonConverter
	{
		internal static IRoomRepository Repo { get; set; }

		public RoomJSONReferenceConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Room);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			try
			{
				var equipmentID = serializer.Deserialize<int>(reader);
				return Repo.GetAll().First(eq => eq.Id == equipmentID);
			}
			catch
			{
				return null;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((Room)value).Id);
		}
	}
}
