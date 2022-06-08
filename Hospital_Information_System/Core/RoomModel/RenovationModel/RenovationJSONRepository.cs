using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HIS.Core.RoomModel.RenovationModel
{
	public class RenovationJSONRepository : IRenovationRepository
	{
		private readonly IList<Renovation> _renovations;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public RenovationJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_renovations = JsonConvert.DeserializeObject<List<Renovation>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _renovations.Count;
		}

		public IEnumerable<Renovation> Get()
		{
			return _renovations.Where(o => !o.Deleted);
		}

		public IEnumerable<Renovation> Get(Room r)
		{
			return Get().Where(ren => ren.Room == r);
		}

		public Renovation Get(int id)
		{
			return _renovations.FirstOrDefault(r => r.Id == id);
		}

		public void Add(Renovation obj)
		{
			_renovations.Add(obj);
		}

		public void Remove(Renovation obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_renovations, Formatting.Indented, _settings));
		}
	}
}
