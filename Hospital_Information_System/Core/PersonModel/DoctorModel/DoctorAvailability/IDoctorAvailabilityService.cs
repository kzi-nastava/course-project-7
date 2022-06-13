using HIS.Core.AppointmentModel;
using System;

namespace HIS.Core.PersonModel.DoctorModel.DoctorAvailability
{
    public interface IDoctorAvailabilityService
    {
        public bool IsAvailable(Doctor doctor, DateTime newSchedule, Appointment refAppointment = null);
        public Doctor FindFirstAvailableDoctor(DateTime scheduledFor);
    }
}
