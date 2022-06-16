using System;
using System.Collections.Generic;
using HIS.Core.Foundation;
using Newtonsoft.Json;

namespace HIS.Core.EquipmentModel.EquipmentRequestModel
{
    public class EquipmentRequest : Entity
    {
        [JsonConverter(typeof(EquipmentJSONDictionaryConverter<int>))]
        public Dictionary<Equipment, int> Equipment = new Dictionary<Equipment, int>();

        public DateTime OrderTime { get; set; }

        public bool Added;

        public EquipmentRequest()
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