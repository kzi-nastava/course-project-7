using System;
using System.Collections.Generic;
using System.Linq;

namespace HospitalIS.Backend.Controller
{
	internal class EquipmentController
	{
		private static bool StringMatch(string s1, string s2)
		{
			s1 = s1.Trim().ToLower();
			s2 = s2.Trim().ToLower();
			if (s1.Length > s2.Length)
				return s1.Contains(s2);
			else
				return s2.Contains(s1);
		}

		public static List<Equipment> GetEquipment()
		{
			return IS.Instance.Hospital.Equipment.Where(e => !e.Deleted).ToList();
		}

		private static List<Equipment> MatchByString(string searchQuery, Func<Equipment, string> equipmentStrConv)
		{
			return GetEquipment().Where(eq => StringMatch(equipmentStrConv(eq), searchQuery)).ToList();
		}

		public static List<Equipment> MatchByType(string searchQuery)
		{
			return MatchByString(searchQuery, eq => eq.Type.ToString());
		}

		public static List<Equipment> MatchByUse(string searchQuery)
		{
			return MatchByString(searchQuery, eq => eq.Use.ToString());
		}

		public static List<Equipment.EquipmentType> GetEquipmentTypes()
		{
			return Enum.GetValues(typeof(Equipment.EquipmentType)).Cast<Equipment.EquipmentType>().ToList();
		}

		public static List<Equipment.EquipmentUse> GetEquipmentUses()
		{
			return Enum.GetValues(typeof(Equipment.EquipmentUse)).Cast<Equipment.EquipmentUse>().ToList();
		}

		public static List<Equipment> FilterByAmount(Func<int, bool> amountPredicate)
		{
			return GetEquipment().Where(eq => amountPredicate(GetTotalSupplyCount(eq))).ToList();
		}

		public static List<Equipment> FilterByUse(Equipment.EquipmentUse use)
		{
			return GetEquipment().Where(eq => eq.Use == use).ToList();
		}

		public static List<Equipment> FilterByType(Equipment.EquipmentType type)
		{
			return GetEquipment().Where(eq => eq.Type == type).ToList();
		}

		public static int GetTotalSupplyCount(Equipment equipment)
		{
			int numberOfEquipmentSupplies = 0;
			RoomController.GetRooms().ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfEquipmentSupplies += r.Equipment[equipment]; });
			return numberOfEquipmentSupplies;
		}

		public static int GetContainingRoomCount(Equipment equipment)
		{
			int numberOfContainingRooms = 0;
			RoomController.GetRooms().ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfContainingRooms++; });
			return numberOfContainingRooms;
		}
	}
}
