using HIS.Core.AppointmentModel.Util;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.ModificationRequestModel.DeleteRequestModel;
using HIS.Core.ModificationRequestModel.UpdateRequestModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.RoomModel;
using HIS.Core.RoomModel.RoomAvailability;
using HIS.Core.PersonModel.DoctorModel.DoctorAvailability;
using HIS.Core.PersonModel.PatientModel.PatientAvailability;

namespace HIS.Core.AppointmentModel
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;
        private readonly IUserAccountService _userAccountService;
        private readonly IDeleteRequestService _deleteRequestService;
        private readonly IUpdateRequestService _updateRequestService;
        private readonly IMedicalRecordService _medicalRecordService;
        private readonly IRoomAvailabilityService _roomAvailabilityService;
        private readonly IDoctorAvailabilityService _doctorAvailabilityService;
        private readonly IPatientAvailabilityService _patientAvailabilityService;

        public AppointmentService(IAppointmentRepository repo, IUserAccountService userAccountService, IDeleteRequestService deleteRequestService, IUpdateRequestService updateRequestService,
            IMedicalRecordService medicalRecordService, IRoomAvailabilityService roomAvailabilityService, IDoctorAvailabilityService doctorAvailabilityService,
            IPatientAvailabilityService patientAvailabilityService)
        {
            _repo = repo;
            _userAccountService = userAccountService;
            _deleteRequestService = deleteRequestService;
            _updateRequestService = updateRequestService;
            _medicalRecordService = medicalRecordService;
            _roomAvailabilityService = roomAvailabilityService;
            _doctorAvailabilityService = doctorAvailabilityService;
            _patientAvailabilityService = patientAvailabilityService;
        }

        public IEnumerable<Appointment> GetAll()
        {
            return _repo.GetAll();
        }

		public IEnumerable<Appointment> GetAll(Patient patient)
		{
			return _repo.GetAll(patient);
		}

		public IEnumerable<Appointment> GetAll(Doctor doctor)
		{
			return _repo.GetAll(doctor);
		}

		public IEnumerable<Appointment> GetModifiable(UserAccount user)
		{
			return _repo.GetModifiable(user);
		}

        public void Add(Appointment appointment, UserAccount user)
        {
            _userAccountService.AddCreatedAppointmentTimestamp(user, DateTime.Now);
            MedicalRecord patientsRecord = _medicalRecordService.GetPatientsMedicalRecord(appointment.Patient);
            appointment.Anamnesis = "";
            patientsRecord.Examinations.Add(appointment);
            _repo.Add(appointment);
        }

        public void Update(Appointment appointment, Appointment updatedAppointment, IEnumerable<AppointmentProperty> propertiesToUpdate, UserAccount user)
        {
            _userAccountService.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                var proposedAppointment = new Appointment();
                Copy(proposedAppointment, appointment, AppointmentPropertyHelpers.GetProperties());
                Copy(proposedAppointment, updatedAppointment, propertiesToUpdate);
                _ = _updateRequestService.Add(new UpdateRequest(user, appointment, proposedAppointment));
            }
            else
            {
                Copy(appointment, updatedAppointment, propertiesToUpdate);
            }
        }

        public void Remove(Appointment appointment, UserAccount user)
        {
            _userAccountService.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                _ = _deleteRequestService.Add(new DeleteRequest(user, appointment));
            }
            else
            {
                _repo.Remove(appointment);
                MedicalRecord patientsRecord = _medicalRecordService.GetPatientsMedicalRecord(appointment.Patient);
                patientsRecord.Examinations.Remove(appointment);
            }
        }

        public bool MustRequestModification(Appointment appointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = appointment.ScheduledFor - DateTime.Now;
                return difference.TotalDays < AppointmentConstants.DaysBeforeModificationNeedsRequest;
            }
            else
            {
                return false;
            }
        }

        public bool AreColliding(DateTime schedule1, DateTime schedule2)
        {
            TimeSpan difference = schedule1 - schedule2;
            return Math.Abs(difference.TotalMinutes) < AppointmentConstants.LengthOfAppointmentInMinutes;
        }

        public void Copy(Appointment target, Appointment source, IEnumerable<AppointmentProperty> whichProperties)
        {
            if (whichProperties.Contains(AppointmentProperty.DOCTOR)) target.Doctor = source.Doctor;
            if (whichProperties.Contains(AppointmentProperty.PATIENT)) target.Patient = source.Patient;
            if (whichProperties.Contains(AppointmentProperty.ROOM)) target.Room = source.Room;
            if (whichProperties.Contains(AppointmentProperty.SCHEDULED_FOR)) target.ScheduledFor = source.ScheduledFor;
            if (whichProperties.Contains(AppointmentProperty.ANAMNESIS)) target.Anamnesis = source.Anamnesis;
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
    }
}
