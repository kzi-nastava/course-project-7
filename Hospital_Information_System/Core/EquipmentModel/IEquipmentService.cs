using System.Collections.Generic;
using System;

namespace HIS.Core.EquipmentModel
{
	public interface IEquipmentService
	{
		IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn);
		IEnumerable<Equipment> FilterByType(EquipmentType equipmentType);
		IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse);
		IEnumerable<Equipment> FilterByAmount(Func<int, bool> amountPredicate);
		IEnumerable<Equipment> GetAll();
		int GetTotalSupply(Equipment eq);
	}
}
