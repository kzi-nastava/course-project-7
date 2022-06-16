using HIS.Core.AppointmentModel.Util;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DoctorAvailability;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.PatientAvailability;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RoomAvailability;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.AppointmentModel.AppointmentAvailability
{
    public class AppointmentAvailabilityService : IAppointmentAvailabilityService
    {
        private readonly IRoomAvailabilityService _roomAvailabilityService;
        private readonly IDoctorAvailabilityService _doctorAvailabilityService;
        private readonly IPatientAvailabilityService _patientAvailabilityService;

        public AppointmentAvailabilityService(IRoomAvailabilityService roomAvailabilityService, IDoctorAvailabilityService doctorAvailabilityService, IPatientAvailabilityService patientAvailabilityService)
        {
            _roomAvailabilityService = roomAvailabilityService;
            _doctorAvailabilityService = doctorAvailabilityService;
            _patientAvailabilityService = patientAvailabilityService;
        }

        public Appointment FindRecommendedAppointment(AppointmentSearchBundle sb)
        {
            for (DateTime currDt = DateTime.Today; currDt < sb.By.Date; currDt = currDt.AddDays(1))
            {
                for (TimeSpan currTs = sb.Start; currTs <= sb.End; currTs = currTs.Add(TimeSpan.FromMinutes(1)))
                {
                    DateTime scheduledFor = currDt.Add(currTs);
                    if (scheduledFor < DateTime.Now) continue;

                    Doctor doctor = (sb.Doctor != null) ?
                        (_doctorAvailabilityService.IsAvailable(sb.Doctor, scheduledFor) ? sb.Doctor : null) : _doctorAvailabilityService.FindFirstAvailableDoctor(scheduledFor);
                    if (doctor == null) continue;

                    Patient patient = (sb.Patient != null) ?
                        (_patientAvailabilityService.IsAvailable(sb.Patient, scheduledFor) ? sb.Patient : null) : _patientAvailabilityService.FindFirstAvailablePatient(scheduledFor);
                    if (patient == null) continue;

                    Room room = _roomAvailabilityService.FindFirstAvailableExaminationRoom(scheduledFor);
                    if (room == null) continue;

                    return new Appointment(doctor, patient, room, scheduledFor);
                }
            }
            return null;
        }

        public Appointment FindUrgentAppointmentSlot(AppointmentSearchBundle sb, Doctor.MedicineSpeciality speciality)
        {
            DateTime currDt = sb.By;
            Doctor doctor = sb.Doctor;
            Patient patient = sb.Patient;

            for (TimeSpan currTs = sb.Start; currTs <= sb.End; currTs = currTs.Add(TimeSpan.FromMinutes(1)))
            {
                DateTime scheduledFor = currDt.Add(currTs);
                if (scheduledFor < DateTime.Now) continue;

                doctor = (sb.Doctor != null) ?
                    (_doctorAvailabilityService.IsAvailable(sb.Doctor, scheduledFor) ? sb.Doctor : null) : _doctorAvailabilityService.FindFirstAvailableDoctorOfSpecialty(scheduledFor, speciality);
                if (doctor == null) continue;

                patient = (sb.Patient != null) ?
                    (_patientAvailabilityService.IsAvailable(sb.Patient, scheduledFor) ? sb.Patient : null) : _patientAvailabilityService.FindFirstAvailablePatient(scheduledFor);
                if (patient == null) continue;

                Room room = _roomAvailabilityService.FindFirstAvailableExaminationRoom(scheduledFor);
                if (room == null) continue;

                return new Appointment(doctor, patient, room, scheduledFor);
            }

            return new Appointment(doctor, patient, null, DateTime.Now);
        }
    }
}
