using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Repository
{
    internal class AppointmentRatingRepository : IRepository<AppointmentRating>
    {
        public void Add(AppointmentRating entity)
        {
            List<AppointmentRating> AppointmentRatings = IS.Instance.Hospital.AppointmentRatings;

            entity.Id = AppointmentRatings.Count > 0 ? AppointmentRatings.Last().Id + 1 : 0;
            AppointmentRatings.Add(entity);
        }

        public AppointmentRating GetById(int id)
        {
            return IS.Instance.Hospital.AppointmentRatings.First(e => e.Id == id);
        }

        public void Load(string fullFilename, JsonSerializerSettings settings)
        {
            IS.Instance.Hospital.AppointmentRatings = JsonConvert.DeserializeObject<List<AppointmentRating>>(File.ReadAllText(fullFilename), settings);
        }

        public void Remove(AppointmentRating entity)
        {
            entity.Deleted = true;
        }

        public void Remove(Func<AppointmentRating, bool> condition)
        {
            IS.Instance.Hospital.AppointmentRatings.ForEach(entity => { if (condition(entity)) Remove(entity); });
        }

        public void Save(string fullFilename, JsonSerializerSettings settings)
        {
            File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.AppointmentRatings, Formatting.Indented, settings));
        }
    }
}
