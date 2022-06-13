using System;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
	public class MedicalRecordService : IMedicalRecordService
	{
		private readonly IMedicalRecordRepository _repo;

		public MedicalRecordService(IMedicalRecordRepository repo)
		{
			_repo = repo;
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
	}
}
