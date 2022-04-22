using HospitalIS.Backend.Repository;
using Newtonsoft.Json;
using System.IO;

namespace HospitalIS.Backend
{
	internal class IS
	{
		private static IS _system;
		public static IS Instance
		{
			get
			{
				return _system ??= new IS();
			}
		}

		public Hospital Hospital = new Hospital();
		public RoomRepository RoomRepo = new RoomRepository();
		public EquipmentRepository EquipmentRepo = new EquipmentRepository();
		public EquipmentRelocationRepository EquipmentRelocationRepo = new EquipmentRelocationRepository();

		private readonly JsonSerializerSettings settings;
		private const string fnameRooms = "rooms.json";
		private const string fnameEquipment = "equipment.json";
		private const string fnameEquipmentRelocation = "equipmentRelocation.json";

		public IS()
		{
			settings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects };
		}

		public void Save(string directory)
		{
			EquipmentRepo.Save(Path.Combine(directory, fnameEquipment), settings);
			RoomRepo.Save(Path.Combine(directory, fnameRooms), settings);
			EquipmentRelocationRepo.Save(Path.Combine(directory, fnameEquipmentRelocation), settings);
		}

		public void Load(string directory)
		{
			EquipmentRepo.Load(Path.Combine(directory, fnameEquipment), settings);
			RoomRepo.Load(Path.Combine(directory, fnameRooms), settings);
			EquipmentRelocationRepo.Load(Path.Combine(directory, fnameEquipmentRelocation), settings);

			foreach (var relocation in Hospital.EquipmentRelocations)
			{
				Hospital.AddEquipmentRelocationTask(relocation);
			}
		}
	}
}
