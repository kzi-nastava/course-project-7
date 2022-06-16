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
	internal class EquipmentRelocationView : AbstractView
	{
		private readonly IRoomService _roomService;
		private readonly IEquipmentRelocationService _service;
		private readonly IList<EquipmentRelocationProperty> _properties;
		private readonly IEquipmentService _equipmentService;

		private const string errNoEquipmentAvailable = "There is no equipment available"; 
		private const string hintInputNewRoom = "\nSelect room to relocate the equipment in";
		internal EquipmentRelocationView(IEquipmentRelocationService service, IRoomService roomService, IEquipmentService equipmentService)
		{
			_roomService = roomService;
			_service = service;
			_properties = Utility.GetEnumValues<EquipmentRelocationProperty>().ToList();
			_equipmentService = equipmentService;
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
		
		internal void CmdMoveDynamicEquipment()
		{
			try
			{
				var needsEquipment =  GetRoomsThatNeedEquipment();
				Room roomTo = SelectRoom(needsEquipment);
				DynamicRelocation(roomTo);
			}
			catch (NothingToSelectException)
			{
				Print(errNoEquipmentAvailable);
			}
			
		}

		private void DynamicRelocation(Room roomTo)
		{
			var roomFrom = SelectRoom();
			var equipment = SelectDynamicEquipment(roomFrom);
			var when = DateTime.Now;

			var relocation = new EquipmentRelocation(equipment, roomFrom, roomTo, when);
			_service.Add(relocation);
		}
		
		private IEnumerable<Room> GetRoomsThatNeedEquipment()
		{
			var needsEquipment = new List<Room>();
			var rooms = _roomService.GetExeminationAndOperationRooms();	
			foreach (Room room in rooms)
			{
				IEnumerable<Equipment> equipment = _roomService.GetEquipment(room);

				if (equipment.Count() > 0)
				{
					PrintRoomsEquipment(room, equipment);
					needsEquipment.Add(room);
				}
			}

			return needsEquipment;
		}

		private void PrintRoomsEquipment(Room room, IEnumerable<Equipment> roomEquipment)
		{
			Console.WriteLine("\n" + room);
			
			foreach (Equipment equipment in roomEquipment)
			{
				if (_roomService.DoesNotHaveEquipmentRightNow(room, equipment))
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(equipment + " Amount: " + room.Equipment[equipment]);
					Console.ResetColor();
					continue;
				}
				if (_roomService.HasEquipmentLessThanFive(room, equipment))
					Console.WriteLine(equipment + " Amount: " + room.Equipment[equipment]);
				
			}
		}

		private Room SelectRoom(IEnumerable<Room> needsEquipment)
		{
			Hint(hintInputNewRoom);
			return EasyInput<Room>.Select(needsEquipment, _cancel);
		}

		private Room SelectRoom()
		{
			return EasyInput<Room>.Select(_roomService.GetAll(), _cancel);
		}

		private Equipment SelectEquipment(Room r)
		{
			return EasyInput<Equipment>.Select(r.Equipment.Keys, _cancel);
		}
		
		private Equipment SelectDynamicEquipment(Room r)
		{
			return EasyInput<Equipment>.Select(r.Equipment.Keys.Where(eq => _equipmentService.IsDynamic(eq)), _cancel);
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
