using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HospitalIS.Backend.Repository
{
    internal class DoctorRepository : IRepository<Doctor>
    {
        public void Add(Doctor entity)
        {
            List<Doctor> Doctors = IS.Instance.Hospital.Doctors;

            entity.Id = Doctors.Count > 0 ? Doctors.Last().Id + 1 : 0;
            Doctors.Add(entity);
        }

        public Doctor GetById(int id)
        {
            return IS.Instance.Hospital.Doctors.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Doctors = JsonConvert.DeserializeObject<List<Doctor>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Doctor entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Doctor, bool> condition)
        {
            IS.Instance.Hospital.Doctors.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Doctors, Formatting.Indented, settings));
        }

        internal class DoctorReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Doctor);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var doctorID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Doctors.First(doctor => doctor.Id == doctorID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Doctor)value).Id);
            }
        }
    }
}
