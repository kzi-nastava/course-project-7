using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HospitalIS.Backend
{
    public class UserAccountForcefullyBlockedException : Exception
    {
        public UserAccountForcefullyBlockedException(string exceptionMessage) : base($"User account blocked: {exceptionMessage}")
        {

        }
    }
    public class UserAccount : Entity
    {
        public enum AccountType
        {
            PATIENT,
            DOCTOR,
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public AccountType Type { get; set; }

        [JsonConverter(typeof(Repository.PersonRepository.PersonReferenceConverter))]
        public Person Person { get; set; }

        public List<DateTime> AppointmentModifiedTimestamps { get; set; }

        public List<DateTime> AppointmentCreatedTimestamps { get; set; }

        public bool Blocked { get; set; }

        public override string ToString()
        {
            return $"UserAccount{{Id = {Id}, Username = {Username}, Password = {Password}, Type = {(int)Type}, Person = {Person.Id}, Blocked = {Blocked}}}";
        }
    }
}
