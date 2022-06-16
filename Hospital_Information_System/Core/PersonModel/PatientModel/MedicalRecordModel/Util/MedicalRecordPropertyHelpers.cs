using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel.Util
{
    public static class MedicalRecordPropertyHelpers
    {
        public static string GetName(MedicalRecordProperty ap)
        {
            return Enum.GetName(typeof(MedicalRecordProperty), ap);
        }
        public static List<MedicalRecordProperty> GetAllProperties()
        {
            return Enum.GetValues(typeof(MedicalRecordProperty)).Cast<MedicalRecordProperty>().ToList();
        }
        
        public static List<MedicalRecordProperty> GetModifiableProperties(){
            List<MedicalRecordProperty> modifiableProperties = GetAllProperties();
            modifiableProperties.Remove(MedicalRecordProperty.PATIENT);
            modifiableProperties.Remove(MedicalRecordProperty.PRESCRIPTIONS);
            return modifiableProperties;
        }

        public static List<MedicalRecordProperty> GetPrescriptionProperty()
        {
            List<MedicalRecordProperty> prescriptionProperty = new List<MedicalRecordProperty>();
            prescriptionProperty.Add(MedicalRecordProperty.PRESCRIPTIONS);
            return prescriptionProperty;
        }
    }
}