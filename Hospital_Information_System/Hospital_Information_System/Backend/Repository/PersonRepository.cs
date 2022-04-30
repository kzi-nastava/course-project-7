using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class PersonRepository : IRepository<Person>
    {
        public void Add(Person entity)
        {
            List<Person> Persons = IS.Instance.Hospital.Persons;

            entity.Id = Persons.Count > 0 ? Persons.Last().Id + 1 : 0;
            Persons.Add(entity);
        }

        public Person GetById(int id)
        {
            return IS.Instance.Hospital.Persons.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Persons = JsonConvert.DeserializeObject<List<Person>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Person entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Person, bool> condition)
        {
            IS.Instance.Hospital.Persons.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Persons, Formatting.Indented, settings));
        }

        internal class PersonReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Person);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var personID = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.Persons.First(person => person.Id == personID);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((Person)value).Id);
            }
        }
    }
}
