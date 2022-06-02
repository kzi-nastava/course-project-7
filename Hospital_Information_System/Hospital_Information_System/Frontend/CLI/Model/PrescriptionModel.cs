using HospitalIS.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Controller;
using HospitalIS.Frontend.CLI.View;

namespace HospitalIS.Frontend.CLI.Model
{
	internal abstract class PrescriptionModel
    {
        private static string hintInputMedication = "Select medication for the prescription";
        private static string hintInputTimeOfUsage = "Input one time of usage [HH:mm]";

        private static string hintDangerousMedication =
            "You've selected an medication that contains the ingredients the patient is allergic to. Try again!";

        private static string hintInputUsage = "Select appropriate usage: ";
        private static string hintInputFrequencyOfUsage = "Input how many times a day is patient supposed to take the medication";
        private static string errFrequencyPositive = "Frequency of usage must be a positive number";
        private static string errFrequencyTooHigh = "Patient must not use the medication more than 5 times a day!";
        private static string hintPrescriptionCreated = "You've successfully created a prescription";

        internal static Prescription CreatePrescription(string inputCancelString, MedicalRecord oldMedicalRecord)
        {
            Prescription newPrescription = new Prescription();
            try
            {
                newPrescription.Medication = InputMedication(inputCancelString, oldMedicalRecord);
                newPrescription.Usage = InputUsage(inputCancelString);
                newPrescription.Frequency = InputFrequencyOfUsage(inputCancelString);
                newPrescription.TimesOfUsage = new List<TimeSpan>();
                for (int i = 0; i < newPrescription.Frequency; i++)
                {
                    TimeSpan timeOfUsage = InputTimeOfUsage(inputCancelString);
                    newPrescription.TimesOfUsage.Add(timeOfUsage);
                }
                IS.Instance.PrescriptionRepo.Add(newPrescription);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine(hintPrescriptionCreated);
            MedicalRecord updatedMedicalRecord = oldMedicalRecord;
                updatedMedicalRecord.Prescriptions.Add(newPrescription);
            //List<MedicalRecordController.MedicalRecordProperty> properties = MedicalRecordController.GetAllMedicalRecordProperties();
            //MedicalRecordController.CopyMedicalRecord(oldMedicalRecord, updatedMedicalRecord, properties);
            List<MedicalRecordController.MedicalRecordProperty> prescriptionProperty =
                new List<MedicalRecordController.MedicalRecordProperty>();
            prescriptionProperty.Add(MedicalRecordController.MedicalRecordProperty.PRESCRIPTIONS);
            MedicalRecordController.CopyMedicalRecord(oldMedicalRecord, updatedMedicalRecord, prescriptionProperty);
            return newPrescription;
        }

        internal static Medication InputMedication(string inputCancelString, MedicalRecord patientsMedicalRecord)
        {
            List<Medication> medications = GetAllMedications();
            while (true)
            {
                Console.WriteLine(hintInputMedication);
                var selectedMedication = EasyInput<Medication>.Select(medications, inputCancelString);
                bool medicationIsSafe = IsMedicationSafe(selectedMedication.Ingredients,
                    patientsMedicalRecord.IngredientAllergies);
                if (!medicationIsSafe)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(hintDangerousMedication);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    continue;
                }
                return selectedMedication;
            }
        }

        internal static List<Medication> GetAllMedications()
        {
            return IS.Instance.Hospital.Medications.Where(
                a => !a.Deleted).ToList();
        }

        internal static bool IsMedicationSafe(List<Ingredient> medicationIngredients,
            List<Ingredient> patientsAllergies)
        {
            for (int i = 0; i < medicationIngredients.Count(); i++)
            {
                for (int j = 0; j < patientsAllergies.Count(); j++)
                {
                    if (medicationIngredients[i] == patientsAllergies[j])
                    {
                        return false; 
                    }
                }
            }
            return true;
        }

        internal static Prescription.UsageTypes InputUsage(string inputCancelString)
        {
            Console.WriteLine(hintInputUsage);
            var selectedUsage = EasyInput<Prescription.UsageTypes>.Select(GetAllMedicationUsages(), inputCancelString);
            return selectedUsage;
        }
        
        internal static List<Prescription.UsageTypes> GetAllMedicationUsages()
        {
            return Enum.GetValues(typeof(Prescription.UsageTypes)).Cast<Prescription.UsageTypes>().ToList();
        }

        internal static int InputFrequencyOfUsage(string inputCancelString)
        {
            Console.WriteLine(hintInputFrequencyOfUsage);
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
                inputCancelString
            );
        }

        internal static TimeSpan InputTimeOfUsage(String inputCancelString)
        {
            Console.WriteLine(hintInputTimeOfUsage);
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
                inputCancelString,
                TimeSpan.Parse);
        }
    }
}