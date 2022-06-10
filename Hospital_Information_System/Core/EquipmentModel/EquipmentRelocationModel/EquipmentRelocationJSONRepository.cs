using HIS.Core.RoomModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.EquipmentModel.EquipmentRelocationModel
{
	public class EquipmentRelocationJSONRepository : IEquipmentRelocationRepository
	{
		private readonly IList<EquipmentRelocation> _equipmentRelocations;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public EquipmentRelocationJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_equipmentRelocations = JsonConvert.DeserializeObject<List<EquipmentRelocation>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _equipmentRelocations.Count;
		}

		public IEnumerable<EquipmentRelocation> GetAll()
		{
			return _equipmentRelocations.Where(o => !o.Deleted);
		}

		public EquipmentRelocation Get(int id)
		{
			return _equipmentRelocations.FirstOrDefault(r => r.Id == id);
		}

		public void Add(EquipmentRelocation obj)
		{
			_equipmentRelocations.Add(obj);
		}

		public void Remove(EquipmentRelocation obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_equipmentRelocations, Formatting.Indented, _settings));
		}

		public IEnumerable<EquipmentRelocation> Get(Room ofRoom)
		{
			return GetAll().Where(reloc => reloc.RoomFrom == ofRoom || reloc.RoomTo == ofRoom);
		}
	}
}
