using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HospitalIS.Backend.Repository
{
    internal class PatientRepository : IRepository<Patient>
    {
        public void Add(Patient entity)
        {
            List<Patient> Patients = IS.Instance.Hospital.Patients;

            entity.Id = Patients.Count > 0 ? Patients.Last().Id + 1 : 0;
            Patients.Add(entity);
        }

        public Patient GetById(int id)
        {
            return IS.Instance.Hospital.Patients.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Patients = JsonConvert.DeserializeObject<List<Patient>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Patient entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Patient, bool> condition)
        {
            IS.Instance.Hospital.Patients.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Patients, Formatting.Indented, settings));
        }

        internal class PatientReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Patient);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var patientID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Patients.First(patient => patient.Id == patientID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Patient)value).Id);
            }
        }
    }
}
