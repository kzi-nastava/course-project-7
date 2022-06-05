using HIS.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace HIS.Core.EquipmentModel
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
			EquipmentJSONDictionaryConverter<int>.Repo = this;
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

		public IEnumerable<Equipment> Search(string searchQuery, IList<EquipmentProperty> propertiesToSearchOn)
		{
			IEnumerable<Equipment> matchType = default, matchUse = default;

			if (propertiesToSearchOn.Contains(EquipmentProperty.TYPE))
			{
				matchType = _equipment.Where(eq => eq.Type.ToString() == searchQuery);
			}
			if (propertiesToSearchOn.Contains(EquipmentProperty.USE))
			{
				matchUse = _equipment.Where(eq => eq.Use.ToString() == searchQuery);
			}

			return matchType.UnionNullSafe(matchUse);
		}

		public IEnumerable<Equipment> FilterByType(EquipmentType equipmentType)
		{
			return _equipment.Where(eq => eq.Type == equipmentType);
		}

		public IEnumerable<Equipment> FilterByUse(EquipmentUse equipmentUse)
		{
			return _equipment.Where(eq => eq.Use == equipmentUse);
		}
	}
}