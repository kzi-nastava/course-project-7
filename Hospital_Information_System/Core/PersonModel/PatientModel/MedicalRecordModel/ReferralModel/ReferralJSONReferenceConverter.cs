using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel
{
    public class ReferralJSONReferenceConverter : JsonConverter
    {
        internal static IReferralRepository Repo { get; set; }

        public ReferralJSONReferenceConverter()
        {
            if (Repo == null) throw new JSONRepoReferenceNullException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Referral);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var referralID = serializer.Deserialize<int>(reader);
            return Repo.GetAll().First(eq => eq.Id == referralID);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, ((Referral)value).Id);
        }
    }
}