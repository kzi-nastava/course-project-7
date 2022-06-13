using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.AppointmentModel
{
    public class AppointmentJSONReferenceConverter : JsonConverter
    {
		internal static IAppointmentRepository Repo { get; set; }

		public AppointmentJSONReferenceConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Appointment);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var appointmentID = serializer.Deserialize<int>(reader);
			return Repo.GetAll().First(eq => eq.Id == appointmentID);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((Appointment)value).Id);
		}
	}
}
