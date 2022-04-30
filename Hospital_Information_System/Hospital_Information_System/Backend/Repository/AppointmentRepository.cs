using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HospitalIS.Backend.Repository
{
    internal class AppointmentRepository : IRepository<Appointment>
    {
        public void Add(Appointment entity)
        {
            List<Appointment> Appointments = IS.Instance.Hospital.Appointments;

            entity.Id = Appointments.Count > 0 ? Appointments.Last().Id + 1 : 0;
            Appointments.Add(entity);
        }

        public Appointment GetById(int id)
        {
            return IS.Instance.Hospital.Appointments.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Appointments = JsonConvert.DeserializeObject<List<Appointment>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Appointment entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Appointment, bool> condition)
        {
            IS.Instance.Hospital.Appointments.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Appointments, Formatting.Indented, settings));
        }

        internal class AppointmentReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Appointment);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var appointmentID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Appointments.First(appointment => appointment.Id == appointmentID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Appointment)value).Id);
            }
        }
    }
}
