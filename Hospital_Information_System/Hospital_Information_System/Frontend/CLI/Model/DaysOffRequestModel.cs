using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend;
using HospitalIS.Backend.Controller;
using HospitalIS.Backend.Util;

namespace HospitalIS.Frontend.CLI.Model
{
    internal abstract class DaysOffRequestModel
    {

        private const string errTooLateToSchedule =
            "You can send this request 2 or more days before the first day off, now it is too late";

        private const string hintInputStartDay = "Input date you want the break to start";
        private const string hintInputEndDay = "Input date you want the break to end";
        private const string hintInputReason = "Input reason for requesting days off";
        private const string errEndBeforeStart = "The last day off comes before the first";
        private const string errAppointmentsScheduled = "You have appointment(s) or days off scheduled during the requested break";
        private const string errNoReason = "You have to input reason";

        internal static void ReadDaysOffRequests(UserAccount user)
        {
            List<DaysOffRequest> requests =(user.Type == UserAccount.AccountType.DOCTOR)
                ? GetDoctorsDaysOffRequests(DoctorController.GetDoctorFromPerson(user.Person))
                : GetAllDaysOffRequests();

            foreach (var request in requests)
            {
                Console.WriteLine(request);
            }
        }

        private static List<DaysOffRequest> GetAllDaysOffRequests()
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted).ToList();
        }

        private static List<DaysOffRequest> GetDoctorsDaysOffRequests(Doctor doctor)
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted && a.Requester == doctor).ToList();
        }
        
        internal static void CreateDaysOffRequest(UserAccount user, string inputCancelString)
        {
            Doctor doctor = DoctorController.GetDoctorFromPerson(user.Person);
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay(inputCancelString);
                end = InputEndDay(inputCancelString, start);
                if (IsRangeCorrect(doctor, start, end)) break;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errAppointmentsScheduled);
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            string reason = InputReason(inputCancelString);
            DaysOffRequest.DaysOffRequestState state = DaysOffRequest.DaysOffRequestState.SENT;
            DaysOffRequest daysOffRequest = new DaysOffRequest(doctor, start, end, reason, state);
            IS.Instance.DaysOffRequestRepo.Add(daysOffRequest);
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
                    m => DateTime.Compare(startDay, m) <= 0,
                },
                new string[]
                {
                    errEndBeforeStart,
                },
                inputCancelString);
        }

        private static bool IsRangeCorrect(Doctor doctor, DateTime start, DateTime end)
        {
            List<Appointment> doctorsAppointments = AppointmentController.GetAppointments(doctor);
            foreach (var appointment in doctorsAppointments)
            {
                if (DateTime.Compare(start, appointment.ScheduledFor) <= 0 && //if date of the appointment is later than the start of break
                    DateTime.Compare(appointment.ScheduledFor, end) <= 0) // if date of the appointment is earlier than the end of break
                {
                    Console.WriteLine(appointment);
                    return false;
                }
            }

            DateTimeRange newRequestRange = new DateTimeRange(start, end);
            List<DaysOffRequest> oldDaysOffRequests = GetDoctorsDaysOffRequests(doctor);
            foreach (var oldRequest in oldDaysOffRequests)
            {
                DateTimeRange oldRequestRange = new DateTimeRange(oldRequest.Start, oldRequest.End);
                if (newRequestRange.Intersects(oldRequestRange))
                {
                    Console.WriteLine(oldRequestRange);
                    return false;
                }
            }

            return true;
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
    }
}