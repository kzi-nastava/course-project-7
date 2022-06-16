using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.PersonModel
{
	public class PersonJSONReferenceConverter : JsonConverter
	{
		internal static IPersonRepository Repo { get; set; }

		public PersonJSONReferenceConverter()
		{
			if (Repo == null) throw new JSONRepoReferenceNullException();
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Person);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var patientID = serializer.Deserialize<int>(reader);
			return Repo.GetAll().First(eq => eq.Id == patientID);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((Person)value).Id);
		}
	}
}
