using System.Collections.Generic;

namespace HospitalIS.Backend.Controller
{
    internal class AppointmentRatingController
    {
        public static List<AppointmentRating> GetAppointmentRatings()
        {
            return IS.Instance.Hospital.AppointmentRatings.FindAll(ar => !ar.Deleted);
        }

        public static List<AppointmentRating> GetAppointmentRatings(Doctor doctor)
        {
            return GetAppointmentRatings().FindAll(ar => ar.Appointment.Doctor == doctor);
        }
    }
}
