using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class PrescriptionRepository : IRepository<Prescription>
    {
        public void Add(Prescription entity)
        {
            List<Prescription> Prescriptions = IS.Instance.Hospital.Prescriptions;

            entity.Id = Prescriptions.Count > 0 ? Prescriptions.Last().Id + 1 : 0;
            Prescriptions.Add(entity);
        }

        public Prescription GetById(int id)
        {
            return IS.Instance.Hospital.Prescriptions.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Prescriptions =
                JsonConvert.DeserializeObject<List<Prescription>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Prescription entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Prescription, bool> condition)
        {
            IS.Instance.Hospital.Prescriptions.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.Prescriptions, Formatting.Indented, settings));
        }

        internal class PrescriptionReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Prescription);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var prescriptionID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Prescriptions.First(prescription => prescription.Id == prescriptionID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Prescription) value).Id);
            }
        }
        internal class PrescriptionListConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var prescriptions = new List<int>();
                foreach (Prescription prescription in (List<Prescription>) value)
                {
                    prescriptions.Add(prescription.Id);
                }

                serializer.Serialize(writer, prescriptions);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                List<int> prescriptionsRefs = serializer.Deserialize<List<int>>(reader);
                var prescriptions = new List<Prescription>();
                foreach (int prescriptionsRef in prescriptionsRefs)
                {
                    prescriptions.Add(IS.Instance.PrescriptionRepo.GetById(prescriptionsRef));
                }

                return prescriptions;
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(List<Prescription>);
            }
        }
    }
}