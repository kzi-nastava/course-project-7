using System;
using System.Collections.Generic;
using HospitalIS.Backend;
using HospitalIS.Backend.Controller;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class DaysOffRequestModel
    {

        private const string errTooLateToSchedule =
            "You can send this request 2 or more days before the first day off, now it is too late";

        private const string hintInputStartDay = "Input date you want the break to start";
        private const string hintInputEndDay = "Input date you want the break to end";
        private const string hintInputReason = "Input reason for requesting days off";
        private const string hintAppointmentsDeleted =
            "Following appointments of yours will be deleted due to the doctor taking an urgent break:";

        private const string hintIsRequestUrgent = "Do you want to make an urgent request?";
        private const string errEndBeforeStart = "The last day off comes before or at the same day as the first day, input last day again";
        private const string errUnableToSchedule = "You have appointment(s) or days off scheduled during the requested break";
        private const string errNoReason = "You have to input reason";
        private const string errBreakTooLong = "You can schedule break that lasts up to 5 days";

        internal static void ReadDaysOffRequests(UserAccount user)
        {
            List<DaysOffRequest> requests =(user.Type == UserAccount.AccountType.DOCTOR)
                ? DaysOffRequestController.GetDaysOffRequests(DoctorController.GetDoctorFromPerson(user.Person))
                : DaysOffRequestController.GetSentDaysOffRequests(); //for secretary

            Print(requests);
        }

        internal static void CreateDaysOffRequest(UserAccount user, string inputCancelString)
        {
            Doctor doctor = DoctorController.GetDoctorFromPerson(user.Person);
            DaysOffRequest daysOffRequest; 
            Console.WriteLine(hintIsRequestUrgent);
            if (EasyInput<bool>.YesNo(inputCancelString)) //request is urgent
            {
                daysOffRequest = CreateUrgentRequest(inputCancelString, doctor);
            }
            else //request is not urgent
            {
                daysOffRequest = CreateUnurgentRequest(inputCancelString, doctor);
            }
            
            IS.Instance.DaysOffRequestRepo.Add(daysOffRequest);
        }

        private static DaysOffRequest CreateUnurgentRequest(string inputCancelString, Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay(inputCancelString);
                end = InputEndDay(inputCancelString, start);
                if (DaysOffRequestController.IsRangeCorrect(doctor, start, end)) break;
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errUnableToSchedule);
                Console.ForegroundColor = ConsoleColor.Gray;
                AppointmentModel.Print(DaysOffRequestController.FindProblematicAppointments(doctor, start, end));
                Print(DaysOffRequestController.FindProblematicDaysOff(doctor, start, end));
            }
            var reason = InputReason(inputCancelString);
            var state = DaysOffRequest.DaysOffRequestState.SENT;
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private static DaysOffRequest CreateUrgentRequest(string inputCancelString, Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay(inputCancelString);
                end = InputEndDay(inputCancelString, start);
                if (DaysOffRequestController.IsEndDateCorrect(start, end)) break;
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errBreakTooLong);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            var reason = InputReason(inputCancelString);
            var state = DaysOffRequest.DaysOffRequestState.APPROVED;
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private static DateTime InputStartDay(string inputCancelString)
        {
            Console.WriteLine(hintInputStartDay);
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
                inputCancelString);
        }
        
        private static DateTime InputEndDay(string inputCancelString, DateTime startDay)
        {
            Console.WriteLine(hintInputEndDay);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    m => DateTime.Compare(startDay.AddDays(1), m) <= 0,
                },
                new string[]
                {
                    errEndBeforeStart,
                },
                inputCancelString);
        }

        private static string InputReason(string inputCancelString)
        {
            Console.WriteLine(hintInputReason);
            return EasyInput<string>.Get(
                new List<Func<string, bool>>
                {
                    s => s.Trim().Length > 0,
                },
                new string[]
                {
                    errNoReason,
                },
                inputCancelString);
        }

        internal static void ShowDeletedAppointments(UserAccount ua)
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

        private static void Print(List<DaysOffRequest> requests)
        {
            foreach (var request in requests)
            {
                Console.WriteLine(request);
            }
        }
    }
}