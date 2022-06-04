using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class ReferralModel
    {
        private const string hintSelectProperties = "Select whether you want to make referral based on exact doctor or specialty: ";
        private const string hintSelectDoctor = "Select a doctor for the referral: ";
        private const string hintSelectSpecialty = "Select a specialty for the referral: ";
        private const string hintReferralMade = "You've Successfully made a referral!";
        private const string hintSelectReferral = "Select referral: ";

        private enum ReferralProperty
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
                    newReferral.Doctor = InputDoctor(appointment, inputCancelString);
                    newReferral.Specialty = newReferral.Doctor.Specialty;
                }

                if (propertyToInput == ReferralProperty.SPECIALTY)
                {
                    newReferral.Doctor = null;
                    newReferral.Specialty = InputSpecialty(inputCancelString);
                }
                
                IS.Instance.ReferralRepo.Add(newReferral);
                Console.WriteLine(hintReferralMade);
            }

            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        private static List<ReferralProperty> GetAllReferralProperties()
        {
            return Enum.GetValues(typeof(ReferralProperty)).Cast<ReferralProperty>().ToList();
        }

        private static Doctor InputDoctor(Appointment appointment, string inputCancelString)
        {
            var availableDoctors = DoctorController.GetAvailableDoctors();
            availableDoctors.Remove(appointment.Doctor);
            Console.WriteLine(hintSelectDoctor);
            var chosenDoctor = EasyInput<Doctor>.Select(availableDoctors, inputCancelString);
            return chosenDoctor;
        }

        internal static Doctor.MedicineSpeciality InputSpecialty(string inputCancelString)
        {
            var specialties = DoctorController.GetAllSpecialties();
            Console.WriteLine(hintSelectSpecialty);
            var chosenSpecialty = EasyInput<Doctor.MedicineSpeciality>.Select(specialties, inputCancelString);
            return chosenSpecialty;

        }

        internal static void GetAllReferrals()
        {
            var allReferrals = IS.Instance.Hospital.Referrals.ToList();
            for (int i = 0; i < allReferrals.Count; i++)
            {
                var referral = allReferrals[i];
                Console.WriteLine(referral.ToString());
            }
        }
        
        internal static void HandleReferrals(string inputCancelString, UserAccount user)
        {
            try
            {
                Console.WriteLine(hintSelectReferral);
                AppointmentModel.CreateAppointmentWithReferral(SelectReferral(inputCancelString), inputCancelString, user);
            }
            catch (NothingToSelectException e)
            {
                Console.WriteLine(e);
            }
        }
        
        private static Referral SelectReferral(string inputCancelString)
        {   
            return EasyInput<Referral>.Select(GetUnusedReferrals(), inputCancelString);
        }
        
        private static List<Referral> GetUnusedReferrals()
        {
            return IS.Instance.Hospital.Referrals.Where(referral => !referral.Deleted && !referral.Scheduled).ToList();
        }
        
    }
}
