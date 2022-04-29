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

        public UserAccount()
        {
            Person = new Person();
            Blocked = BlockedBy.NONE;
            AppointmentCreatedTimestamps = new List<DateTime>();
            AppointmentModifiedTimestamps = new List<DateTime>();
        }
        
        public UserAccount(AccountType type)
        {
            Type = type;
            Person = new Person();
            Blocked = BlockedBy.NONE;
            AppointmentCreatedTimestamps = new List<DateTime>();
            AppointmentModifiedTimestamps = new List<DateTime>();
        }

        public enum BlockedBy
        {
            NONE,
            SYSTEM,
            SECRETARY
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public AccountType Type { get; set; }

        [JsonConverter(typeof(Repository.PersonRepository.PersonReferenceConverter))]
        public Person Person { get; set; }

        public List<DateTime> AppointmentModifiedTimestamps { get; set; }

        public List<DateTime> AppointmentCreatedTimestamps { get; set; }

        public BlockedBy Blocked { get; set; }

        public override string ToString()
        {
            return $"UserAccount{{Id = {Id}, Username = {Username}, Password = {Password}, Type = {(int)Type}, Person = {Person.Id}, Blocked = {Blocked}}}";
        }
    }
}
