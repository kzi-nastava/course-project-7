using System;
using System.Collections.Generic;
using HospitalIS.Backend.Repository;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class RequestEquipment : Entity
    {
        [JsonConverter(typeof(EquipmentRepository.EquipmentDictionaryConverter<int>))]
        public Dictionary<Equipment, int> Equipment = new Dictionary<Equipment, int>();

        public DateTime OrderTime { get; set; }

        public bool Added;

        public RequestEquipment()
        {
            Added = false;
        }
        
        public override string ToString()
        {
            return $"RequestEquipment{{Id = {Id}, OrderTime = {OrderTime}, Added = {Added}}}";
        }

        public int GetTimeToLive()
        {
            return (int) (OrderTime - OrderTime.AddDays(1)).TotalMilliseconds;
        }
        
    }
}