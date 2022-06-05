using System;
using System.Collections.Generic;

namespace HIS.Core.EquipmentModel
{
	public interface IEquipmentRepository
	{
		public void Save();
		public IEnumerable<Equipment> Get();
		public Equipment Get(int id);
		public void Add(Equipment obj);
		public void Remove(Equipment obj);
		IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn);
		IEnumerable<Equipment> FilterByType(EquipmentType equipmentType);
		IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse);
	}
}