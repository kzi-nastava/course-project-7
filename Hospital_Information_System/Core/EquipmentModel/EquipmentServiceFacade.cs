using System;
using System.Collections.Generic;
using HIS.Core.RoomModel;
using System.Linq;

namespace HIS.Core.EquipmentModel
{
	// TODO : Overkill? 
	//
	// Dependency chain:
	//
	// Equipment view ==(1)==> Equipment service =====> Equipment repository
	//				  ==(2)==>    Room service   =====> Room repository
	//
	// (1) - This is obvious.
	// (2) - We need the GetTotalSupply() method in the view, but we need the room
	// repository for that, not equipment repository, so we also need room service in
	// equipment view.
	//
	// Sol. 1: Equipment view uses both services.
	// Sol. 2: Equipment service uses both repositories.
	// Sol. 3: Facade over room & equipment service, Equipment view uses the facade.
	//		   The view isn't using a service interface but a concrete impementation.
	// Sol. 4: Merge views / services / repositories? The code can get too big.

	public class EquipmentServiceFacade : IEquipmentService
	{
		private readonly IEquipmentService _equipmentService;
		private readonly IRoomService _roomService;

		public EquipmentServiceFacade(IEquipmentService equipmentService, IRoomService roomService)
		{
			_equipmentService = equipmentService;
			_roomService = roomService;
		}

		public int GetTotalSupply(Equipment eq)
		{
			int v = 0;
			return _roomService.Get().Sum(r => r.Equipment.TryGetValue(eq, out v) ? v : 0);
		}

		public IEnumerable<Equipment> FilterByAmount(Func<int, bool> amountPredicate)
		{
			return _equipmentService.Get().Where(eq => amountPredicate(GetTotalSupply(eq)));
		}

		public IEnumerable<Equipment> FilterByType(EquipmentType equipmentType)
		{
			return _equipmentService.FilterByType(equipmentType);
		}

		public IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse)
		{
			return _equipmentService.FilterByUse(equipmentUse);
		}

		public IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn)
		{
			return _equipmentService.Search(searchQuery, propertiesToSearchOn);
		}

		public IEnumerable<Equipment> Get()
		{
			return _equipmentService.Get();
		}
	}
}
