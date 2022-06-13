using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel.AppointmentPollModel;
using HIS.Core.PollModel.HospitalPollModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.CLI.View
{
	internal class PollSummaryView : View
	{
		private IHospitalPollService _hospitalPollService;
		private IAppointmentPollService _appointmentPollService;
		private IDoctorService _doctorService;

		public PollSummaryView(IHospitalPollService hospitalPollService, IAppointmentPollService appointmentPollService, IDoctorService doctorService, UserAccount account) : base(account)
		{
			_hospitalPollService = hospitalPollService;
			_appointmentPollService = appointmentPollService;
			_doctorService = doctorService;
		}

		public void CmdHospitalPolls()
		{
			ViewHospitalPoll();
			PrintHospitalAverageRatings();
			PrintAllComments();
		}

		private void ViewHospitalPoll()
		{
			Print(EasyInput<HospitalPoll>.Select(_hospitalPollService.GetAll(), poll => poll.Pollee.ToString() + ": " + poll.Comment, _cancel).ToString());
		}

		private void PrintAllComments()
		{
			foreach (var poll in _hospitalPollService.GetAll())
			{
				Print($"{poll.Comment}");
			}
		}

		private void PrintHospitalAverageRatings()
		{
			foreach (var kvp in GetHospitalPollTransposedRatings())
			{
				Print($"{kvp.Key}: {kvp.Value.Average()} ({kvp.Value.Count()} ratings)");
			}
		}

		private Dictionary<string, IList<double>> GetHospitalPollTransposedRatings()
		{
			var summaryAverageCollected = new Dictionary<string, IList<double>>();
			foreach (var poll in _hospitalPollService.GetAll())
			{
				var questions = poll.GetQuestions();

				foreach (var q in questions)
				{
					if (!summaryAverageCollected.ContainsKey(q))
						summaryAverageCollected[q] = new List<double>();
					summaryAverageCollected[q].Add(poll.GetRating(q));
				}
			}

			return summaryAverageCollected;
		}
	}
}
