using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.AppointmentModel.Util;
using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.PrescriptionModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;

namespace HIS.CLI.View
{
    internal class PrescriptionView : AbstractView
    {
        private IPrescriptionService _service;
        private IMedicalRecordService _medicalRecordService;
        private IMedicationService _medicationService;
        
        private static string hintInputMedication = "Select medication for the prescription";
        private static string hintInputTimeOfUsage = "Input one time of usage [HH:mm]";

        private static string hintDangerousMedication =
            "You've selected an medication that contains the ingredients the patient is allergic to. Try again!";

        private static string hintInputUsage = "Select appropriate usage: ";
        private static string hintInputFrequencyOfUsage = "Input how many times a day is patient supposed to take the medication";
        private static string errFrequencyPositive = "Frequency of usage must be a positive number";
        private static string errFrequencyTooHigh = "Patient must not use the medication more than 5 times a day!";
        private static string hintPrescriptionCreated = "You've successfully created a prescription";

        public PrescriptionView(IPrescriptionService service, IMedicalRecordService medicalRecordService, IMedicationService medicationService)
        {
            _service = service;
            _medicalRecordService = medicalRecordService;
            _medicationService = medicationService;
        }
        internal Prescription CreatePrescription(MedicalRecord oldMedicalRecord)
        {
            Prescription newPrescription = new Prescription();
            try
            {
                newPrescription.Medication = InputMedication(oldMedicalRecord);
                newPrescription.Usage = InputUsage();
                newPrescription.Frequency = InputFrequencyOfUsage();
                newPrescription.TimesOfUsage = new List<TimeSpan>();
                for (int i = 0; i < newPrescription.Frequency; i++)
                {
                    TimeSpan timeOfUsage = InputTimeOfUsage();
                    newPrescription.TimesOfUsage.Add(timeOfUsage);
                }
                _service.Add(newPrescription);
            }
            catch (Exception e)
            {
                Error(e.Message);
            }
            Hint(hintPrescriptionCreated);
            
            MedicalRecord updatedMedicalRecord = oldMedicalRecord;
            updatedMedicalRecord.Prescriptions.Add(newPrescription);
            var prescriptionProperty = _medicalRecordService.GetPrescriptionProperty();
            _medicalRecordService.Copy(oldMedicalRecord, updatedMedicalRecord, prescriptionProperty);
            return newPrescription;
        }

        private Medication InputMedication(MedicalRecord patientsMedicalRecord)
        {
            List<Medication> medications = _medicationService.GetAll().ToList();
            while (true)
            {
                Hint(hintInputMedication);
                Medication selectedMedication = EasyInput<Medication>.Select(medications, _cancel);
                bool medicationIsSafe = _medicationService.IsMedicationSafe((List<Ingredient>)selectedMedication.Ingredients,
                    patientsMedicalRecord.IngredientAllergies);
                if (!medicationIsSafe)
                {
                    Error(hintDangerousMedication);
                    continue;
                }
                return selectedMedication;
            }
        }


        private Prescription.UsageTypes InputUsage()
        {
            Hint(hintInputUsage);
            var selectedUsage = EasyInput<Prescription.UsageTypes>.Select(_service.GetAllMedicationUsages(), _cancel);
            return selectedUsage;
        }

        private int InputFrequencyOfUsage()
        {
            Hint(hintInputFrequencyOfUsage);
            return EasyInput<int>.Get(
                new List<Func<int, bool>>
                {
                    s => s > 0,
                    s => s < 5,
                },
                new[]
                {
                    errFrequencyPositive,
                    errFrequencyTooHigh,
                },
                _cancel
            );
        }

        private TimeSpan InputTimeOfUsage()
        {
            Hint(hintInputTimeOfUsage);
            return EasyInput<TimeSpan>.Get(
                new List<Func<TimeSpan, bool>>()
                {
                    ts => AppointmentSearchBundle.TsInDay(ts),
                    ts => AppointmentSearchBundle.TsZeroSeconds(ts),
                },
                new string[]
                {
                    AppointmentSearchBundle.ErrTimeSpanNotInDay,
                    AppointmentSearchBundle.ErrTimeSpanHasSeconds,
                },
                _cancel,
                TimeSpan.Parse);
        }
    }
}