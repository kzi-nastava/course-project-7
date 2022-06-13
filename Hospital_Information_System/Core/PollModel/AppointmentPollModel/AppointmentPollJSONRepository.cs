using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.Util;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Core.PollModel.AppointmentPollModel
{
	public class AppointmentPollJSONRepository : IAppointmentPollRepository
	{
		private readonly IList<AppointmentPoll> _appointmentPolls;
		private readonly string _fname;
		private readonly JsonSerializerSettings _settings;

		public AppointmentPollJSONRepository(string fname, JsonSerializerSettings settings)
		{
			_fname = fname;
			_settings = settings;
			_appointmentPolls = JsonConvert.DeserializeObject<List<AppointmentPoll>>(File.ReadAllText(fname), _settings);
		}

		public int GetNextId()
		{
			return _appointmentPolls.Count;
		}

		public IEnumerable<AppointmentPoll> GetAll()
		{
			return _appointmentPolls.Where(o => !o.Deleted);
		}

		public AppointmentPoll Get(int id)
		{
			return _appointmentPolls.FirstOrDefault(r => r.Id == id);
		}

		public AppointmentPoll Add(AppointmentPoll obj)
		{
			obj.Id = GetNextId();
			_appointmentPolls.Add(obj);
			return obj;
		}

		public void Remove(AppointmentPoll obj)
		{
			obj.Deleted = true;
		}

		public void Save()
		{
			File.WriteAllText(_fname, JsonConvert.SerializeObject(_appointmentPolls, Formatting.Indented, _settings));
		}

        public IEnumerable<AppointmentPoll> GetAll(Doctor doctor)
		{
			return GetAll().Where(ar => ar.Appointment.Doctor == doctor);
		}
    }
}
