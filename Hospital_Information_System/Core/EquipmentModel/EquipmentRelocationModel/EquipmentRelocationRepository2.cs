using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HospitalIS.Backend.Repository
{
	/*
	internal class EquipmentRelocationRepository2 : IRepository<EquipmentRelocation2>
	{
		public void Add(EquipmentRelocation2 entity)
		{
			List<EquipmentRelocation2> Relocations = IS.Instance.Hospital.EquipmentRelocations;

			entity.Id = Relocations.Count > 0 ? Relocations.Last().Id + 1 : 0;
			Relocations.Add(entity);
		}

		public EquipmentRelocation2 GetById(int id)
		{
			return IS.Instance.Hospital.EquipmentRelocations.First(e => e.Id == id);
		}

		public void Load(string fullFilename, JsonSerializerSettings settings)
		{
			IS.Instance.Hospital.EquipmentRelocations = JsonConvert.DeserializeObject<List<EquipmentRelocation2>>(File.ReadAllText(fullFilename), settings);
		}

		public void Remove(EquipmentRelocation2 entity)
		{
			entity.Deleted = true;
		}

		public void Remove(Func<EquipmentRelocation2, bool> condition)
		{
			IS.Instance.Hospital.EquipmentRelocations.ForEach(entity => { if (condition(entity)) Remove(entity); });
		}

		public void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.EquipmentRelocations, Formatting.Indented, settings));
		}

		public void Execute(EquipmentRelocation2 relocation)
		{
			Thread.Sleep(Math.Max(relocation.GetTimeToLive(), 0));

			if (relocation.Deleted)
				return;

			if (!IS.Instance.Hospital.EquipmentRelocations.Contains(relocation))
				throw new EntityNotFoundException();

			Console.WriteLine($"Performed relocation {relocation}.");

			IS.Instance.RoomRepo.Add(relocation.RoomNew, relocation.Equipment);
			IS.Instance.RoomRepo.Remove(relocation.RoomOld, relocation.Equipment);
			IS.Instance.EquipmentRelocationRepo.Remove(relocation);
		}

		public void AddTask(EquipmentRelocation2 equipmentRelocation)
		{
			Task t = new Task(() => Execute(equipmentRelocation));
			IS.Instance.Hospital.EquipmentRelocationTasks.Add(t);
			t.Start();
		}
	}
	*/
}
