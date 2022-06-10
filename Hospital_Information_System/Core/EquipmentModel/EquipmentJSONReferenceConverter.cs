using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.EquipmentModel
{
	public class EquipmentJSONReferenceConverter : JsonConverter
	{
		internal static IEquipmentRepository Repo { get; set; }

		public EquipmentJSONReferenceConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Equipment);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var equipmentID = serializer.Deserialize<int>(reader);
			return Repo.GetAll().First(eq => eq.Id == equipmentID);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((Equipment)value).Id);
		}
	}
}
