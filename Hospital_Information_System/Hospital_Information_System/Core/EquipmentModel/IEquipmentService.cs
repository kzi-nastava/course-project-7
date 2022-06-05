using System.Collections.Generic;
using System;

namespace HospitalIS.Core.EquipmentModel
{
	internal interface IEquipmentService
	{
		public IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn);
		IEnumerable<Equipment> FilterByType(EquipmentType equipmentType);
		IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse);
		IEnumerable<Equipment> FilterByAmount(Func<int, bool> amountPredicate);
		IEnumerable<Equipment> Get();
	}
}
