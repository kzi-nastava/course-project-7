using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.AppointmentModel;
using HIS.Core.ModificationRequestModel;
using HIS.Core.ModificationRequestModel.DeleteRequestModel;
using HIS.Core.ModificationRequestModel.UpdateRequestModel;


namespace HIS.CLI.View
{
    internal class RequestView : AbstractView
    {
        private readonly IDeleteRequestService _deleteRequestService;
        private readonly IUpdateRequestService _updateRequestService;
        private readonly IAppointmentService _appointmentService;
        
        private const string hintSelectRequests = "Select requests by their number, separated by whitespace.\nEnter a newline to finish";
        private const string hintSelectAction = "Select action over requests";
        private const string hintUpdateRequests = "Update requests";
        private const string hintDeleteRequests = "Delete requests";

        public RequestView(IDeleteRequestService deleteRequestService, IUpdateRequestService updateRequestService, IAppointmentService appointmentService)
        {
            _deleteRequestService = deleteRequestService;
            _updateRequestService = updateRequestService;
            _appointmentService = appointmentService;
        }

        internal void CmdRead()
        {
            try
            {
                Print(hintUpdateRequests);
                foreach (var request in _updateRequestService.GetAll())
                {
                    Print(request.ToString());
                }

                Print(hintDeleteRequests);
                foreach (var request in _deleteRequestService.GetAll())
                {
                    Print(request.ToString());
                }
            }
            catch (NothingToSelectException e)
            {
                Error(e.Message);
            }
            
        }

        internal void CmdHandle()
        {
            try
            {
                var actions = new Dictionary<string, Action>
                {
                    ["Approve delete requests"] = () => ApproveDeleteRequest(),
                    ["Deny delete requests"] = () => DenyDeleteRequest(),
                    ["Approve update requests"] = () => ApproveUpdateRequest(),
                    ["Deny update requests"] = () => DenyUpdateRequest(),
                };
            
                Print(hintSelectAction);
                var actionChoice = EasyInput<string>.Select(actions.Keys.ToList(), _cancel);

                actions[actionChoice]();
            }
            catch (NothingToSelectException e)
            {
                Error(e.Message);
            }
        }

        internal void CmdHandleRefferals()
        {
            throw new NotImplementedException();
        }

        private void ApproveDeleteRequest()
        {
            Print(hintSelectRequests);
            
            var selectedRequestsForApproval = SelectDeleteRequests();
            foreach (var request in selectedRequestsForApproval)
            {
                _deleteRequestService.Remove(request);
                _appointmentService.Remove(_appointmentService.Get(request.Appointment.Id), User);
                request.State = ModificationRequest.StateType.APPROVED;
            }
        }
        
        private void ApproveUpdateRequest()
        {
            Print(hintSelectRequests);
            
            var selectedRequestsForApproval = SelectUpdateRequests();
            foreach (var request in selectedRequestsForApproval)
            {
                _appointmentService.Add(request.NewAppointment, User);
                request.State = ModificationRequest.StateType.APPROVED;
            }
        }
        
        private void DenyDeleteRequest()
        {
            Print(hintSelectRequests);
            
            var selectedRequestsForDenial = SelectDeleteRequests();
            foreach (var request in selectedRequestsForDenial)
            {
                request.State = ModificationRequest.StateType.DENIED;
            }
        }
        
        private void DenyUpdateRequest()
        {
            Print(hintSelectRequests);
            
            var selectedRequestsForDenial = SelectUpdateRequests();
            foreach (var request in selectedRequestsForDenial)
            {
                request.State = ModificationRequest.StateType.DENIED;
            }
        }

        private IEnumerable<DeleteRequest> SelectDeleteRequests()
        {   
            return EasyInput<DeleteRequest>.SelectMultiple(_deleteRequestService.GetPending().ToList(), r => r.Id.ToString(), _cancel).ToList();
        }
        private IEnumerable<UpdateRequest> SelectUpdateRequests()
        {   
            return EasyInput<UpdateRequest>.SelectMultiple(_updateRequestService.GetPending(), r => r.Id.ToString(), _cancel).ToList();
        }
        
        
    }
}