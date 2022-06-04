using System;
using System.Collections.Generic;
using System.Linq;
using static HospitalIS.Backend.Controller.MedicalRecordController;
using static HospitalIS.Backend.Controller.AppointmentController;
using HospitalIS.Backend;
using System.Diagnostics;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class MedicalRecordModel
    {
        private const string hintInputWeight = "Input patient's weight [kg]";
        private const string hintInputHeight = "Input patient's height [cm]";

        private const string hintInputIllnesses =
            "Input patient's illnesses one by one\nEnter q to finish";

        private const string hintInputIllness = "Input illness:";

        private const string hintInputAllergies =
            "Input patient's allergies one by one\nEnter q to finish";

        private const string hintInputAllergy = "Input allegie:";
        private const string hintInputIngredients = "Input ingredients: ";

        private const string hintSelectProperties =
            "Select properties by their number, separated by whitespace.\nEnter a newline to finish";

        private const string errWeightPositive = "Weight must be a positive number!";
        private const string errWeightTooHigh = "Weight must be lower than 500kg!";
        private const string errWeightTooLow = "Weight must be higher than 5kg!";
        private const string errHeightPositive = "Height must be a positive number!";
        private const string errHeightTooHigh = "Height must be lower than 250cm!";
        private const string errHeightTooLow = "Height must be higher than 50cm!";
        private const string hintNoMedicalRecord = "This patient doesn't have a medical record yet.";
        private const string hintMedicalRecordUpdated = "You've successfully updated patient's medical record!";
        private const string hintSelectAction = "Select the action that you want to perform";
        private const string hintInputNew = "Input the item you want to add, or newLine to finish";
        private const string hintSelectToRemove = "Select items you want removed";
        private const string hintUpdatingCurrentAllergies = "Updating current list of allergies";
        private const string hintUpdatingCurrentIllnesses = "Updating current list of illnesses";

        private const string hintSearchSortBy = "Enter sorting criteria";
        private const string hintSearchQuery = "Enter search query";

        private const string hintGetMinutes = "Enter how many minutes before the scheduled medication time you want to receive a notification.";
        private const string errMinutesMustBePositiveNumber = "Minutes must be a positive number!";

        private static void CreateMedicalRecord(Patient patient, string inputCancelString)
        {
            try
            {
                MedicalRecord medicalRecord = new MedicalRecord();
                medicalRecord.Patient = patient;
                medicalRecord.Weight = InputWeight(inputCancelString);
                medicalRecord.Height = InputHeight(inputCancelString);
                medicalRecord.Illnesses = InputIllnesses();
                medicalRecord.IngredientAllergies = InputIngredients(inputCancelString);
                medicalRecord.OtherAllergies = InputAllergies();
                medicalRecord.Prescriptions = new List<Prescription>();

                medicalRecord.Examinations = GetAppointments(patient);

                IS.Instance.MedicalRecordRepo.Add(medicalRecord);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        internal static void ReadMedicalRecord(Appointment appointment, string inputCancelString)
        {
            MedicalRecord medicalRecord = GetPatientsMedicalRecord(appointment.Patient);

            if (medicalRecord is null)
            {
                Console.WriteLine(hintNoMedicalRecord);
            }
            else
            {
                Console.WriteLine(medicalRecord);
            }
        }

        internal static void UpdateMedicalRecord(Patient patient, string inputCancelString)
        {
            MedicalRecord medicalRecord = GetPatientsMedicalRecord(patient);
            if (medicalRecord is null)
            {
                CreateMedicalRecord(patient, inputCancelString);
            }
            else
            {
                UpdateMedicalRecord(medicalRecord, inputCancelString);
            }

        }

        private static void UpdateMedicalRecord(MedicalRecord medicalRecord, string inputCancelString)
        {
            Console.WriteLine(hintSelectProperties);
            try
            {
                List<MedicalRecordProperty> modifiableProperties = GetModifiableProperties();
                var propertiesToUpdate = EasyInput<MedicalRecordProperty>.SelectMultiple(
                    modifiableProperties,
                    ap => GetAppointmentPropertyName(ap),
                    inputCancelString
                ).ToList();
                var updatedMedialRecord =
                    GetUpdatedMedicalRecord(inputCancelString, propertiesToUpdate, medicalRecord);
                CopyMedicalRecord(medicalRecord, updatedMedialRecord, propertiesToUpdate);
                Console.WriteLine(hintMedicalRecordUpdated);
            }
                
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static float InputWeight(string inputCancelString)
        {
            Console.WriteLine(hintInputWeight);
            return EasyInput<float>.Get(
                new List<Func<float, bool>>
                {
                    s => s > 0.0,
                    s => s > 5.0,
                    s => s < 500.0,
                },
                new[]
                {
                    errWeightPositive,
                    errWeightTooLow,
                    errWeightTooHigh,
                },
                inputCancelString
            );
        }

        private static float InputHeight(string inputCancelString)
        {
            Console.WriteLine(hintInputHeight);
            return EasyInput<float>.Get(
                new List<Func<float, bool>>
                {
                    s => s > 0,
                    s => s > 50,
                    s => s < 250,
                },
                new[]
                {
                    errHeightPositive,
                    errHeightTooLow,
                    errHeightTooHigh,
                },
                inputCancelString
            );
        }

        private static List<String> InputIllnesses()
        {
            List<String> illnesses = new List<string>();
            Console.WriteLine(hintInputIllnesses);
            while (true)
            {
                Console.WriteLine(hintInputIllness);
                string illness = Console.ReadLine();
                if (illness.ToLower() == "q")
                {
                    break;
                }

                illnesses.Add(illness);
            }

            return illnesses;
        }


        private static List<String> InputAllergies()
        {
            List<String> allergies = new List<string>();
            Console.WriteLine(hintInputAllergies);
            while (true)
            {
                Console.WriteLine(hintInputAllergy);
                string allergy = Console.ReadLine();
                if (allergy.ToLower() == "q" || allergy == "")
                {
                    break;
                }

                allergies.Add(allergy);
            }

            return allergies;
        }
        
        private static List<Ingredient> InputIngredients(string inputCancelString)
        {
            Console.WriteLine(hintInputIngredients);
            List<Ingredient> ingredients = GetAllIngredients();
            var selectedIngredients = EasyInput<Ingredient>.SelectMultiple(ingredients, inputCancelString).ToList();
            return selectedIngredients;
        }

        private static List<Ingredient> GetAllIngredients()
        {
            return IS.Instance.Hospital.Ingredients.Where(
                a => !a.Deleted).ToList();
        }

        private static MedicalRecord GetUpdatedMedicalRecord(string inputCancelString,
            List<MedicalRecordProperty> propertiesToUpdate, MedicalRecord oldMedicalRecord)
        {
            MedicalRecord updatedMedicalRecord = oldMedicalRecord;
            if (propertiesToUpdate.Contains(MedicalRecordProperty.HEIGHT))
            {
                updatedMedicalRecord.Height = InputHeight(inputCancelString);
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.WEIGHT))
            {
                updatedMedicalRecord.Weight = InputWeight(inputCancelString);
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.OTHER_ALLERGIES))
            {
                Console.WriteLine(hintUpdatingCurrentAllergies);
                updatedMedicalRecord.OtherAllergies = PerformActionsOnList(oldMedicalRecord.OtherAllergies, inputCancelString);
            }
            if (propertiesToUpdate.Contains(MedicalRecordProperty.ALLERGIES_TO_INGREDIENTS))
            {
                Console.WriteLine(hintUpdatingCurrentAllergies);
                updatedMedicalRecord.IngredientAllergies = InputIngredients(inputCancelString);
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.ILLNESSES))
            {
                Console.WriteLine(hintUpdatingCurrentIllnesses);
                updatedMedicalRecord.Illnesses = PerformActionsOnList(oldMedicalRecord.Illnesses, inputCancelString);
            }

            return updatedMedicalRecord;
        }

        private static List<String> PerformActionsOnList(List<String> properties, string inputCancelString)
        {
            Console.WriteLine(hintSelectAction);
            List<string> allActions = GetActionsPerformableOnList();
            var actionsToPerform = EasyInput<String>.SelectMultiple(allActions, inputCancelString);
            List<string> updatedProperties = new List<string>();
            foreach (var action in actionsToPerform)
            {
                if (action == "ADD")
                {
                    updatedProperties = Add(properties);
                }
                else
                {
                    updatedProperties = Remove(properties, inputCancelString);
                }

            }

            return updatedProperties;
        }


        private static List<String> Add(List<String> oldList)
        {
            List<String> updatedList = oldList;
            while (true)
            {
                Console.WriteLine(hintInputNew);
                string newItem = Console.ReadLine();
                if (newItem != "")
                {
                    updatedList.Add(newItem);
                }

                else
                {
                    break;
                }
            }

            return updatedList;
        }

        private static List<String> Remove(List<String> oldList, string inputCancelString)
        {
            List<String> updatedList = oldList;
            Console.WriteLine(hintSelectToRemove);
            var itemsToRemove = EasyInput<String>.SelectMultiple(oldList, inputCancelString);
            foreach (var item in itemsToRemove)
            {
                updatedList.Remove(item);
            }

            return updatedList;

        }

        internal static void Search(UserAccount user, string inputCancelString)
        {
            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            Console.WriteLine(hintSearchQuery);
            string query = Console.ReadLine();

            var sortBy = new Dictionary<string, Appointment.AppointmentComparer>()
            {
                ["Sort by date"] = new Appointment.CompareByDate(),
                ["Sort by doctor"] = new Appointment.CompareByDoctor(),
                ["Sort by doctor specialty"] = new Appointment.CompareByDoctorSpecialty(),
            };
            Console.WriteLine(hintSearchSortBy);
            var sortChoice = EasyInput<string>.Select(sortBy.Keys.ToList(), inputCancelString);

            Patient patient = IS.Instance.Hospital.Patients.Find(p => p.Person == user.Person);
            List<Appointment> matches = MatchAppointmentByAnamnesis(query, sortBy[sortChoice], patient);

            foreach (Appointment match in matches)
            {
                Console.WriteLine(match.AnamnesisFocusedToString());
            }
        }

        internal static void ChangeMinutesBeforeNotification(UserAccount user, string inputCancelString)
        {
            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            Patient patient = PatientController.GetPatientFromPerson(user.Person);
            Debug.Assert(patient != null);
            MedicalRecord record = GetPatientsMedicalRecord(patient);
            Debug.Assert(record != null);

            Console.WriteLine(hintGetMinutes);
            int newMinutes = EasyInput<int>.Get(
                new List<Func<int, bool>> { m => m > 0 },
                new string[] { errMinutesMustBePositiveNumber, },
                inputCancelString);

            record.MinutesBeforeNotification = newMinutes;
        }
    }
}