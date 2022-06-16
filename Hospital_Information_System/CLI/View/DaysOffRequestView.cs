using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.CLI.View
{
    internal class DaysOffRequestView : AbstractView
    {
        private readonly IDaysOffRequestService _service;
        private readonly IDoctorService _doctorService;
        private readonly AppointmentView _appointmentView;
        
        
        private const string errTooLateToSchedule =
            "You can send this request 2 or more days before the first day off, now it is too late";

        private const string hintInputStartDay = "Input date you want the break to start";
        private const string hintInputEndDay = "Input date you want the break to end";
        private const string hintInputReason = "Input reason for requesting days off";

        private const string hintIsRequestUrgent = "Do you want to make an urgent request?";
        private const string errEndBeforeStart = "The last day off comes before or at the same day as the first day, input last day again";
        private const string errUnableToSchedule = "You have appointment(s) or days off scheduled during the requested break";
        private const string errNoReason = "You have to input reason";
        private const string errBreakTooLong = "You can schedule break that lasts up to 5 days";

        public DaysOffRequestView(IDaysOffRequestService service, IDoctorService doctorService, AppointmentView appointmentView)
        {
            _service = service;
            _doctorService = doctorService;
            _appointmentView = appointmentView;
        }

        internal void CmdRead()
        {
            var requests = _service.Get(User);
            PrintAll(requests);
        }
        
        private void PrintAll(List<DaysOffRequest> requests)
        {
            foreach (var request in requests)
            {
                Print(request.ToString());
            }
        }
        
        internal void CmdCreateDaysOffRequest()
        {
            Doctor doctor = _doctorService.GetDoctorFromPerson(User.Person);
            DaysOffRequest daysOffRequest; 
            Hint(hintIsRequestUrgent);
            if (EasyInput<bool>.YesNo(_cancel)) //request is urgent
            {
                daysOffRequest = CreateUrgentRequest(doctor);
            }
            else //request is not urgent
            {
                daysOffRequest = CreateUnurgentRequest(doctor);
            }
            
            _service.Add(daysOffRequest);
        }

        private DaysOffRequest CreateUnurgentRequest(Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay();
                end = InputEndDay(start);
                if (_service.IsRangeCorrect(doctor, start, end)) break;
                
                Error(errUnableToSchedule);
                _appointmentView.Print(_service.FindProblematicAppointments(doctor, start, end));
                Print(_service.FindProblematicDaysOff(doctor, start, end));
            }
            var reason = InputReason();
            var state = DaysOffRequest.DaysOffRequestState.SENT;
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private DaysOffRequest CreateUrgentRequest(Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay();
                end = InputEndDay(start);
                if (_service.IsEndDateCorrect(start, end)) break;
                
                Error(errBreakTooLong);
            }

            var reason = InputReason();
            var state = DaysOffRequest.DaysOffRequestState.APPROVED;
            _service.DeleteProblematicAppointments(doctor, start, end);
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private DateTime InputStartDay()
        {
            Hint(hintInputStartDay);
            DateTime lastDayForScheduling = DateTime.Now.AddDays(2);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    m => DateTime.Compare(lastDayForScheduling, m) <= 0,
                },
                new string[]
                {
                    errTooLateToSchedule,
                },
                _cancel);
        }
        
        private DateTime InputEndDay(DateTime startDay)
        {
            Hint(hintInputEndDay);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    m => DateTime.Compare(startDay.AddDays(1), m) <= 0,
                },
                new string[]
                {
                    errEndBeforeStart,
                },
                _cancel);
        }

        private string InputReason()
        {
            Hint(hintInputReason);
            return EasyInput<string>.Get(
                new List<Func<string, bool>>
                {
                    s => s.Trim().Length > 0,
                },
                new string[]
                {
                    errNoReason,
                },
                _cancel);
        }

        /*
        internal void ShowDeletedAppointments(UserAccount ua)
        {
            var appointmentsToDelete = DaysOffRequestController.GetAppointmentsToDelete(ua);
            if (appointmentsToDelete.Count == 0) return;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(hintAppointmentsDeleted);
            Console.ForegroundColor = ConsoleColor.Gray;
            foreach (var appointment in appointmentsToDelete)
            {
                Console.WriteLine(appointment);
                UserAccount doctorsAccount = DoctorController.GetUserFromDoctor(appointment.Doctor);
                AppointmentController.Delete(appointment, doctorsAccount);
            }
        }
        */

        private void Print(List<DaysOffRequest> requests)
        {
            foreach (var request in requests)
            {
               Print(request.ToString());
            }
        }
    }
}