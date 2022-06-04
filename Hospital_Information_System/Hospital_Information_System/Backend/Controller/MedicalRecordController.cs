﻿using System.Collections.Generic;
using System;
using System.Linq;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Backend.Controller
{
    internal class MedicalRecordController
    {
       public enum MedicalRecordProperty
        {
            PATIENT,
            HEIGHT,
            WEIGHT,
            ALLERGIES_TO_INGREDIENTS,
            OTHER_ALLERGIES,
            ILLNESSES,
            PRESCRIPTIONS
        }

        public static string GetAppointmentPropertyName(MedicalRecordProperty ap)
        {
            return Enum.GetName(typeof(MedicalRecordProperty), ap);
        }
        private static List<MedicalRecordProperty> GetAllMedicalRecordProperties()
        {
            return Enum.GetValues(typeof(MedicalRecordProperty)).Cast<MedicalRecordProperty>().ToList();
        }
        
        public static List<MedicalRecordProperty> GetModifiableProperties(){
            List<MedicalRecordProperty> modifiableProperties = GetAllMedicalRecordProperties();
            modifiableProperties.Remove(MedicalRecordProperty.PATIENT);
            modifiableProperties.Remove(MedicalRecordProperty.PRESCRIPTIONS);
            return modifiableProperties;
        }
        private static List<MedicalRecord> GetAllMedicalRecords()
        {
            return IS.Instance.Hospital.MedicalRecords.Where(
                a => !a.Deleted).ToList();
        }

        public static MedicalRecord GetPatientsMedicalRecord(Patient patient)
        {
            var allMedicalRecords = GetAllMedicalRecords();
            foreach (var medicalRecord in allMedicalRecords)
            {
                if (patient.Id == medicalRecord.Patient.Id)
                {
                    return medicalRecord;
                }

            }
            return null;
        }

        public static List<String> GetActionsPerformableOnList()
        {
            List<string> actions = new List<string>();
            actions.Add("ADD");
            actions.Add("REMOVE");
            return actions;
        }
        
        public static void CopyMedicalRecord(MedicalRecord target, MedicalRecord source, List<MedicalRecordProperty> whichProperties)
        {
            if (whichProperties.Contains(MedicalRecordProperty.HEIGHT)) target.Height = source.Height;
            if (whichProperties.Contains(MedicalRecordProperty.WEIGHT)) target.Weight = source.Weight;
            if (whichProperties.Contains(MedicalRecordProperty.ILLNESSES)) target.Illnesses = source.Illnesses;
            if (whichProperties.Contains(MedicalRecordProperty.OTHER_ALLERGIES)) target.OtherAllergies = source.OtherAllergies;
            if (whichProperties.Contains(MedicalRecordProperty.ALLERGIES_TO_INGREDIENTS)) target.IngredientAllergies = source.IngredientAllergies;
            if (whichProperties.Contains(MedicalRecordProperty.PRESCRIPTIONS)) target.Prescriptions = source.Prescriptions;
        }

        public static List<Appointment> MatchAppointmentByAnamnesis(string query, Appointment.AppointmentComparer comparer, Patient patient)
        {
            var matches = GetPatientsMedicalRecord(patient).Examinations.FindAll(
                e => e.ScheduledFor < DateTime.Now && e.Anamnesis.Trim().ToLower().Contains(query.Trim().ToLower()));
            matches.Sort(comparer);
            return matches;
        }
        public static void AddNotifsIfNecessary(UserAccount ua)
        {
            if (ua.Type == UserAccount.AccountType.PATIENT)
            {
                Patient p = PatientController.GetPatientFromPerson(ua.Person);
                MedicalRecord mr = MedicalRecordController.GetPatientsMedicalRecord(p);
                IS.Instance.MedicalRecordRepo.AddTask(mr);
            }
        }
    }
}