using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using HospitalIS.Backend;

namespace HospitalIS.Frontend.CLI.Model
{
    public class RequestModel
    {
        private const string hintSelectRequests = "Select requests by their number, separated by whitespace.\nEnter a newline to finish";

        internal static void ApproveDeleteRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForApproval = SelectDeleteRequests(inputCancelString);
            foreach (var request in selectedRequestsForApproval)
            {
                IS.Instance.DeleteRequestRepo.Remove(request);
                IS.Instance.AppointmentRepo.Remove(IS.Instance.AppointmentRepo.GetById(request.Appointment.Id));
                request.State = Request.StateType.APPROVED;
            }
        }
        
        internal static void ApproveUpdateRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForApproval = SelectUpdateRequests(inputCancelString);
            foreach (var request in selectedRequestsForApproval)
            {
                IS.Instance.AppointmentRepo.Add(request.NewAppointment);
                request.State = Request.StateType.APPROVED;
            }
        }

        internal static void DenyDeleteRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForDenial = SelectDeleteRequests(inputCancelString);
            foreach (var request in selectedRequestsForDenial)
            {
                request.State = Request.StateType.DENIED;
            }
        }

        internal static void DenyUpdateRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForDenial = SelectUpdateRequests(inputCancelString);
            foreach (var request in selectedRequestsForDenial)
            {
                request.State = Request.StateType.DENIED;
            }
        }
        
        private static List<DeleteRequest> SelectDeleteRequests(string inputCancelString)
        {   
            return EasyInput<DeleteRequest>.SelectMultiple(GetPendingDeleteRequests(), r => r.Id.ToString(), inputCancelString).ToList();
        }

        public static List<DeleteRequest> GetPendingDeleteRequests()
        {
            return IS.Instance.Hospital.DeleteRequests.Where(request => !request.Deleted && isModifiableDeleteRequests(request)).ToList();
        }
        
        private static List<UpdateRequest> SelectUpdateRequests(string inputCancelString)
        {   
            return EasyInput<UpdateRequest>.SelectMultiple(GetPendingUpdateRequests(), r => r.Id.ToString(), inputCancelString).ToList();
        }

        public static List<UpdateRequest> GetPendingUpdateRequests()
        {
            return IS.Instance.Hospital.UpdateRequests.Where(request => !request.Deleted && isModifiableUpdateRequests(request)).ToList();
        }

        internal static bool isModifiableUpdateRequests(UpdateRequest request)
        {
            return request.OldAppointment.ScheduledFor > DateTime.Now && request.State == Request.StateType.PENDING;
            // return request.State == Request.StateType.PENDING;
        }
        
        internal static bool isModifiableDeleteRequests(DeleteRequest request)
        {
            return request.Appointment.ScheduledFor > DateTime.Now && request.State == Request.StateType.PENDING;
            // return request.State == Request.StateType.PENDING;
        }
        
    }
}