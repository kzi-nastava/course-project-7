using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace HospitalIS.Backend.Repository
{
	internal class EquipmentRelocationJSON
	{
		public int Id = -1;
		public bool Deleted = false;
		public int Equipment = -1;
		public int RoomNew = -1;
		public DateTime ScheduledFor = DateTime.MinValue;
		internal EquipmentRelocationJSON()
		{
		}
		internal EquipmentRelocationJSON(EquipmentRelocation equipmentRelocation)
		{
			Id = equipmentRelocation.Id;
			Deleted = equipmentRelocation.Deleted;
			Equipment = equipmentRelocation.Equipment.Id;
			RoomNew = equipmentRelocation.RoomNew.Id;
			ScheduledFor = equipmentRelocation.ScheduledFor;
		}
		internal EquipmentRelocation GetRelocation(Hospital hospital)
		{
			var result = new EquipmentRelocation(
				hospital.Equipment[Equipment],
				hospital.Rooms[RoomNew],
				ScheduledFor
			);
			result.Id = Id;
			result.Deleted = Deleted;
			return result;
		}
	}
	internal abstract class EquipmentRelocationRepository
	{
		internal static void Save(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			List<EquipmentRelocationJSON> relocations = (
				from eqReloc in hospital.EquipmentRelocationsAll()
				select new EquipmentRelocationJSON(eqReloc)
			).ToList();

			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(relocations, Formatting.Indented, settings));
		}

		internal static List<EquipmentRelocation> Load(Hospital hospital, string fullFilename, JsonSerializerSettings settings)
		{
			var equipmentRelocationJSON = JsonConvert.DeserializeObject<List<EquipmentRelocationJSON>>(File.ReadAllText(fullFilename), settings);
			return (
				from eq in equipmentRelocationJSON
				select eq.GetRelocation(hospital)
			).ToList();
		}

		internal static void PerformRelocation(Hospital hospital, EquipmentRelocation relocation)
		{
			int dt = (int)(relocation.ScheduledFor - DateTime.Now).TotalMilliseconds;
			Thread.Sleep(Math.Max(dt, 0));

			if (relocation.Deleted)
				return;

			if (!hospital.EquipmentRelocations.Contains(relocation))
				throw new EntityNotFoundException();

			Console.WriteLine("[REMOVE ME] Performed relocation");

			relocation.RoomNew.Equipment.Add(relocation.Equipment);
			RoomHasEquipmentRepository.GetRoom(hospital, relocation.Equipment).Equipment.Remove(relocation.Equipment);
			hospital.Remove(relocation);
		}
	}
}
