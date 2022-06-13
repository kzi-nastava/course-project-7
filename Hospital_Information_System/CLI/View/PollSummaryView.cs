using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel;
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

		public PollSummaryView(IHospitalPollService hospitalPollService, IAppointmentPollService appointmentPollService, UserAccount account) : base(account)
		{
			_hospitalPollService = hospitalPollService;
			_appointmentPollService = appointmentPollService;
		}

		public void CmdHospitalPolls()
		{
			PrintSingleHospitalPoll();
			PrintHospitalAverageRatings();
			PrintAllHospitalComments();

			PrintDoctorRatings();
			PrintTop3();
		}

		private void PrintTop3()
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
			} else
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

		private void PrintDoctorRatings()
		{
			var doctorPolls = _appointmentPollService.GetAppointmentPollsByDoctor();
			foreach (var doctorPollsPair in doctorPolls)
			{
				var ratingsForThisDoctor = PollHelpers.ReduceToRatings(doctorPollsPair.Value);

				Print($"{doctorPollsPair.Key}");
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

		private void PrintSingleHospitalPoll()
		{
			Print(EasyInput<HospitalPoll>.Select(_hospitalPollService.GetAll(), poll => poll.Pollee.ToString() + ": " + poll.Comment, _cancel).ToString());
		}

		private void PrintAllHospitalComments()
		{
			foreach (var poll in _hospitalPollService.GetAll())
			{
				Print($"{poll.Comment}");
			}
		}

		private void PrintHospitalAverageRatings()
		{
			foreach (var kvp in PollHelpers.ReduceToRatings(_hospitalPollService.GetAll()))
			{
				Print($"{kvp.Key}: {kvp.Value.Average()} ({kvp.Value.Count()} ratings)");
			}
		}
	}
}
