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
			return IS.Instance.Hospital.Equipment.Where(eq => StringMatch(equipmentStrConv(eq), searchQuery)).ToList();
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
			return IS.Instance.Hospital.Equipment.Where(eq => amountPredicate(GetTotalCount(eq))).ToList();
		}

		public static List<Equipment> FilterByUse(Equipment.EquipmentUse use)
		{
			return IS.Instance.Hospital.Equipment.Where(eq => eq.Use == use).ToList();
		}

		public static List<Equipment> FilterByType(Equipment.EquipmentType type)
		{
			return IS.Instance.Hospital.Equipment.Where(eq => eq.Type == type).ToList();
		}

		public static int GetTotalCount(Equipment equipment)
		{
			int numberOfContainingRooms = 0;
			IS.Instance.Hospital.Rooms.ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfContainingRooms += r.Equipment[equipment]; });
			return numberOfContainingRooms;
		}

		public static int GetRoomCount(Equipment equipment)
		{
			int numberOfContainingRooms = 0;
			IS.Instance.Hospital.Rooms.ForEach(r => { if (r.Equipment.ContainsKey(equipment)) numberOfContainingRooms++; });
			return numberOfContainingRooms;
		}
	}
}
