﻿namespace HospitalIS.Backend.Room
{
	public class Equipment : Entity
	{
		public enum EquipmentType
		{
			Unknown, Chair, Table, Bed, Cabinet
		}

		public enum EquipmentUse
		{
			Unknown, Examination, Operation, Furniture, Hallway
		}

		public EquipmentType Type { get; set; }
		public EquipmentUse Use { get; set; }

		public Equipment()
		{
			Type = EquipmentType.Unknown;
			Use = EquipmentUse.Unknown;
		}

		public Equipment(EquipmentType type, EquipmentUse use)
		{
			Type = type;
			Use = use;
		}

		public override string ToString()
		{
			return $"Equipment{{Id = {Id}, Type = {(int)Type} ({Type}), Use = {(int)Use} ({Use})}}";
		}
	}
}