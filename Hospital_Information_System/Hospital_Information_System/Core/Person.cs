using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalIS.Backend
{
    public class Person : Entity
    {
        public enum PersonGender
        {
            MALE,
            FEMALE,
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public PersonGender Gender { get; set; }

        public override string ToString()
        {
            return $"Person{{Id = {Id}, FirstName = {FirstName}, LastName = {LastName}, Gender = {(int)Gender}}}";
        }
    }
}
