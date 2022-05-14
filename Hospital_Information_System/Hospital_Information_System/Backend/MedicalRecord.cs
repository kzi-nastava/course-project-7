using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using HospitalIS.Backend.Repository;

namespace HospitalIS.Backend
{
    public class MedicalRecord : Entity
    {
        [JsonConverter(typeof(Repository.PatientRepository.PatientReferenceConverter))]
        public Patient Patient { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public List<String> Illnesses { get; set; }

        [JsonConverter(typeof(IngredientRepository.IngredientListConverter))]
        public List<Ingredient> IngredientAllergies { get; set; }

        public List<String> OtherAllergies { get; set; }
        
        [JsonConverter(typeof(PrescriptionRepository.PrescriptionListConverter))]
        public List<Prescription> Prescriptions { get; set; }

        [JsonConverter(typeof(Repository.AppointmentRepository.AppointmentListConverter))]
        public List<Appointment> Examinations { get; set; }

        public MedicalRecord()
        {
        }
        

        public MedicalRecord(Patient patient, float height, float weight, List<string> illnesses, List<Ingredient> ingredientAllergies, List<string> otherAllergies, List<Prescription> prescriptions, List<Appointment> examinations)
        {
            Patient = patient;
            Height = height;
            Weight = weight;
            Illnesses = illnesses;
            IngredientAllergies = ingredientAllergies;
            OtherAllergies = otherAllergies;
            Prescriptions = prescriptions;
            Examinations = examinations;
        }

        public MedicalRecord(Patient patient)
        {
            Patient = patient;
            Height = 0;
            Weight = 0;
            Illnesses = new List<string>();
            IngredientAllergies = new List<Ingredient>();
            OtherAllergies = new List<string>();
            Prescriptions = new List<Prescription>();
            Examinations = new List<Appointment>();
        }
        
        public override string ToString()
        {
            return $"MedicalRecord{{Id = {Id}, Patient = {Patient.Id}, Weight = {Weight}, Height = {Height}, Illnesses = {ConvertStringListToString(Illnesses)}, AllergiesToIngredients = {Medication.ConvertIngredientListToString(IngredientAllergies)}, OtherAllergies = {ConvertStringListToString(OtherAllergies)}, Prescriptions = {ConvertPrescriptionListToString(Prescriptions)}}}";
        }

        public static string ConvertStringListToString(List<String> entry)
        {
            string result = "";
            for (int i = 0; i <= entry.Count - 1; i++)
            {
                result += entry[i];
                if (i < entry.Count - 1)
                {
                    result += ", ";
                }
            }

            return result;
        }
        private static string ConvertPrescriptionListToString(List<Prescription> entry)
        {
            string result = "";
            for (int i = 0; i <= entry.Count - 1; i++)
            {
                result += entry[i].Medication.Name;
                if (i < entry.Count - 1)
                {
                    result += ", ";
                }
            }

            return result;
        }
    }
}