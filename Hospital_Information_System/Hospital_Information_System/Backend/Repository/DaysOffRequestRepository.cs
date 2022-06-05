using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class DaysOffRequestRepository : IRepository<DaysOffRequest>
    {
        public void Add(DaysOffRequest entity)
        {
            var daysOffRequests = IS.Instance.Hospital.DaysOffRequests;

            entity.Id = daysOffRequests.Count > 0 ? daysOffRequests.Last().Id + 1 : 0;
            daysOffRequests.Add(entity);
        }

        public DaysOffRequest GetById(int id)
        {
            return IS.Instance.Hospital.DaysOffRequests.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.DaysOffRequests =
                JsonConvert.DeserializeObject<List<DaysOffRequest>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(DaysOffRequest entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<DaysOffRequest, bool> condition)
        {
            IS.Instance.Hospital.DaysOffRequests.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.DaysOffRequests, Formatting.Indented, settings));
        }
    }
}