using HIS.Core.AppointmentModel;
using HIS.Core.AppointmentModel.AppointmentComparers;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.PatientModel.MedicalRecordModel;
using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
    internal class MedicalRecordView : View
    {
        private readonly IMedicalRecordService _service;
        private readonly IPatientService _patientService;

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

        public MedicalRecordView(IMedicalRecordService service, IPatientService patientService, UserAccount user) : base(user)
        {
            _service = service;
            _patientService = patientService;
        }

        internal void Search()
        {
            if (_user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            Console.WriteLine(hintSearchQuery);
            string query = Console.ReadLine();

            var sortBy = new Dictionary<string, AppointmentComparer>()
            {
                ["Sort by date"] = new AppointmentCompareByDate(),
                ["Sort by doctor"] = new AppointmentCompareByDoctor(),
                ["Sort by doctor specialty"] = new AppointmentCompareByDoctorSpecialty(),
            };
            Console.WriteLine(hintSearchSortBy);
            var sortChoice = EasyInput<string>.Select(sortBy.Keys.ToList(), _cancel);

            Patient patient = _patientService.GetPatientFromPerson(_user.Person);
            List<Appointment> matches = _service.MatchAppointmentByAnamnesis(query, sortBy[sortChoice], patient).ToList();

            foreach (Appointment match in matches)
            {
                Console.WriteLine(match.AnamnesisFocusedToString());
            }
        }
    }
}
