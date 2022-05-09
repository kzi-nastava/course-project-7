using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class ReferralRepository : IRepository<Referral>
    {
        public void Add(Referral entity)
        {
            List<Referral> Referrals = IS.Instance.Hospital.Referrals;

            entity.Id = Referrals.Count > 0 ? Referrals.Last().Id + 1 : 0;
            Referrals.Add(entity);
        }

        public Referral GetById(int id)
        {
            return IS.Instance.Hospital.Referrals.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Referrals =
                JsonConvert.DeserializeObject<List<Referral>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Referral entity)
        {
            entity.Deleted = true;
        }
        
        public static void Scheduled(Referral entity)
        {
            entity.Scheduled = true;
        }
        
        public void Remove(Func<Referral, bool> condition)
        {
            IS.Instance.Hospital.Referrals.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.Referrals, Formatting.Indented, settings));
        }

        internal class ReferralReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Referral);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var referralID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Referrals.First(referral => referral.Id == referralID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Referral) value).Id);
            }
        }
    }
}