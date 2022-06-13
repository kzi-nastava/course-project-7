using System;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using System.Collections.Generic;
using System.Text;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.MedicationModel.PrescriptionModel;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
	public class MedicalRecordService : IMedicalRecordService
	{
		private readonly IMedicalRecordRepository _repo;
        private readonly IPatientService _patientService;

        public MedicalRecordService(IMedicalRecordRepository repo, IPatientService patientService)
		{
			_repo = repo;
            _patientService = patientService;
		}

		public IEnumerable<MedicalRecord> GetAll()
		{
			return _repo.GetAll();
		}

        public MedicalRecord GetPatientsMedicalRecord(Patient patient)
        {
            var allMedicalRecords = _repo.GetAll();
            foreach (var medicalRecord in allMedicalRecords)
            {
                if (patient.Id == medicalRecord.Patient.Id)
                {
                    return medicalRecord;
                }

            }
            return null;
        }

		public IEnumerable<Appointment> MatchAppointmentByAnamnesis(string query, AppointmentComparer comparer, Patient patient)
		{
			var matches = GetPatientsMedicalRecord(patient).Examinations.FindAll(
				e => e.ScheduledFor < DateTime.Now && e.Anamnesis.Trim().ToLower().Contains(query.Trim().ToLower()));
			matches.Sort(comparer);
			return matches;
		}

        public void AddNotifsIfNecessary(UserAccount ua)
        {
            if (ua.Type == UserAccount.AccountType.PATIENT)
            {
                Patient p = _patientService.GetPatientFromPerson(ua.Person);
                MedicalRecord mr = GetPatientsMedicalRecord(p);
                AddPrescriptionNotificationTasks(mr);
            }
        }

        private void ExecutePrescriptionNotification(MedicalRecord record, Prescription prescription)
        {
            prescription.TimesOfUsage.Sort();
            foreach (TimeSpan time in prescription.TimesOfUsage)
            {
                TimeSpan timeUntilPrescription = time - DateTime.Now.TimeOfDay;
                if (timeUntilPrescription.TotalMinutes < 0) continue;
                TimeSpan timeToSleep = timeUntilPrescription - TimeSpan.FromMinutes(record.MinutesBeforeNotification);
                timeToSleep = timeToSleep.TotalMinutes > 0 ? timeToSleep : TimeSpan.FromMinutes(0);

                Thread.Sleep(timeToSleep);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Don't forget to take {prescription.Medication.Name} at {time}!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        private void AddPrescriptionNotificationTasks(MedicalRecord record)
        {
            var tasks = new List<Task>();
            foreach (Prescription p in record.Prescriptions)
            {
                tasks.Add(new Task(() => ExecutePrescriptionNotification(record, p)));
            }
            foreach (Task t in tasks)
            {
                t.Start();
            }
        }
    }
}
