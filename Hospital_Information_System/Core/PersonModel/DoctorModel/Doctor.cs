using System;
using HIS.Core.Foundation;
using HIS.Core.PersonModel;
using Newtonsoft.Json;

namespace HIS.Core.PersonModel.DoctorModel
{
    public class Doctor : Entity
    {
        [JsonConverter(typeof(PersonJSONReferenceConverter))]
        public Person Person { get; set; }

        public enum MedicineSpeciality
        {
            GENERAL_PRACTICE, SURGERY, PSYCHIATRY
        }

        public MedicineSpeciality Specialty { get; set; }

        public override string ToString()
        {
            return $"Doctor{{Id = {Id}, Person = {Person.Id}, Specialty = {Specialty}}}";
        }
    }
}
