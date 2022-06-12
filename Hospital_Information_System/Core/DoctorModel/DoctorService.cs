using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.DoctorModel
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
	}
}
