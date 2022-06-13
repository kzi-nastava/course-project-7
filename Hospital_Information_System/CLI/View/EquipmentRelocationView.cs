using HIS.Core.EquipmentModel;
using HIS.Core.EquipmentModel.EquipmentRelocationModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.RoomModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class EquipmentRelocationView : View
	{
		private readonly IRoomService _roomService;
		private readonly IEquipmentRelocationService _service;
		private readonly IList<EquipmentRelocationProperty> _properties;

		internal EquipmentRelocationView(IEquipmentRelocationService service, IRoomService roomService, UserAccount user) : base(user)
		{
			_roomService = roomService;
			_service = service;
			_properties = Utility.GetEnumValues<EquipmentRelocationProperty>().ToList();
		}

		internal void CmdPerform()
		{
			var roomFrom = SelectRoom();
			var equipment = SelectEquipment(roomFrom);
			var roomTo = SelectRoom();
			var when = SelectWhen();

			var relocation = new EquipmentRelocation(equipment, roomFrom, roomTo, when);
			_service.Add(relocation);
		}

		private Room SelectRoom()
		{
			return EasyInput<Room>.Select(_roomService.GetAll(), _cancel);
		}

		private Equipment SelectEquipment(Room r)
		{
			return EasyInput<Equipment>.Select(r.Equipment.Keys, _cancel);
		}

		private DateTime SelectWhen()
		{
			DateTime result = default;
			EasyInput<string>.Get(
				new List<Func<string, bool>>
				{
					s => DateTime.TryParse(s, out result),
					s => DateTime.Parse(s) > DateTime.Now
				},
				new[]
				{
					"Invalid timestamp",
					"Must be in the future"
				},
				_cancel
			);
			return result;
		}
	}
}
