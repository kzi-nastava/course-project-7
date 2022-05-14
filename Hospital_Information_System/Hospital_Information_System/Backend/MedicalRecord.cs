using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HospitalIS.Backend
{
    public class MedicalRecord : Entity
    {
        [JsonConverter(typeof(Repository.PatientRepository.PatientReferenceConverter))]
        public Patient Patient { get; set; }

        public float Height { get; set; }

        public float Weight { get; set; }

        public List<String> Illnesses { get; set; }

        public List<String> Allergies { get; set; }

        [JsonConverter(typeof(Repository.AppointmentRepository.AppointmentListConverter))]
        public List<Appointment> Examinations { get; set; }

        public MedicalRecord()
        {
        }
        

        public MedicalRecord(Patient patient, float height, float weight, List<string> illnesses, List<string> allergies, List<Appointment> examinations)
        {
            Patient = patient;
            Height = height;
            Weight = weight;
            Illnesses = illnesses;
            Allergies = allergies;
            Examinations = examinations;
        }

        public MedicalRecord(Patient patient)
        {
            Patient = patient;
            Height = 0;
            Weight = 0;
            Illnesses = new List<string>();
            Allergies = new List<string>();
            Examinations = new List<Appointment>();
        }
        
        public override string ToString()
        {
            return $"MedicalRecord{{Id = {Id}, Patient = {Patient.Id}, Weight = {Weight}, Height = {Height}, Illnesses = {ConvertListToString(Illnesses)}, Allergies = {ConvertListToString(Allergies)}}}";
        }

        public static string ConvertListToString(List<String> entry)
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