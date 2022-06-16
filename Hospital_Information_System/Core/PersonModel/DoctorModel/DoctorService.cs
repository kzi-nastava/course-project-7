using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using HIS.Core.PollModel.AppointmentPollModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PersonModel.DoctorModel
{
	public class DoctorService : IDoctorService
	{
		private readonly IDoctorRepository _repo;
        private readonly IAppointmentPollService _appointmentPollService;

		public DoctorService(IDoctorRepository repo, IAppointmentPollService appointmentPollService)
		{
			_repo = repo;
            _appointmentPollService = appointmentPollService;
		}

		public IEnumerable<Doctor> GetAll()
		{
			return _repo.GetAll();
		}

        public Doctor GetDoctorFromPerson(Person person)
		{
            return _repo.GetAll().First(d => d.Person == person);
		}

        public IEnumerable<Doctor> MatchByFirstName(string query, DoctorComparer comparer)
        {
            return _repo.MatchByFirstName(query, comparer);
        }

        public IEnumerable<Doctor> MatchByLastName(string query, DoctorComparer comparer)
        {
            return _repo.MatchByLastName(query, comparer);
        }

        public IEnumerable<Doctor> MatchBySpecialty(string query, DoctorComparer comparer)
        {
            return _repo.MatchBySpecialty(query, comparer);
        }

        public IEnumerable<Doctor> MatchByString(string query, DoctorComparer comparer, Func<Doctor, string> toStr)
        {
            return _repo.MatchByString(query, comparer, toStr);
        }

        public double CalculateRating(Doctor doctor)
        {
            return _appointmentPollService.GetAll(doctor).Average(r => r.GetRating(AppointmentPollHelpers.QServiceQuality));
        }

        public string VerboseToString(Doctor doctor)
        {
            return $"Doctor{{Id = {doctor.Id}, First name = {doctor.Person.FirstName}, Last name = {doctor.Person.LastName}, Specialty = {doctor.Specialty}, Rating = {Math.Round(CalculateRating(doctor), 2)}}}";
        }
        public bool ExistForSpecialty(Doctor.MedicineSpeciality speciality)
        {
            return _repo.GetAll().Any(d => d.Specialty == speciality);
        }

        public IEnumerable<Doctor.MedicineSpeciality> GetAllSpecialties()
        {
            return Enum.GetValues(typeof(Doctor.MedicineSpeciality)).Cast<Doctor.MedicineSpeciality>().ToList();
        }
    }
}
