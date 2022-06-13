using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using HIS.Core.PersonModel.UserAccountModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
    internal class DoctorView : View
    {
        private readonly IDoctorService _service;
        private readonly AppointmentView _appointmentView;

        private const string hintMatchBy = "Select criteria to match by";
        private const string hintSearchQuery = "Enter search query";
        private const string hintSortBy = "Select sorting criteria";

        private const string hintNoDoctorMatches = "No doctors matching search criteria.";

        private const string hintSelectDoctor = "Select doctor";

        public DoctorView(IDoctorService service, AppointmentView appointmentView, UserAccount user) : base(user)
        {
            _service = service;
            _appointmentView = appointmentView;
        }

        internal void Search()
        {
            IEnumerable<Doctor> doctors = SearchImpl();
            if (doctors == null)
            {
                Console.WriteLine(hintNoDoctorMatches);
                return;
            }
            foreach (Doctor d in doctors)
            {
                Console.WriteLine(d.VerboseToString());
            }
        }

        internal void AppointFromSearch()
        {
            if (_user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            try
            {
                IEnumerable<Doctor> doctors = SearchImpl();
                Console.WriteLine(hintSelectDoctor);
                Doctor doctor = EasyInput<Doctor>.Select(doctors, d => d.VerboseToString(), _cancel);

                var refAppointment = new Appointment()
                {
                    Doctor = doctor
                };
                var properties = new List<AppointmentProperty> { AppointmentProperty.DOCTOR };
                _appointmentView.CreateWithPredefinedProperties(properties, refAppointment);
            }
            catch (NothingToSelectException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private IEnumerable<Doctor> SearchImpl()
        {
            var matchBy = new Dictionary<string, Func<string, DoctorComparer, IEnumerable<Doctor>>>
            {
                ["Match by first name"] = _service.MatchByFirstName,
                ["Match by last name"] = _service.MatchByLastName,
                ["Match by specialty"] = _service.MatchBySpecialty,
            };
            Console.WriteLine(hintMatchBy);
            var matchChoice = EasyInput<string>.Select(matchBy.Keys.ToList(), _cancel);

            Console.WriteLine(hintSearchQuery);
            string query = Console.ReadLine();

            var sortBy = new Dictionary<string, DoctorComparer>()
            {
                ["Sort by first name"] = new DoctorCompareByFirstName(),
                ["Sort by last name"] = new DoctorCompareByLastName(),
                ["Sort by specialty"] = new DoctorCompareBySpecialty(),
                ["Sort by rating"] = new DoctorCompareByRatingDesc(),
            };
            Console.WriteLine(hintSortBy);
            var sortChoice = EasyInput<string>.Select(sortBy.Keys.ToList(), _cancel);

            return matchBy[matchChoice](query, sortBy[sortChoice]);
        }
    }
}
