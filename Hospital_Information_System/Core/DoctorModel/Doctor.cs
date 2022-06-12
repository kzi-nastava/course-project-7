using System;
using HIS.Core.Foundation;
using HIS.Core.PersonModel;
using Newtonsoft.Json;

namespace HIS.Core.DoctorModel
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

        public string VerboseToString()
        {
            throw new NotImplementedException();
            //return $"Doctor{{Id = {Id}, First name = {Person.FirstName}, Last name = {Person.LastName}, Specialty = {Specialty}, Rating = {Math.Round(DoctorController.CalculateRating(this), 2)}}}";
        }
    }
}
