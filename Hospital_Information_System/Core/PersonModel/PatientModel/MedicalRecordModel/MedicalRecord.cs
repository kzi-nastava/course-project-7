using HIS.Core.AppointmentModel;
using HIS.Core.Foundation;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.PrescriptionModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel.PatientModel.MedicalRecordModel
{
    public class MedicalRecord : Entity
    {
        [JsonConverter(typeof(PatientJSONReferenceConverter))]
        public Patient Patient { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public List<String> Illnesses { get; set; }

        [JsonConverter(typeof(IngredientJSONIListConverter))]
        public List<Ingredient> IngredientAllergies { get; set; }

        public List<String> OtherAllergies { get; set; }

        [JsonConverter(typeof(PrescriptionJSONIListConverter))]
        public List<Prescription> Prescriptions { get; set; }

        [JsonConverter(typeof(AppointmentJSONIListConverter))]
        public List<Appointment> Examinations { get; set; }

        public int MinutesBeforeNotification { get; set; }

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
            return $"MedicalRecord{{Id = {Id}, Patient = {Patient.Id}, Weight = {Weight}, Height = {Height}, Illnesses = {ListToString(Illnesses)}, AllergiesToIngredients = {Ingredient.IngredientsToString(IngredientAllergies)}, OtherAllergies = {ListToString(OtherAllergies)}, Prescriptions = {Prescription.PrescriptionsToString(Prescriptions)}}}";
        }

        private static string ListToString(List<String> entry)
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
    }
}
