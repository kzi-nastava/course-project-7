using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace HospitalIS.Backend.Repository
{
	class EquipmentRepository : IRepository<Equipment>
	{
		public void Add(Equipment entity)
		{
			List<Equipment> Equipment = IS.Instance.Hospital.Equipment;

			entity.Id = Equipment.Count > 0 ? Equipment.Last().Id + 1 : 0;
			Equipment.Add(entity);
		}

		public Equipment GetById(int id)
		{
			return IS.Instance.Hospital.Equipment.First(e => e.Id == id);
		}

		public void Load(string fullFilename, JsonSerializerSettings settings)
		{
			IS.Instance.Hospital.Equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(fullFilename), settings);
		}

		public void Remove(Equipment entity)
		{
			entity.Deleted = true;

			IS.Instance.EquipmentRelocationRepo.Remove(relocation => relocation.Equipment == entity);
		}

		public void Remove(Func<Equipment, bool> condition)
		{
			IS.Instance.Hospital.Equipment.ForEach(entity => { if (condition(entity)) Remove(entity); });
		}

		public void Save(string fullFilename, JsonSerializerSettings settings)
		{
			File.WriteAllText(fullFilename, JsonConvert.SerializeObject(IS.Instance.Hospital.Equipment, Formatting.Indented, settings));
		}
	}
}
