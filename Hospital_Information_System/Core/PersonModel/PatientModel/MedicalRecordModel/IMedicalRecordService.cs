using System.Collections.Generic;
using System;
using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
    public interface IMedicalRecordService
    {
        IEnumerable<MedicalRecord> GetAll();
        MedicalRecord GetPatientsMedicalRecord(Patient patient);
        IEnumerable<Appointment> MatchAppointmentByAnamnesis(string query, AppointmentComparer comparer, Patient patient);
    }
}
