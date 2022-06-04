using System;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class AppointmentRating : Entity
    {
        public const string errRatingNotInBounds = "Rating must be between 0 and 5!";

        [JsonConverter(typeof(Repository.AppointmentRepository.AppointmentReferenceConverter))]
        public Appointment Appointment { get; set; }

        private double _rating;
        public double Rating
        {
            get { return _rating; }
            set
            {
                if (value < 0 || value > 5) throw new ArgumentException(errRatingNotInBounds);
                _rating = value;
            }
        }
    }
}
