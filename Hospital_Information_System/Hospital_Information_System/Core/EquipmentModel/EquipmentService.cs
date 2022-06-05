using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Core.EquipmentModel
{
	internal class EquipmentService : IEquipmentService
	{
		private readonly IEquipmentRepository _repo;

		public EquipmentService(IEquipmentRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Equipment> FilterByAmount(Func<int, bool> amountPredicate)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Equipment> FilterByType(EquipmentType equipmentType)
		{
			return _repo.FilterByType(equipmentType);
		}

		public IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse)
		{
			return _repo.FilterByUse(equipmentUse);
		}

		public IEnumerable<Equipment> Get()
		{
			return _repo.Get();
		}

		public IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn)
		{
			return _repo.Search(searchQuery, propertiesToSearchOn);
		}
	}
}
