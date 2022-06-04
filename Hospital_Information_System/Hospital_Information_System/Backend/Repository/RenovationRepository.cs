using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HospitalIS.Backend.Repository
{
	internal class RenovationRepository : IRepository<Renovation>
	{
        public void Add(Renovation entity)
        {
            List<Renovation> Renovations = IS.Instance.Hospital.Renovations;

            entity.Id = Renovations.Count > 0 ? Renovations.Last().Id + 1 : 0;
            Renovations.Add(entity);
        }

        public Renovation GetById(int id)
        {
            return IS.Instance.Hospital.Renovations.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.Renovations = JsonConvert.DeserializeObject<List<Renovation>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(Renovation entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<Renovation, bool> condition)
        {
            IS.Instance.Hospital.Renovations.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Renovations, Formatting.Indented, settings));
        }
    }
}
