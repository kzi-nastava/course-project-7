using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Prescription : Entity
    {
        [JsonConverter(typeof(Repository.MedicationRepository.MedicationReferenceConverter))]
        public Medication Medication { get; set; }
        
        public enum UsageTypes
        {
            BEFORE_MEAL, DURING_MEAL, AFTER_MEAL, NOT_SPECIFIED
        }
        
        public UsageTypes Usage { get; set; }
        public int Frequency { get; set; }

        public List<TimeSpan> TimesOfUsage { get; set; }

        public Prescription()
        {
        }

        public Prescription(Medication medication, UsageTypes usage, int frequency, List<TimeSpan> timesOfUsage)
        {
            this.Medication = medication;
            this.Usage = usage;
            this.Frequency = frequency;
            this.TimesOfUsage = timesOfUsage;
        }
        
        public override string ToString()
        {
            return $"Prescription{{Id = {Id}, Medicine = {Medication.Name}, Usage = {Usage}, FrequencyOfUsage = {Frequency}, TimesOfUsage = {TimesOfUsage}}}";
        }
        
        public static string PrescriptionsToString(List<Prescription> entry)
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