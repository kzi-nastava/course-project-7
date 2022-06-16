using System;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using System.Collections.Generic;
using System.Linq;
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

        public int GetNextId()
        {
            return _repo.GetNextId();
        }

        public void Save()
        {
            _repo.Save();
        }

        public MedicalRecord Get(int id)
        {
            return _repo.Get(id);
        }

        public MedicalRecord Add(MedicalRecord obj)
        {
            return _repo.Add(obj);
        }

        public void Remove(MedicalRecord obj)
        {
            _repo.Remove(obj);
        }
        
        public void Copy(MedicalRecord target, MedicalRecord source, IEnumerable<MedicalRecordProperty> whichProperties)
        {
            if (whichProperties.Contains(MedicalRecordProperty.HEIGHT)) target.Height = source.Height;
            if (whichProperties.Contains(MedicalRecordProperty.WEIGHT)) target.Weight = source.Weight;
            if (whichProperties.Contains(MedicalRecordProperty.ILLNESSES)) target.Illnesses = source.Illnesses;
            if (whichProperties.Contains(MedicalRecordProperty.OTHER_ALLERGIES)) target.OtherAllergies = source.OtherAllergies;
            if (whichProperties.Contains(MedicalRecordProperty.ALLERGIES_TO_INGREDIENTS)) target.IngredientAllergies = source.IngredientAllergies;
            if (whichProperties.Contains(MedicalRecordProperty.PRESCRIPTIONS)) target.Prescriptions = source.Prescriptions;
        }
        
        public List<String> GetActionsPerformableOnList()
        {
            List<string> actions = new List<string>();
            actions.Add("ADD");
            actions.Add("REMOVE");
            return actions;
        }
    }
}
