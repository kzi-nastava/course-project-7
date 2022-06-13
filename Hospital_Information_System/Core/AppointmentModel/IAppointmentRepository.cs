using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
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
		public IEnumerable<Appointment> GetPast(Patient patient);
		public Appointment Get(int id);
		public Appointment Add(Appointment obj);
		public void Remove(Appointment obj);
	}
}
