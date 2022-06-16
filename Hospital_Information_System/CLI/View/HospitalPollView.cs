using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel.HospitalPollModel;
using System.Collections.Generic;

namespace HIS.CLI.View
{
	internal class HospitalPollView : AbstractView
	{
		private IHospitalPollService _service;
		private IPatientService _patientService;
		private PollView _pollView;

		private const string hintComment = "Input comment";

		public HospitalPollView(IHospitalPollService service, IPatientService patientService, PollView pollView)
		{
			_service = service;
			_patientService = patientService;
			_pollView = pollView;
		}

		internal void CmdCreate()
		{
			if (User.Type != UserAccount.AccountType.PATIENT)
			{
				return;
			}

			Dictionary<string, int> questionnaire = _pollView.GenerateQuestionnaire(HospitalPollHelpers.Questions);

			Hint(hintComment);
			string comment = EasyInput<string>.Get(_cancel);

			Patient patient = _patientService.GetPatientFromPerson(User.Person);
			var poll = new HospitalPoll(questionnaire, comment, patient);

			_service.Add(poll);
		}
	}
}
