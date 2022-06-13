using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PersonModel.DoctorModel
{
	public class DoctorService : IDoctorService
	{
		private readonly IDoctorRepository _repo;

		public DoctorService(IDoctorRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Doctor> GetAll()
		{
			return _repo.GetAll();
		}

        public Doctor GetDoctorFromPerson(Person person)
		{
            return _repo.GetAll().First(d => d.Person == person);
		}
    }
}
