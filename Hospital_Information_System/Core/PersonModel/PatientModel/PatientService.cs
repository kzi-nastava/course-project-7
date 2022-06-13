using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PersonModel.PatientModel
{
    public class PatientService : IPatientService
    {
		private readonly IPatientRepository _repo;

		public PatientService(IPatientRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Patient> GetAll()
		{
			return _repo.GetAll();
		}

        public Patient GetPatientFromPerson(Person person)
		{
			return _repo.GetAll().First(p => p.Person == person);
		}
    }
}
