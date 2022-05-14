using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Prescription : Entity
    {
        [JsonConverter(typeof(Repository.MedicationRepository.MedicationReferenceConverter))]
        public Medication Medication { get; set; }
        public int Frequency { get; set; }

        public enum UsageTypes
        {
            BEFORE_MEAL, DURING_MEAL, AFTER_MEAL, NOT_SPECIFIED
        }
        
        public UsageTypes Usage { get; set; }

        public Prescription()
        {
        }

        public Prescription(Medication medication, int frequency, UsageTypes usage)
        {
            this.Medication = medication;
            this.Frequency = frequency;
            this.Usage = usage;
        }
        
        public override string ToString()
        {
            return $"Prescription{{Id = {Id}, Medicine = {Medication.Name}, FrequencyOfUsage = {Frequency}, Usage = {Usage}}}";
        }
    }
}