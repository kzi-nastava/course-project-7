using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.PrescriptionModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel.Util;

namespace HIS.CLI.View
{
	internal class MedicalRecordView : AbstractView
	{
		private readonly IMedicalRecordService _service;
		private readonly IPatientService _patientService;
		private readonly IAppointmentService _appointmentService;
		private readonly IIngredientService _ingredientService;

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
		private const string hintUpdatingCurrentAllergies = "Updating current list of allergies";
		private const string hintUpdatingCurrentIllnesses = "Updating current list of illnesses";

		private const string hintSearchSortBy = "Enter sorting criteria";
		private const string hintSearchQuery = "Enter search query";

		private const string hintGetMinutes = "Enter how many minutes before the scheduled medication time you want to receive a notification.";
		private const string errMinutesMustBePositiveNumber = "Minutes must be a positive number!";
		
		private const string hintSelectAction = "Select the action that you want to perform";
		private const string hintInputNew = "Input the item you want to add, or newLine to finish";
		private const string hintSelectToRemove = "Select items you want removed";

		public MedicalRecordView(IMedicalRecordService service, IPatientService patientService, IAppointmentService appointmentService, IIngredientService ingredientService)
		{
			_service = service;
			_patientService = patientService;
			_appointmentService = appointmentService;
			_ingredientService = ingredientService;
		}

		internal void CmdSearch()
		{
			if (User.Type != UserAccount.AccountType.PATIENT)
			{
				return;
			}

			Hint(hintSearchQuery);
			string query = Console.ReadLine();

			var sortBy = new Dictionary<string, AppointmentComparer>()
			{
				["Sort by date"] = new AppointmentCompareByDate(),
				["Sort by doctor"] = new AppointmentCompareByDoctor(),
				["Sort by doctor specialty"] = new AppointmentCompareByDoctorSpecialty(),
			};
			Hint(hintSearchSortBy);
			var sortChoice = EasyInput<string>.Select(sortBy.Keys.ToList(), _cancel);

			Patient patient = _patientService.GetPatientFromPerson(User.Person);
			List<Appointment> matches = _service.MatchAppointmentByAnamnesis(query, sortBy[sortChoice], patient).ToList();

			foreach (Appointment match in matches)
			{
				Print(match.AnamnesisFocusedToString());
			}
		}

		internal void CmdChangeMinutesBeforeNotification()
		{
			if (User.Type != UserAccount.AccountType.PATIENT)
			{
				return;
			}

			Patient patient = _patientService.GetPatientFromPerson(User.Person);
			Debug.Assert(patient != null);
			MedicalRecord record = _service.GetPatientsMedicalRecord(patient);
			Debug.Assert(record != null);

			Hint(hintGetMinutes);
			int newMinutes = EasyInput<int>.Get(
				new List<Func<int, bool>> { m => m > 0 },
				new string[] { errMinutesMustBePositiveNumber, },
				_cancel);

			record.MinutesBeforeNotification = newMinutes;
		}
		
		internal void ReadMedicalRecord(Appointment appointment)
		{
			MedicalRecord medicalRecord = _service.GetPatientsMedicalRecord(appointment.Patient);

			if (medicalRecord is null)
			{
				Hint(hintNoMedicalRecord);
			}
			else
			{
				Print(medicalRecord.ToString());
			}
		}
		
		private void CreateMedicalRecord(Patient patient)
		{
			try
			{
				MedicalRecord medicalRecord = new MedicalRecord();
				medicalRecord.Patient = patient;
				medicalRecord.Weight = InputWeight();
				medicalRecord.Height = InputHeight();
				medicalRecord.Illnesses = InputIllnesses();
				medicalRecord.IngredientAllergies = InputIngredients();
				medicalRecord.OtherAllergies = InputAllergies();
				medicalRecord.Prescriptions = new List<Prescription>();

				medicalRecord.Examinations = _appointmentService.GetAll(patient).ToList();

				_service.Add(medicalRecord);
			}
			catch (Exception e)
			{
				Error(e.Message);
			}
		}
		
		internal void UpdateMedicalRecord(Patient patient)
        {
            MedicalRecord medicalRecord = _service.GetPatientsMedicalRecord(patient);
            if (medicalRecord is null)
            {
                CreateMedicalRecord(patient);
            }
            else
            {
                UpdateMedicalRecord(medicalRecord);
            }

        }

        private void UpdateMedicalRecord(MedicalRecord medicalRecord)
        {
            Hint(hintSelectProperties);
            try
            {
                var modifiableProperties = MedicalRecordPropertyHelpers.GetModifiableProperties();
                var propertiesToUpdate = EasyInput<MedicalRecordProperty>.SelectMultiple(
                    modifiableProperties,
                    ap => MedicalRecordPropertyHelpers.GetName(ap),
                    _cancel
                ).ToList();
                var updatedMedialRecord =
                    GetUpdatedMedicalRecord(propertiesToUpdate, medicalRecord);
                _service.Copy(medicalRecord, updatedMedialRecord, propertiesToUpdate);
                Hint(hintMedicalRecordUpdated);
            }
                
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private float InputWeight()
        {
            Hint(hintInputWeight);
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
                _cancel
            );
        }

        private float InputHeight()
        {
            Hint(hintInputHeight);
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
                _cancel
            );
        }

        private List<String> InputIllnesses()
        {
            List<String> illnesses = new List<string>();
            Hint(hintInputIllnesses);
            while (true)
            {
                Hint(hintInputIllness);
                string illness = Console.ReadLine();
                if (illness.ToLower() == "q")
                {
                    break;
                }

                illnesses.Add(illness);
            }

            return illnesses;
        }


        private List<String> InputAllergies()
        {
            List<String> allergies = new List<string>();
            Hint(hintInputAllergies);
            while (true)
            {
                Hint(hintInputAllergy);
                string allergy = Console.ReadLine();
                if (allergy.ToLower() == "q" || allergy == "")
                {
                    break;
                }

                allergies.Add(allergy);
            }

            return allergies;
        }
        
        private List<Ingredient> InputIngredients()
        {
            Hint(hintInputIngredients);
            List<Ingredient> ingredients = _ingredientService.GetAll().ToList();
            var selectedIngredients = EasyInput<Ingredient>.SelectMultiple(ingredients, _cancel).ToList();
            return selectedIngredients;
        }

        private MedicalRecord GetUpdatedMedicalRecord(List<MedicalRecordProperty> propertiesToUpdate, MedicalRecord oldMedicalRecord)
        {
            MedicalRecord updatedMedicalRecord = oldMedicalRecord;
            if (propertiesToUpdate.Contains(MedicalRecordProperty.HEIGHT))
            {
                updatedMedicalRecord.Height = InputHeight();
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.WEIGHT))
            {
                updatedMedicalRecord.Weight = InputWeight();
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.OTHER_ALLERGIES))
            {
                Hint(hintUpdatingCurrentAllergies);
                updatedMedicalRecord.OtherAllergies = PerformActionsOnList(oldMedicalRecord.OtherAllergies);
            }
            if (propertiesToUpdate.Contains(MedicalRecordProperty.ALLERGIES_TO_INGREDIENTS))
            {
                Hint(hintUpdatingCurrentAllergies);
                updatedMedicalRecord.IngredientAllergies = InputIngredients();
            }

            if (propertiesToUpdate.Contains(MedicalRecordProperty.ILLNESSES))
            {
                Hint(hintUpdatingCurrentIllnesses);
                updatedMedicalRecord.Illnesses = PerformActionsOnList(oldMedicalRecord.Illnesses);
            }

            return updatedMedicalRecord;
        }

        private List<String> PerformActionsOnList(List<String> properties)
        {
	        Hint(hintSelectAction);
	        List<string> allActions = _service.GetActionsPerformableOnList();
	        var actionsToPerform = EasyInput<String>.SelectMultiple(allActions, _cancel);
	        List<string> updatedProperties = new List<string>();
	        foreach (var action in actionsToPerform)
	        {
		        if (action == "ADD")
		        {
			        updatedProperties = Add(properties);
		        }
		        else
		        {
			        updatedProperties = Remove(properties);
		        }

	        }

	        return updatedProperties;
        }


        private List<String> Add(List<String> oldList)
        {
	        List<String> updatedList = oldList;
	        while (true)
	        {
		        Hint(hintInputNew);
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

        private List<String> Remove(List<String> oldList)
        {
	        List<String> updatedList = oldList;
	        Hint(hintSelectToRemove);
	        var itemsToRemove = EasyInput<String>.SelectMultiple(oldList, _cancel);
	        foreach (var item in itemsToRemove)
	        {
		        updatedList.Remove(item);
	        }

	        return updatedList;

        }

	}
}
