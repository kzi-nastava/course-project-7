using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
    public class MedicationRequestRepository : IRepository<MedicationRequest>
    {
        public void Add(MedicationRequest entity)
        {
            var medicationRequests = IS.Instance.Hospital.MedicationRequests;

            entity.Id = medicationRequests.Count > 0 ? medicationRequests.Last().Id + 1 : 0;
            medicationRequests.Add(entity);
        }

        public MedicationRequest GetById(int id)
        {
            return IS.Instance.Hospital.MedicationRequests.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.MedicationRequests =
                JsonConvert.DeserializeObject<List<MedicationRequest>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(MedicationRequest entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<MedicationRequest, bool> condition)
        {
            IS.Instance.Hospital.MedicationRequests.ForEach(entity =>
            {
                if (condition(entity)) Remove(entity);
            });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename,
                JsonConvert.SerializeObject(IS.Instance.Hospital.MedicationRequests, Formatting.Indented, settings));
        }
    }
}