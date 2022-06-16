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
		
		public IEnumerable<Equipment> GetDynamicEquipment()
		{
			return _repo.GetAll().Where(IsDynamicEquipment).ToList();
		}
		
		public IEnumerable<Equipment> GetDynamicEquipmentNotInStock()
		{
			return GetDynamicEquipment().Where(eq => GetTotalSupply(eq) == 0).ToList();
		}
		
		public bool IsDynamicEquipment(Equipment equipment)
		{
			return (equipment.Use == EquipmentUse.Examination || 
			        equipment.Use == EquipmentUse.Operation ||
			        equipment.Use == EquipmentUse.Unknown) && 
			       (equipment.Type == EquipmentType.Gauze ||
			        equipment.Type == EquipmentType.Injection ||
			        equipment.Type == EquipmentType.BandAid ||
			        equipment.Type == EquipmentType.Pen ||
			        equipment.Type == EquipmentType.Paper ||
			        equipment.Type == EquipmentType.Unknown);
		}
	}
}
