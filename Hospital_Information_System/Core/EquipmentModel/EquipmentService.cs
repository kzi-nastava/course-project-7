using HIS.Core.RoomModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.EquipmentModel
{
	public class EquipmentService : IEquipmentService
	{
		private readonly IEquipmentRepository _repo;
		private readonly IRoomService _roomService;

		public EquipmentService(IEquipmentRepository repo, IRoomService roomService)
		{
			_repo = repo;
			_roomService = roomService;
		}

		public IEnumerable<Equipment> FilterByAmount(Func<int, bool> amountPredicate)
		{
			return GetAll().Where(eq => amountPredicate(GetTotalSupply(eq)));
		}

		public IEnumerable<Equipment> FilterByType(EquipmentType equipmentType)
		{
			return _repo.FilterByType(equipmentType);
		}

		public IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse)
		{
			return _repo.FilterByUse(equipmentUse);
		}

		public IEnumerable<Equipment> GetAll()
		{
			return _repo.GetAll();
		}

		public IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn)
		{
			return _repo.Search(searchQuery, propertiesToSearchOn);
		}

		public int GetTotalSupply(Equipment eq)
		{
			int v = 0;
			return _roomService.GetAll().Sum(r => r.Equipment.TryGetValue(eq, out v) ? v : 0);
		}
		
		private IEnumerable<Equipment> GetDynamic()
		{
			return _repo.GetDynamic();
		}
		
		public IEnumerable<Equipment> GetDynamicEquipmentNotInStock()
		{
			return GetDynamic().Where(eq => GetTotalSupply(eq) == 0).ToList();
		}
		
		public bool IsDynamic(Equipment equipment)
		{
			return _repo.IsDynamic(equipment);
		}

		public Dictionary<Equipment, int> GetDynamicEquipment(Room room)
		{
			Dictionary<Equipment, int> allEquipment = room.Equipment;
			Dictionary<Equipment, int> dynamicEquipment = new Dictionary<Equipment, int>();
			foreach (KeyValuePair<Equipment, int> entry in allEquipment)
			{
				if (IsDynamic(entry.Key))
				{
					dynamicEquipment[entry.Key] = entry.Value;
				}
			}

			return dynamicEquipment;
		}
		
		public Dictionary<Equipment, int> GetNonDynamicEquipment(Room room)
		{
			Dictionary<Equipment, int> allEquipment = room.Equipment;
			Dictionary<Equipment, int> dynamicEquipment = new Dictionary<Equipment, int>();
			foreach (KeyValuePair<Equipment, int> entry in allEquipment)
			{
				if (!IsDynamic(entry.Key))
				{
					dynamicEquipment[entry.Key] = entry.Value;
				}
			}

			return dynamicEquipment;
		}

		public Dictionary<Equipment, int> GetEquipmentAfterDeletion(Dictionary<Equipment, int> dynamicEquipment, Dictionary<Equipment, int> nonDynamicEquipment)
		{
			Dictionary<Equipment, int> equipment = new Dictionary<Equipment, int>();
			foreach (KeyValuePair<Equipment, int> entry in dynamicEquipment)
			{
				equipment.Add(entry.Key, entry.Value);
			}
			foreach (KeyValuePair<Equipment, int> entry in nonDynamicEquipment)
			{
				equipment.Add(entry.Key, entry.Value);
			}

			return equipment;
		}
	}
}
