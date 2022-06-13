using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.AppointmentModel.Util;
using Newtonsoft.Json;
using System;
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

        public IEnumerable<Appointment> GetAll(Patient patient)
		{
			return GetAll().Where(a => a.Patient == patient);
		}

        public IEnumerable<Appointment> GetAll(Doctor doctor)
		{
			return GetAll().Where(a => a.Doctor == doctor);
		}

        public IEnumerable<Appointment> GetModifiable(UserAccount user)
		{
			if (user.Type == UserAccount.AccountType.PATIENT)
			{
				return GetAll().Where(a => a.Patient.Person == user.Person && CanModify(a, user));
			}

			if (user.Type == UserAccount.AccountType.DOCTOR)
			{
				return GetAll().Where(a => a.Doctor.Person == user.Person && CanModify(a, user));
			}
			else
			{
				return GetAll().Where(a => CanModify(a, user));
			}
		}

		// TODO: Put elsewhere?
		private bool CanModify(Appointment appointment, UserAccount user)
		{
			if (user.Type == UserAccount.AccountType.PATIENT)
			{
				TimeSpan difference = appointment.ScheduledFor - DateTime.Now;
				return difference.TotalDays >= AppointmentConstants.DaysBeforeAppointmentUnmodifiable;
			}
			else
			{
				return true;
			}
		}
	}
}
