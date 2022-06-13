using System.Collections.Generic;
using System;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
    public interface IMedicalRecordService
    {
        IEnumerable<MedicalRecord> GetAll();
        MedicalRecord GetPatientsMedicalRecord(Patient patient);
    }
}
