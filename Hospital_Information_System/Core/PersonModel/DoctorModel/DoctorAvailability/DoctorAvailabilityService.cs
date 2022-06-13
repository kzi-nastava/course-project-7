using HIS.Core.AppointmentModel;
using System;
using System.Linq;

namespace HIS.Core.PersonModel.DoctorModel.DoctorAvailability
{
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private IDoctorService _doctorService;
        private IAppointmentService _appointmentService;

        public DoctorAvailabilityService()
        {
        }

        public DoctorAvailabilityService(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        public Doctor FindFirstAvailableDoctor(DateTime scheduledFor)
        {
            return _doctorService.GetAll().First(d => IsAvailable(d, scheduledFor));
        }

        public bool IsAvailable(Doctor doctor, DateTime newSchedule, Appointment refAppointment = null)
        {
            foreach (Appointment appointment in _appointmentService.GetAll())
            {
                if ((doctor == appointment.Doctor) && (appointment != refAppointment))
                {
                    if (_appointmentService.AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
