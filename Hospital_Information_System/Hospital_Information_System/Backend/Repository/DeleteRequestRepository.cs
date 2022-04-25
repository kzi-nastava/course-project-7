using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class DeleteRequestRepository : IRepository<DeleteRequest>
    {
        public void Add(DeleteRequest entity)
        {
            List<DeleteRequest> DeleteRequests = IS.Instance.Hospital.DeleteRequests;

            entity.Id = DeleteRequests.Count > 0 ? DeleteRequests.Last().Id + 1 : 0;
            DeleteRequests.Add(entity);
        }

        public DeleteRequest GetById(int id)
        {
            return IS.Instance.Hospital.DeleteRequests.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.DeleteRequests = JsonConvert.DeserializeObject<List<DeleteRequest>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(DeleteRequest entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<DeleteRequest, bool> condition)
        {
            IS.Instance.Hospital.DeleteRequests.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.DeleteRequests, Formatting.Indented, settings));
        }

    }
}
