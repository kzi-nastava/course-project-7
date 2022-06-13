using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel;
using HIS.Core.PollModel.AppointmentPollModel;
using HIS.Core.PollModel.HospitalPollModel;
using System.Linq;

namespace HIS.CLI.View
{
	internal class PollSummaryView : View
	{
		private IHospitalPollService _hospitalPollService;
		private IAppointmentPollService _appointmentPollService;

		public PollSummaryView(IHospitalPollService hospitalPollService, IAppointmentPollService appointmentPollService, UserAccount account) : base(account)
		{
			_hospitalPollService = hospitalPollService;
			_appointmentPollService = appointmentPollService;
		}

		public void CmdPrintDoctorTop3()
		{
			var doctorRatingsSorted = _appointmentPollService.GetTotalAverageRatingsByDoctor().OrderByDescending(kv => kv.Value).ToList();
			int topN = 3;

			if (doctorRatingsSorted.Count() <= topN)
			{
				Hint("Top 3:");
				foreach (var docRating in doctorRatingsSorted)
				{
					Print($"{docRating.Key}: {docRating.Value}");
				}
			}
			else
			{
				Hint("Top 3:");
				for (int i = 0; i < topN; i++)
				{
					var docRating = doctorRatingsSorted[i];
					Print($"{docRating.Key}: {docRating.Value}");
				}

				Hint("Bottom 3:");
				for (int i = topN - 1; i > 0 - 1; i--)
				{
					var docRating = doctorRatingsSorted[doctorRatingsSorted.Count() - 1 - i];
					Print($"{docRating.Key}: {docRating.Value}");
				}
			}
		}

		public void CmdPrintDoctorRatings()
		{
			var doctorPolls = _appointmentPollService.GetAppointmentPollsByDoctor();
			foreach (var doctorPollsPair in doctorPolls)
			{
				Print($"{doctorPollsPair.Key}");
				var ratingsForThisDoctor = PollHelpers.ReduceToRatings(doctorPollsPair.Value);
				foreach (var questionRatingPair in ratingsForThisDoctor)
				{
					Print($"\t{questionRatingPair.Key}: {questionRatingPair.Value.Average()}");
				}

				foreach (var poll in doctorPollsPair.Value)
				{
					if (poll.Comment != "")
					{
						Print($"\t{poll.Comment}");
					}
				}
			}
		}

		public void CmdPrintSingleHospitalPoll()
		{
			Print(EasyInput<HospitalPoll>.Select(_hospitalPollService.GetAll(), poll => poll.Pollee.ToString() + ": " + poll.Comment, _cancel).ToString());
		}

		public void CmdPrintAllHospitalComments()
		{
			foreach (var poll in _hospitalPollService.GetAll())
			{
				Print($"{poll.Comment}");
			}
		}

		public void CmdPrintHospitalAverageRatings()
		{
			foreach (var kvp in PollHelpers.ReduceToRatings(_hospitalPollService.GetAll()))
			{
				Print($"{kvp.Key}: {kvp.Value.Average()} ({kvp.Value.Count()} ratings)");
			}
		}
	}
}
