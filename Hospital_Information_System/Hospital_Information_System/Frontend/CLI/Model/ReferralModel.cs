using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class ReferralModel
    {
        private const string hintSelectProperties = "Select whether you want to make referral based on exact doctor or specialty: ";
        private const string hintSelectDoctor = "Select a doctor for the referral: ";
        private const string hintSelectSpecialty = "Select a specialty for the referral: ";
        private const string hintReferralMade = "You've Successfully made a referral!";

        public enum ReferralProperty
        {
            DOCTOR,
            SPECIALTY
        }
        internal static void CreateReferral(Appointment appointment, string inputCancelString)
        {
            try
            {
                Referral newReferral = new Referral();
                newReferral.Patient = appointment.Patient;
                List<ReferralProperty> properties = GetAllReferralProperties();
                Console.WriteLine(hintSelectProperties);
                var propertyToInput = EasyInput<ReferralProperty>.Select(properties, inputCancelString);
                if (propertyToInput == ReferralProperty.DOCTOR)
                {
                    newReferral.Doctor = inputDoctor(appointment, inputCancelString);
                    newReferral.Specialty = newReferral.Doctor.Specialty;
                }

                if (propertyToInput == ReferralProperty.SPECIALTY)
                {
                    newReferral.Doctor = null;
                    newReferral.Specialty = inputSpecialty(inputCancelString);
                }
                
                IS.Instance.ReferralRepo.Add(newReferral);
                Console.WriteLine(hintReferralMade);
            }

            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        internal static List<ReferralProperty> GetAllReferralProperties()
        {
            return Enum.GetValues(typeof(ReferralProperty)).Cast<ReferralProperty>().ToList();
        }

        internal static Doctor inputDoctor(Appointment appointment, string inputCancelString)
        {
            var availableDoctos = GetAvailableDoctors();
            availableDoctos.Remove(appointment.Doctor);
            Console.WriteLine(hintSelectDoctor);
            var chosenDoctor = EasyInput<Doctor>.Select(availableDoctos, inputCancelString);
            return chosenDoctor;
        }

        internal static List<Doctor> GetAvailableDoctors()
        {
            return IS.Instance.Hospital.Doctors.ToList();
        }
        
        internal static Doctor.MedicineSpeciality inputSpecialty(string inputCancelString)
        {
            var specialties = GetAllSpecialties();
            Console.WriteLine(hintSelectSpecialty);
            var chosenSpecialty = EasyInput<Doctor.MedicineSpeciality>.Select(specialties, inputCancelString);
            return chosenSpecialty;

        }
        
        internal static List<Doctor.MedicineSpeciality> GetAllSpecialties()
        {
            return Enum.GetValues(typeof(Doctor.MedicineSpeciality)).Cast<Doctor.MedicineSpeciality>().ToList();
        }
        
        
        
        
    }
}
