using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Patient : Entity
    {
        [JsonConverter(typeof(Repository.PersonRepository.PersonReferenceConverter))]
        public Person Person { get; set; }

        public Patient(Person person)
        {
            Person = person;
        }

        public override string ToString()
        {
            return $"Patient{{Id = {Id}, Person = {Person.Id}}}";
        }
    }
}
