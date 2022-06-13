using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.MedicationModel
{
	public class MedicationJSONReferenceConverter : JsonConverter
	{
		internal static IMedicationRepository Repo { get; set; }

		public MedicationJSONReferenceConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Medication);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var patientID = serializer.Deserialize<int>(reader);
			return Repo.GetAll().First(eq => eq.Id == patientID);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((Medication)value).Id);
		}
	}
}
