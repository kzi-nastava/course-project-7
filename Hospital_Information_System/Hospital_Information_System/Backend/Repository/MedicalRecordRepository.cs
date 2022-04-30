using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class MedicalRecordRepository : IRepository<MedicalRecord>
    {
        public void Add(MedicalRecord entity)
        {
            List<MedicalRecord> MedicalRecords = IS.Instance.Hospital.MedicalRecords;

            entity.Id = MedicalRecords.Count > 0 ? MedicalRecords.Last().Id + 1 : 0;
            MedicalRecords.Add(entity);
        }

        public MedicalRecord GetById(int id)
        {
            return IS.Instance.Hospital.MedicalRecords.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.MedicalRecords = JsonConvert.DeserializeObject<List<MedicalRecord>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(MedicalRecord entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<MedicalRecord, bool> condition)
        {
            IS.Instance.Hospital.MedicalRecords.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.MedicalRecords, Formatting.Indented, settings));
        }

        internal class MedicalRecordReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(MedicalRecord);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var medicalRecordID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.MedicalRecords.First(medicalRecord => medicalRecord.Id == medicalRecordID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((MedicalRecord)value).Id);
            }
        }
    }
}