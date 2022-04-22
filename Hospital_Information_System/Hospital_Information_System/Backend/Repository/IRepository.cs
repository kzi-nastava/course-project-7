using Newtonsoft.Json;
using System;

namespace HospitalIS.Backend.Repository
{
	internal interface IRepository<T> where T : Entity
	{
		public abstract void Load(string fullFilename, JsonSerializerSettings settings);
		public abstract void Save(string fullFilename, JsonSerializerSettings settings);
		public abstract void Add(T entity);
		public abstract void Remove(T entity);
		public abstract void Remove(Func<T, bool> condition);
		public abstract T GetById(int id);
	}
}
