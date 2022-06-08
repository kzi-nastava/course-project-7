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
			return _repo.Get();
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
	}
}
