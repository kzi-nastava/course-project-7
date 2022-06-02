using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.Room
{
	public class EquipmentJSONRepository : IEquipmentRepository
	{
		private List<Equipment> _equipment;
		private string _fname;
		private JsonSerializerSettings _settings;

		public EquipmentJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_equipment = JsonConvert.DeserializeObject<List<Equipment>>(File.ReadAllText(fname), _settings);
		}

		public IEnumerable<Equipment> Get()
		{
			return _equipment;
		}

		public Equipment Get(int id)
		{
			return _equipment.FirstOrDefault(r => r.Id == id);
		}

		public void Add(Equipment obj)
		{
			_equipment.Add(obj);
		}

		public void Remove(Equipment obj)
		{
			_equipment.Remove(obj);
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_equipment, Formatting.Indented, _settings));
		}
	}
}