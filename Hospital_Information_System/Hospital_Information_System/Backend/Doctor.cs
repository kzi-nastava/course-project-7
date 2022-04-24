using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Doctor : Entity
    {
        [JsonConverter(typeof(Repository.PersonRepository.PersonReferenceConverter))]
        public Person Person { get; set; }

        public override string ToString()
        {
            return $"Doctor{{Id = {Id}, Person = {Person.Id}}}";
        }
    }
}
