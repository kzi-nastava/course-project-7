using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using HospitalIS.Backend.Controller;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Doctor : Entity
    {
        [JsonConverter(typeof(Repository.PersonRepository.PersonReferenceConverter))]
        public Person Person { get; set; }

        public enum MedicineSpeciality
        {
            GENERAL_PRACTICE, UROLOGY, SURGERY, PSYCHIATRY, PEDIATRICS, OPHTHAMOLOGY, NEUROLOGY, ANESTHESIOLOGY, DERMATOLOGY, PATHOLOGY, GENETICS, EMERGENCY_MEDICINE, GYNAECOLOGY
        }

        public MedicineSpeciality Specialty { get; set; }

        public override string ToString()
        {
            return $"Doctor{{Id = {Id}, Person = {Person.Id}, Specialty = {Specialty}}}";
        }

        public string VerboseToString()
        {
            return $"Doctor{{Id = {Id}, First name = {Person.FirstName}, Last name = {Person.LastName}, Specialty = {Specialty}, Rating = {Math.Round(DoctorController.CalculateRating(this), 2)}}}";
        }

        public abstract class Comparer : Comparer<Doctor>
        {

        }

        public class CompareByFirstName : Comparer
        {
            public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
            {
                return x.Person.FirstName.CompareTo(y.Person.FirstName);
            }
        }

        public class CompareByLastName : Comparer
        {
            public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
            {
                return x.Person.LastName.CompareTo(y.Person.LastName);
            }
        }

        public class CompareBySpecialty : Comparer
        {
            public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
            {
                return x.Specialty.CompareTo(y.Specialty);
            }
        }

        public class CompareByRatingDesc : Comparer
        {
            public override int Compare([AllowNull] Doctor x, [AllowNull] Doctor y)
            {
                return -DoctorController.CalculateRating(x).CompareTo(DoctorController.CalculateRating(y));
            }
        }
    }
}
