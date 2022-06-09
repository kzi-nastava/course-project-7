using System;
using System.Collections.Generic;
using System.Linq;
using HospitalIS.Backend.Util;

namespace HospitalIS.Backend.Controller
{
    public class DaysOffRequestController
    {
        public static List<DaysOffRequest> GetAllDaysOffRequests()
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted).ToList();
        }
        public static List<DaysOffRequest> GetSentDaysOffRequests()
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted && a.State == DaysOffRequest.DaysOffRequestState.SENT).ToList();
        }

        public static List<DaysOffRequest> GetDaysOffRequests(Doctor doctor)
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted && a.Requester == doctor).ToList();
        }

        private static List<DaysOffRequest> GetFutureDaysOffRequests()
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a => !a.Deleted && DateTime.Compare(DateTime.Now, a.Start) <= 0).ToList();
        }

        public static List<DaysOffRequest> GetApprovedRequests(Doctor doctor)
        {
            return IS.Instance.Hospital.DaysOffRequests.Where(a =>
                !a.Deleted && a.Requester == doctor && a.State == DaysOffRequest.DaysOffRequestState.APPROVED).ToList();
        }

        public static bool IsRangeCorrect(Doctor doctor, DateTime start, DateTime end)
        {
            var problematicAppointments = FindProblematicAppointments(doctor, start, end);

            var problematicDaysOffRequests = FindProblematicDaysOff(doctor, start, end);

            return (problematicAppointments.Count == 0 && problematicDaysOffRequests.Count == 0);
        }

        public static List<DaysOffRequest> FindProblematicDaysOff(Doctor doctor, DateTime start, DateTime end)
        {
            DateTimeRange newRequestRange = new DateTimeRange(start, end);
            List<DaysOffRequest> daysOffRequests = GetDaysOffRequests(doctor);
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

        public static List<Appointment> FindProblematicAppointments(Doctor doctor, DateTime start, DateTime end)
        {
            List<Appointment> doctorsAppointments = AppointmentController.GetAppointments(doctor);
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
        
        public static List<Appointment> GetAppointmentsToDelete(UserAccount ua)
        {
            Patient patient = PatientController.GetPatientFromPerson(ua.Person);
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

        public static bool IsEndDateCorrect(DateTime start, DateTime end)
        {
            var latestEndDay = start.AddDays(5);
            return !(DateTime.Compare(latestEndDay, end) <= 0);
        }
    }
}