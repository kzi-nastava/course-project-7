using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace HospitalIS.Backend.Repository
{
	internal static class EquipmentRelocationRepository
	{
		public static void Load(string fullFilename, JsonSerializerSettings settings)
		{
			Hospital.Instance.EquipmentRelocations = JsonConvert.DeserializeObject<List<EquipmentRelocation>>(File.ReadAllText(fullFilename), settings);
		}

		public static void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(Hospital.Instance.EquipmentRelocations, Formatting.Indented, settings));
		}

		public static void Execute(EquipmentRelocation relocation)
		{
			Thread.Sleep(Math.Max(relocation.GetTimeToLive(), 0));

			if (relocation.Deleted)
				return;

			if (!Hospital.Instance.EquipmentRelocations.Contains(relocation))
				throw new EntityNotFoundException();

			Console.WriteLine("[REMOVE ME] Performed relocation");

			RoomRepository.AddEquipment(relocation.RoomNew, relocation.Equipment);
			RoomRepository.RemoveEquipment(relocation.RoomOld, relocation.Equipment);
			Hospital.Instance.Remove(relocation);
		}
	}
}
