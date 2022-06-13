using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel.HospitalPollModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.CLI.View
{
    internal class HospitalPollView : View
    {
        private IHospitalPollService _service;
        private IPatientService _patientService;
        private PollView _pollView;

        private const string hintComment = "Input comment";

        public HospitalPollView(IHospitalPollService service, IPatientService patientService, PollView pollView, UserAccount user) : base(user)
        {
            _service = service;
            _patientService = patientService;
            _pollView = pollView;
        }

        internal void CreatePoll()
        {
            if (_user.Type != UserAccount.AccountType.PATIENT)
            {
                return;
            }

            Dictionary<string, int> questionnaire = _pollView.GenerateQuestionnaire(HospitalPollHelpers.Questions);

            Hint(hintComment);
            string comment = EasyInput<string>.Get(_cancel);

            Patient patient = _patientService.GetPatientFromPerson(_user.Person);
            var poll = new HospitalPoll(questionnaire, comment, patient);

            _service.Add(poll);
        }
    }
}
