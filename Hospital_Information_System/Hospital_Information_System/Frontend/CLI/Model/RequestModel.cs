using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using HospitalIS.Backend;
using HospitalIS.Backend.Controller;
using HospitalIS.Frontend.CLI.View;

namespace HospitalIS.Frontend.CLI.Model
{
	public class RequestModel
    {
        private const string hintSelectRequests = "Select requests by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectAction = "Select action over requests";

        internal static void HandleRequests(string inputCancelString)
        {
            try
            {
                var actions = new Dictionary<string, Action>
                {
                    ["Approve delete requests"] = () => ApproveDeleteRequest(inputCancelString),
                    ["Deny delete requests"] = () => DenyDeleteRequest(inputCancelString),
                    ["Approve update requests"] = () => ApproveUpdateRequest(inputCancelString),
                    ["Deny update requests"] = () => DenyUpdateRequest(inputCancelString),
                };
            
                Console.WriteLine(hintSelectAction);
                var actionChoice = EasyInput<string>.Select(actions.Keys.ToList(), inputCancelString);

                actions[actionChoice]();
            }
            catch (NothingToSelectException e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        internal static void ViewRequests()
        {
            Console.WriteLine("Update Requests");
            foreach (var request in IS.Instance.Hospital.UpdateRequests)
            {
                Console.WriteLine(request.ToString());
            }
            Console.WriteLine("Delete Requests");
            foreach (var request in IS.Instance.Hospital.DeleteRequests)
            {
                Console.WriteLine(request.ToString());
            }
        }

        
        private static void ApproveDeleteRequest(string inputCancelString)
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
        
        private static void ApproveUpdateRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForApproval = SelectUpdateRequests(inputCancelString);
            foreach (var request in selectedRequestsForApproval)
            {
                IS.Instance.AppointmentRepo.Add(request.NewAppointment);
                request.State = Request.StateType.APPROVED;
            }
        }

        private static void DenyDeleteRequest(string inputCancelString)
        {
            Console.WriteLine(hintSelectRequests);
            
            var selectedRequestsForDenial = SelectDeleteRequests(inputCancelString);
            foreach (var request in selectedRequestsForDenial)
            {
                request.State = Request.StateType.DENIED;
            }
        }

        private static void DenyUpdateRequest(string inputCancelString)
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

        private static List<DeleteRequest> GetPendingDeleteRequests()
        {
            return IS.Instance.Hospital.DeleteRequests.Where(request => !request.Deleted && IsModifiableDeleteRequests(request)).ToList();
        }
        
        private static List<UpdateRequest> SelectUpdateRequests(string inputCancelString)
        {   
            return EasyInput<UpdateRequest>.SelectMultiple(GetPendingUpdateRequests(), r => r.Id.ToString(), inputCancelString).ToList();
        }

        private static List<UpdateRequest> GetPendingUpdateRequests()
        {
            return IS.Instance.Hospital.UpdateRequests.Where(request => !request.Deleted && IsModifiableUpdateRequests(request)).ToList();
        }

        private static bool IsModifiableUpdateRequests(UpdateRequest request)
        {
            return request.OldAppointment.ScheduledFor > DateTime.Now && request.State == Request.StateType.PENDING;
        }
        
        private static bool IsModifiableDeleteRequests(DeleteRequest request)
        {
            return request.Appointment.ScheduledFor > DateTime.Now && request.State == Request.StateType.PENDING;
        }
        
    }
}