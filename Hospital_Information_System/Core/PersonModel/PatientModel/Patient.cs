using HIS.Core.Foundation;
using HIS.Core.PersonModel;
using Newtonsoft.Json;

namespace HIS.Core.PatientModel
{
    public class Patient : Entity
    {
        [JsonConverter(typeof(PersonJSONReferenceConverter))]
        public Person Person { get; set; }

        public Patient(Person person)
        {
            Person = person;
        }

        public override string ToString()
        {
            return $"Patient{{Id = {Id}, Person = {Person.Id}, FirstName = {Person.FirstName}, LastName = {Person.LastName}}}";
        }
    }
}
