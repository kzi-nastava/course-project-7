using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel
{
    public class DaysOffRequestJSONReferenceConverter : JsonConverter
    {
        internal static IDaysOffRequestRepository Repo { get; set; }

        public DaysOffRequestJSONReferenceConverter()
        {
            if (Repo == null) throw new JSONRepoReferenceNullException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DaysOffRequest);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var daysOffRequestID = serializer.Deserialize<int>(reader);
            return Repo.GetAll().First(eq => eq.Id == daysOffRequestID);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((DaysOffRequest)value).Id);
        }
    }
}