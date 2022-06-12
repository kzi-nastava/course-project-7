using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.PropertyUtil;
using HIS.Core.AppointmentModel.SearchUtil;
using HIS.Core.DoctorModel;
using HIS.Core.PatientModel;
using HIS.Core.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.AppointmentModel
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repo;

        private const int lengthOfAppointmentInMinutes = 15;
        private const int daysBeforeModificationNeedsRequest = 2;

		public AppointmentService(IAppointmentRepository repo)
        {
            _repo = repo;
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
            // TODO: Implement.
            //UserAccountController.AddCreatedAppointmentTimestamp(user, DateTime.Now);
            //MedicalRecord patientsRecord = MedicalRecordController.GetPatientsMedicalRecord(appointment.Patient);
            appointment.Anamnesis = "";
            //patientsRecord.Examinations.Add(appointment);
            _repo.Add(appointment);
        }

        public void Update(Appointment appointment, Appointment updatedAppointment, IEnumerable<AppointmentProperty> propertiesToUpdate, UserAccount user)
        {
            // TODO: Implement.
            //UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                var proposedAppointment = new Appointment();
                Copy(proposedAppointment, appointment, AppointmentPropertyHelper.GetProperties());
                Copy(proposedAppointment, updatedAppointment, propertiesToUpdate);
                //IS.Instance.UpdateRequestRepo.Add(new UpdateRequest(user, appointment, proposedAppointment));
            }
            else
            {
                Copy(appointment, updatedAppointment, propertiesToUpdate);
            }
        }

        public void Remove(Appointment appointment, UserAccount user)
        {
            // TODO: Implement.
            //UserAccountController.AddModifiedAppointmentTimestamp(user, DateTime.Now);
            if (MustRequestModification(appointment, user))
            {
                //IS.Instance.DeleteRequestRepo.Add(new DeleteRequest(user, appointment));
            }
            else
            {
                _repo.Remove(appointment);
                //MedicalRecord patientsRecord = MedicalRecordController.GetPatientsMedicalRecord(appointment.Patient);
                //patientsRecord.Examinations.Remove(appointment);
            }
        }

        public bool MustRequestModification(Appointment appointment, UserAccount user)
        {
            if (user.Type == UserAccount.AccountType.PATIENT)
            {
                TimeSpan difference = appointment.ScheduledFor - DateTime.Now;
                return difference.TotalDays < daysBeforeModificationNeedsRequest;
            }
            else
            {
                return false;
            }
        }

        public bool AreColliding(DateTime schedule1, DateTime schedule2)
        {
            TimeSpan difference = schedule1 - schedule2;
            return Math.Abs(difference.TotalMinutes) < lengthOfAppointmentInMinutes;
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
            // TODO: Implement.
            //for (DateTime currDt = DateTime.Today; currDt < sb.By.Date; currDt = currDt.AddDays(1))
            //{
            //    for (TimeSpan currTs = sb.Start; currTs <= sb.End; currTs = currTs.Add(TimeSpan.FromMinutes(1)))
            //    {
            //        DateTime scheduledFor = currDt.Add(currTs);
            //        if (scheduledFor < DateTime.Now) continue;

            //        Doctor doctor = (sb.Doctor != null) ?
            //            (DoctorController.IsAvailable(sb.Doctor, scheduledFor) ? sb.Doctor : null) : DoctorController.FindFirstAvailableDoctor(scheduledFor);
            //        if (doctor == null) continue;

            //        Patient patient = (sb.Patient != null) ?
            //            (PatientController.IsAvailable(sb.Patient, scheduledFor) ? sb.Patient : null) : PatientController.FindFirstAvailablePatient(scheduledFor);
            //        if (patient == null) continue;

            //        Room room = RoomController.FindFirstAvailableExaminationRoom(scheduledFor);
            //        if (room == null) continue;

            //        return new Appointment(doctor, patient, room, scheduledFor);
            //    }
            //}
            return null;
        }
    }
}
