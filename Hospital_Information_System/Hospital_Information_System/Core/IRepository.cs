using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HospitalIS.Backend
{
	public interface IRepository<T> where T : Entity
	{
		public void Save();
		public IEnumerable<T> Get();
		public T Get(int id);
		public void Add(T obj);
		public void Remove(T obj);
	}
}
