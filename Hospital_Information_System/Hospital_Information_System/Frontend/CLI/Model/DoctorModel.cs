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

        internal static void Search(UserAccount user, string inputCancelString)
        {
            if (user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

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

            List<Doctor> matches = matchBy[matchChoice](query, sortBy[sortChoice]);

            foreach (Doctor d in matches)
            {
                Console.WriteLine(d.VerboseToString());
            }
        }
    }
}
