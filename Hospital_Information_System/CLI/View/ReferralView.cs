using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel.ReferralModel;

namespace HIS.CLI.View
{
    internal class ReferralView : AbstractView
    {

        private readonly IReferralService _service;
        private readonly IDoctorService _doctorService;
        
        private const string hintSelectProperties = "Select whether you want to make referral based on exact doctor or specialty: ";
        private const string hintSelectDoctor = "Select a doctor for the referral: ";
        private const string hintSelectSpecialty = "Select a specialty for the referral: ";
        private const string hintReferralMade = "You've Successfully made a referral!";

        public ReferralView(IReferralService service, IDoctorService doctorService)
        {
            _service = service;
            _doctorService = doctorService;
        }
        
         internal void CreateReferral(Appointment appointment)
        {
            try
            {
                Referral newReferral = new Referral();
                newReferral.Patient = appointment.Patient;
                List<ReferralProperty> properties = _service.GetAllReferralProperties();
                Hint(hintSelectProperties);
                var propertyToInput = EasyInput<ReferralProperty>.Select(properties, _cancel);
                if (propertyToInput == ReferralProperty.DOCTOR)
                {
                    newReferral.Doctor = InputDoctor(appointment);
                    newReferral.Specialty = newReferral.Doctor.Specialty;
                }

                if (propertyToInput == ReferralProperty.SPECIALTY)
                {
                    newReferral.Doctor = null;
                    newReferral.Specialty = InputSpecialty();
                }

                _service.Add(newReferral);
                Hint(hintReferralMade);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

         private Doctor InputDoctor(Appointment appointment)
         { 
             var availableDoctors = _doctorService.GetAll().ToList();
            availableDoctors.Remove(appointment.Doctor);
            Hint(hintSelectDoctor);
            var chosenDoctor = EasyInput<Doctor>.Select(availableDoctors, _cancel);
            return chosenDoctor;
        }

        internal Doctor.MedicineSpeciality InputSpecialty()
        {
            var specialties = _doctorService.GetAllSpecialties();
            Hint(hintSelectSpecialty);
            var chosenSpecialty = EasyInput<Doctor.MedicineSpeciality>.Select(specialties, _cancel);
            return chosenSpecialty;

        }
        
    }
}