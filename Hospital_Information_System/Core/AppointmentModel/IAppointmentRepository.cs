using System;
using System.Collections.Generic;

namespace HIS.Core.AppointmentModel
{
    public interface IAppointmentRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Appointment> GetAll();
		public Appointment Get(int id);
		public Appointment Add(Appointment obj);
		public void Remove(Appointment obj);
	}
}
