using System.Collections.Generic;
using System;
using HIS.Core.RoomModel;

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
		IEnumerable<Equipment> GetDynamicEquipmentNotInStock();
		bool IsDynamic(Equipment equipment);
		Dictionary<Equipment, int> GetDynamicEquipment(Room room);
		Dictionary<Equipment, int> GetNonDynamicEquipment(Room room);
		Dictionary<Equipment, int> GetEquipmentAfterDeletion(Dictionary<Equipment, int> dynamicEquipment, Dictionary<Equipment, int> nonDynamicEquipment);
	}
}
