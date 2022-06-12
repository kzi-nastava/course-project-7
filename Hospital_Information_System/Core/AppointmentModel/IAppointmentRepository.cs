using HIS.Core.DoctorModel;
using HIS.Core.PatientModel;
using HIS.Core.UserAccountModel;
using System.Collections.Generic;

namespace HIS.Core.AppointmentModel
{
    public interface IAppointmentRepository
	{
		public int GetNextId();
		public void Save();
		public IEnumerable<Appointment> GetAll();
		public IEnumerable<Appointment> GetAll(Patient patient);
		public IEnumerable<Appointment> GetAll(Doctor doctor);
		public IEnumerable<Appointment> GetModifiable(UserAccount user);
		public Appointment Get(int id);
		public Appointment Add(Appointment obj);
		public void Remove(Appointment obj);
	}
}
