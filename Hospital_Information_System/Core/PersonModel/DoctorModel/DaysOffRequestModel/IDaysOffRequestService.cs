using System;
using System.Collections.Generic;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel
{
    public interface IDaysOffRequestService
    {
        public int GetNextId();
        public void Save();
        public IEnumerable<DaysOffRequest> GetAll();
        public DaysOffRequest Get(int id);
        public DaysOffRequest Add(DaysOffRequest obj);
        public void Remove(DaysOffRequest obj);

        public List<DaysOffRequest> GetSent();

        public List<DaysOffRequest> GetChanged(UserAccount user);

        public List<DaysOffRequest> Get(Doctor doctor);

        public List<DaysOffRequest> GetFuture();

        public List<DaysOffRequest> GetApproved(Doctor doctor);

        public List<DaysOffRequest> GetSentAndApproved(Doctor doctor);
        
        public bool IsRangeCorrect(Doctor doctor, DateTime start, DateTime end);

        public List<DaysOffRequest> FindProblematicDaysOff(Doctor doctor, DateTime start, DateTime end);

        public List<Appointment> FindProblematicAppointments(Doctor doctor, DateTime start, DateTime end);

        public List<Appointment> GetAppointmentsToDelete(UserAccount ua);

        public bool IsEndDateCorrect(DateTime start, DateTime end);

        public List<DaysOffRequest> Get(UserAccount user);

        public void DeleteProblematicAppointments(Doctor doctor, DateTime start, DateTime end);
    }
}