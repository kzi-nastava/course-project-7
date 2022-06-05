using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class UpdateRequestRepository : IRepository<UpdateRequest>
    {
        public void Add(UpdateRequest entity)
        {
            List<UpdateRequest> UpdateRequests = IS.Instance.Hospital.UpdateRequests;

            entity.Id = UpdateRequests.Count > 0 ? UpdateRequests.Last().Id + 1 : 0;
            UpdateRequests.Add(entity);
        }

        public UpdateRequest GetById(int id)
        {
            return IS.Instance.Hospital.UpdateRequests.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.UpdateRequests = JsonConvert.DeserializeObject<List<UpdateRequest>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(UpdateRequest entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<UpdateRequest, bool> condition)
        {
            IS.Instance.Hospital.UpdateRequests.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.UpdateRequests, Formatting.Indented, settings));
        }
    }
}
