using System.Collections.Generic;

namespace HospitalIS.Backend.Room
{
	public interface IEquipmentRepository
	{
		public void Save();
		public IEnumerable<Equipment> Get();
		public Equipment Get(int id);
		public void Add(Equipment obj);
		public void Remove(Equipment obj);
	}
}