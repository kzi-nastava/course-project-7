using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.AppointmentModel
{
	internal class AppointmentJSONIListConverter : JsonConverter
	{
		internal static IAppointmentRepository Repo { get; set; }

		public AppointmentJSONIListConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var writableList = ((IList<Appointment>)value).Select(ing => ing.Id);
			serializer.Serialize(writer, writableList);
		}

		public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
		{
			var readableList = serializer.Deserialize<IList<int>>(reader);
			return readableList.Select(ingId => Repo.Get(ingId)).ToList();
		}

		public override bool CanConvert(System.Type objectType)
		{
			return objectType == typeof(IList<int>);
		}
	}
}
