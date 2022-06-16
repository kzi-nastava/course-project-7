using System;
using System.Collections.Generic;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
    public interface IMedicalRecordService
    {
        IEnumerable<MedicalRecord> GetAll();
        MedicalRecord GetPatientsMedicalRecord(Patient patient);
        IEnumerable<Appointment> MatchAppointmentByAnamnesis(string query, AppointmentComparer comparer, Patient patient);
        void AddNotifsIfNecessary(UserAccount ua);
        public int GetNextId();
        public void Save();
        public MedicalRecord Get(int id);
        public MedicalRecord Add(MedicalRecord obj);
        public void Remove(MedicalRecord obj);
        
        void Copy(MedicalRecord target, MedicalRecord source, IEnumerable<MedicalRecordProperty> whichProperties);
        List<String> GetActionsPerformableOnList();
    }
}
