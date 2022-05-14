using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class MedicationRepository : IRepository<Medication>
    {
        public void Add(Medication entity)
        {
            List<Medication> Medications = IS.Instance.Hospital.Medications;

            entity.Id = Medications.Count > 0 ? Medications.Last().Id + 1 : 0;
            Medications.Add(entity);
        }

        public Medication GetById(int id)
        {
            return IS.Instance.Hospital.Medications.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Medications =
                JsonConvert.DeserializeObject<List<Medication>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Medication entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Medication, bool> condition)
        {
            IS.Instance.Hospital.Medications.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.Medications, Formatting.Indented, settings));
        }

        internal class MedicationReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Medication);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                var medicationID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Medications.First(medicine => medicine.Id == medicationID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Medication) value).Id);
            }
        }
    }
}