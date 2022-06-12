﻿using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.AppointmentModel
{
    public class AppointmentJSONRepository : IAppointmentRepository
    {
        private readonly IList<Appointment> _appointments;
        private readonly string _fname;
        private readonly JsonSerializerSettings _settings;

        public AppointmentJSONRepository(string fname, JsonSerializerSettings settings)
        {
            _fname = fname;
            _settings = settings;
            AppointmentJSONReferenceConverter.Repo = this;
            _appointments = JsonConvert.DeserializeObject<List<Appointment>>(File.ReadAllText(fname), _settings);
        }

		public int GetNextId()
		{
			return _appointments.Count;
		}

		public IEnumerable<Appointment> GetAll()
		{
			return _appointments.Where(o => !o.Deleted);
		}

		public Appointment Get(int id)
		{
			return _appointments.FirstOrDefault(r => r.Id == id);
		}

		public Appointment Add(Appointment obj)
		{
			obj.Id = GetNextId();
			_appointments.Add(obj);
			return obj;
		}

		public void Remove(Appointment obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_appointments, Formatting.Indented, _settings));
		}
    }
}