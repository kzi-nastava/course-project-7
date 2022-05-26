using HospitalIS.Backend;
using HospitalIS.Backend.Controller;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class DoctorModel
    {
        private const string hintMatchBy = "Select criteria to match by";
        private const string hintSearchQuery = "Enter search query";
        private const string hintSortBy = "Select sorting criteria";

        private const string hintNoDoctorMatches = "No doctors matching search criteria.";

        private const string hintSelectDoctor = "Select doctor";

        internal static void Search(string inputCancelString)
        {
            List<Doctor> doctors = SearchImpl(inputCancelString);
            if (doctors == null)
            {
                Console.WriteLine(hintNoDoctorMatches);
                return;
            }
            foreach (Doctor d in doctors)
            {
                d.VerboseToString();
            }
        }

        internal static void AppointFromSearch(UserAccount user, string inputCancelString)
        {
            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            try
            {
                List<Doctor> doctors = SearchImpl(inputCancelString);
                Console.WriteLine(hintSelectDoctor);
                Doctor doctor = EasyInput<Doctor>.Select(doctors, d => d.VerboseToString(), inputCancelString);

                var refAppointment = new Appointment()
                {
                    Doctor = doctor
                };
                var properties = new List<AppointmentController.AppointmentProperty> { AppointmentController.AppointmentProperty.DOCTOR };
                AppointmentModel.CreateWithPredefinedProperties(inputCancelString, user, properties, refAppointment);
            }
            catch (InputFailedException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<Doctor> SearchImpl(string inputCancelString)
        {
            var matchBy = new Dictionary<string, Func<string, Doctor.Comparer, List<Doctor>>>
            {
                ["Match by first name"] = DoctorController.MatchByFirstName,
                ["Match by last name"] = DoctorController.MatchByLastName,
                ["Match by specialty"] = DoctorController.MatchBySpecialty,
            };
            Console.WriteLine(hintMatchBy);
            var matchChoice = EasyInput<string>.Select(matchBy.Keys.ToList(), inputCancelString);

            Console.WriteLine(hintSearchQuery);
            string query = Console.ReadLine();

            var sortBy = new Dictionary<string, Doctor.Comparer>()
            {
                ["Sort by first name"] = new Doctor.CompareByFirstName(),
                ["Sort by last name"] = new Doctor.CompareByLastName(),
                ["Sort by specialty"] = new Doctor.CompareBySpecialty(),
            };
            Console.WriteLine(hintSortBy);
            var sortChoice = EasyInput<string>.Select(sortBy.Keys.ToList(), inputCancelString);

            return matchBy[matchChoice](query, sortBy[sortChoice]);
        }
    }
}
