using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HospitalIS.Backend.EquipmentModel
{
	public class EquipmentJSONRepository : IEquipmentRepository
	{
		private readonly IList<Equipment> _equipment;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public EquipmentJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			EquipmentJSONDictionaryConverter<int>.repo = this;
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