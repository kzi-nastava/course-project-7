using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.PersonModel
{
	public class PersonService : IPersonService
	{
		private readonly IPersonRepository _repo;

		public PersonService(IPersonRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Person> GetAll()
		{
			return _repo.GetAll();
		}
	}
}
