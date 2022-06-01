using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class Referral : Entity
    {
        // Sometimes the exact doctor is specified
        [JsonConverter(typeof(Repository.DoctorRepository.DoctorReferenceConverter))]
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public Doctor Doctor { get; set; }
        
        // Sometimes we just know the field
        public Doctor.MedicineSpeciality Specialty{ get; set; }
        
        [JsonConverter(typeof(Repository.PatientRepository.PatientReferenceConverter))]
        public Patient Patient { get; set; }
        
        public bool Scheduled { get; set; }

        public Referral()
        {
        }

        public Referral(Doctor doctor, Doctor.MedicineSpeciality specialty, Patient patient, bool scheduled = false)
        {
            Doctor = doctor;
            Specialty = specialty;
            Patient = patient;
            Scheduled = scheduled;
        }

        public override string ToString()
        {
            if (Doctor == null)
            {
                return $"Referral{{Id = {Id}, Doctor = Not specified, Specialty = {Specialty}, Patient = {Patient.Id}}}";
            }
            return $"Referral{{Id = {Id}, Doctor = {Doctor.Id}, Specialty = {Specialty}, Patient = {Patient.Id}}}";
        }
    }
    
    
}