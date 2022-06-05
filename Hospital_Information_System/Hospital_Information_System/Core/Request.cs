﻿using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public abstract class Request : Entity
    {
        [JsonConverter(typeof(Repository.UserAccountRepository.UserAccountReferenceConverter))]
        public UserAccount Requestee { get; set; }

        public enum StateType
        {
            PENDING, APPROVED, DENIED
        }

        public StateType State { get; set; }
        
        public Request(UserAccount requestee)
        {
            Requestee = requestee;
            State = StateType.PENDING;
        }
    }
}