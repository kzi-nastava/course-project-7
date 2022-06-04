using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class RequestEquipmentRepository : IRepository<RequestEquipment>
    {

        public void Add(RequestEquipment entity)
        {
            List<RequestEquipment> RequestsEquipment = IS.Instance.Hospital.RequestsEquipment;

            entity.Id = RequestsEquipment.Count > 0 ? RequestsEquipment.Last().Id + 1 : 0;
            Console.WriteLine(entity);
            RequestsEquipment.Add(entity);
        }
        
        public RequestEquipment GetById(int id)
        {
            return IS.Instance.Hospital.RequestsEquipment.FirstOrDefault(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.RequestsEquipment = JsonConvert.DeserializeObject<List<RequestEquipment>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(RequestEquipment requestEquipment)
        {
            //TODO
        }
        
        public void Remove(Func<RequestEquipment, bool> condition)
        {
            IS.Instance.Hospital.RequestsEquipment.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.RequestsEquipment, Formatting.Indented, settings));
        }
        
        public void Execute(RequestEquipment request)
        {
            Thread.Sleep(Math.Max(request.GetTimeToLive(), 0));
            
            if (request.Added)
                return;
            
            if (!IS.Instance.Hospital.RequestsEquipment.Contains(request))
                throw new EntityNotFoundException();
            
            Room warehouse = IS.Instance.RoomRepo.GetWarehouse();
            foreach (var equipment in request.Equipment)
            {
                IS.Instance.RoomRepo.Add(warehouse, equipment.Key, equipment.Value);    
            }
            
            Console.WriteLine($"Ordered equipment added.");
            request.Added = true;
        }
        
        public void AddTask(RequestEquipment request)
        {
            Task t = new Task(() => Execute(request));
            IS.Instance.Hospital.RequestEquipmentTasks.Add(t);
            t.Start();
        }

        internal class RequestEquipmentReferenceConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(RequestEquipment);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var id = serializer.Deserialize<int>(reader);
                return IS.Instance.Hospital.RequestsEquipment.First(request => request.Id == id);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, ((RequestEquipment)value).Id);
            }
        }
    }
}