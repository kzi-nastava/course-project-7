using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;

namespace HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel
{
    public class DaysOffRequestService : IDaysOffRequestService
    {
        private readonly IDaysOffRequestRepository _repo;
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IUserAccountService _userAccountService;
        
        
        private const string hintAppointmentsDeleted =
            "Following appointments of yours will be deleted due to the doctor taking an urgent break:";
        
        public DaysOffRequestService(IDaysOffRequestRepository repo, IAppointmentService appointmentService, IPatientService patientService, IDoctorService doctorService, IUserAccountService userAccountService)
        {
            _repo = repo;
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
            _userAccountService = userAccountService;
        }

        public IEnumerable<DaysOffRequest> GetAll()
        {
            return _repo.GetAll();
        }
        
        public int GetNextId()
        {
            return _repo.GetNextId();
        }

        public void Save()
        {
            _repo.Save();
        }

        public DaysOffRequest Get(int id)
        {
            return _repo.Get(id);
        }

        public DaysOffRequest Add(DaysOffRequest obj)
        {
            return _repo.Add(obj);
        }

        public void Remove(DaysOffRequest obj)
        {
            _repo.Remove(obj);
        }

        public List<DaysOffRequest> GetSent()
        {
            return _repo.GetSent();
        }

        public List<DaysOffRequest> GetChanged(UserAccount user)
        {
            return _repo.GetChanged(user);
        }

        public List<DaysOffRequest> GetDaysOffRequests(Doctor doctor)
        {
            return _repo.GetDaysOffRequests(doctor);
        }

        public List<DaysOffRequest> GetFutureDaysOffRequests()
        {
            return _repo.GetFutureDaysOffRequests();
        }

        public List<DaysOffRequest> GetApprovedRequests(Doctor doctor)
        {
            return _repo.GetApprovedRequests(doctor);
        }

        public List<DaysOffRequest> GetSentAndApprovedRequests(Doctor doctor)
        {
            return _repo.GetSentAndApprovedRequests(doctor);
        }
        
        public bool IsRangeCorrect(Doctor doctor, DateTime start, DateTime end)
        {
            var problematicAppointments = FindProblematicAppointments(doctor, start, end);

            var problematicDaysOffRequests = FindProblematicDaysOff(doctor, start, end);

            return (problematicAppointments.Count == 0 && problematicDaysOffRequests.Count == 0);
        }

        public List<DaysOffRequest> FindProblematicDaysOff(Doctor doctor, DateTime start, DateTime end)
        {
            DateTimeRange newRequestRange = new DateTimeRange(start, end);
            List<DaysOffRequest> daysOffRequests = GetSentAndApprovedRequests(doctor);
            List<DaysOffRequest> problematicDaysOffRequests = new List<DaysOffRequest>();
            foreach (var request in daysOffRequests)
            {
                DateTimeRange oldRequestRange = new DateTimeRange(request.Start, request.End);
                if (newRequestRange.Intersects(oldRequestRange))
                {
                    problematicDaysOffRequests.Add(request);
                }
            }

            return problematicDaysOffRequests;
        }

        public List<Appointment> FindProblematicAppointments(Doctor doctor, DateTime start, DateTime end)
        {
            List<Appointment> doctorsAppointments = _appointmentService.GetAll(doctor).ToList();
            List<Appointment> problematicAppointments = new List<Appointment>();
            foreach (var appointment in doctorsAppointments)
            {
                //if date of the appointment is later than the start of break
                if (DateTime.Compare(start, appointment.ScheduledFor) <= 0 && 
                    // if date of the appointment is earlier than the end of break
                    DateTime.Compare(appointment.ScheduledFor, end) <= 0) 
                {
                    problematicAppointments.Add(appointment);
                }
            }

            return problematicAppointments;
        }
        
        public List<Appointment> GetAppointmentsToDelete(UserAccount ua)
        {
            Patient patient = _patientService.GetPatientFromPerson(ua.Person);
            var requests = GetFutureDaysOffRequests();
            List<Appointment> appointmentsToDelete = new List<Appointment>();
            foreach (var request in requests)
            {
                var problematicAppointments = FindProblematicAppointments(request.Requester, request.Start, request.End);
                foreach (var appointment in problematicAppointments)
                {
                    if (appointment.Patient == patient)
                    {
                        appointmentsToDelete.Add(appointment);
                    }
                }
            }

            return appointmentsToDelete;
        }

        public bool IsEndDateCorrect(DateTime start, DateTime end)
        {
            var latestEndDay = start.AddDays(5);
            return !(DateTime.Compare(latestEndDay, end) <= 0);
        }

        public List<DaysOffRequest> Get(UserAccount user)
        {
            return (user.Type == UserAccount.AccountType.DOCTOR)
                ? GetDaysOffRequests(_doctorService.GetDoctorFromPerson(user.Person))
                : GetSent(); //for secretary
        }

        public void DeleteProblematicAppointments(Doctor doctor, DateTime start, DateTime end)
        {
            var problematicAppointments = FindProblematicAppointments(doctor, start, end);
            UserAccount ua = _userAccountService.GetUserFromDoctor(doctor);
            foreach (var app in problematicAppointments)
            {
                _appointmentService.Remove(app, ua);
            }
        }
    }
}