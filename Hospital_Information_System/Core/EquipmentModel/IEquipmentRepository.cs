using System;
using System.Collections.Generic;

namespace HIS.Core.EquipmentModel
{
	public interface IEquipmentRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Equipment> GetAll();
		public Equipment Get(int id);
		public Equipment Add(Equipment obj);
		public void Remove(Equipment obj);
		IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn);
		IEnumerable<Equipment> FilterByType(EquipmentType equipmentType);
		IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse);
		bool IsDynamic(Equipment equipment);
		IEnumerable<Equipment> GetDynamic();
	}
}