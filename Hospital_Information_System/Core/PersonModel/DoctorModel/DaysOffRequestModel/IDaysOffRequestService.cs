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

        public List<DaysOffRequest> GetSentDaysOffRequests();

        public List<DaysOffRequest> GetDaysOffRequests(Doctor doctor);

        public List<DaysOffRequest> GetFutureDaysOffRequests();

        public List<DaysOffRequest> GetApprovedRequests(Doctor doctor);

        public List<DaysOffRequest> GetSentAndApprovedRequests(Doctor doctor);
        
        public bool IsRangeCorrect(Doctor doctor, DateTime start, DateTime end);

        public List<DaysOffRequest> FindProblematicDaysOff(Doctor doctor, DateTime start, DateTime end);

        public List<Appointment> FindProblematicAppointments(Doctor doctor, DateTime start, DateTime end);

        public List<Appointment> GetAppointmentsToDelete(UserAccount ua);

        public bool IsEndDateCorrect(DateTime start, DateTime end);

        public List<DaysOffRequest> Get(UserAccount user);
    }
}