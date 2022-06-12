using System;
using System.Collections.Generic;

namespace HIS.Core.PersonModel
{
	public interface IPersonRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Person> GetAll();
		public Person Get(int id);
		public Person Add(Person obj);
		public void Remove(Person obj);
	}
}
