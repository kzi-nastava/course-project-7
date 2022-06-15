using System.Collections.Generic;
using System;
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
        void Add(Patient patient);
    }
}
