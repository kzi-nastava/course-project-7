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
		// TODO: Some of the methods belong in a service. 

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

			PrintDoctorRatings();
		}

		private void PrintDoctorRatings()
		{
			var doctorPolls = GetAppointmentPollsByDoctor();
			foreach (var doctorPollsPair in doctorPolls)
			{
				var ratingsForThisDoctor = ReduceToRatings(doctorPollsPair.Value);

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

		private Dictionary<Doctor, List<AppointmentPoll>> GetAppointmentPollsByDoctor()
		{
			var doctorPolls = new Dictionary<Doctor, List<AppointmentPoll>>();
			foreach (var poll in _appointmentPollService.GetAll())
			{
				Doctor doctor = poll.Appointment.Doctor;

				if (!doctorPolls.ContainsKey(doctor))
					doctorPolls[doctor] = new List<AppointmentPoll>();
				doctorPolls[doctor].Add(poll);
			}

			return doctorPolls;
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
			foreach (var kvp in ReduceToRatings(_hospitalPollService.GetAll()))
			{
				Print($"{kvp.Key}: {kvp.Value.Average()} ({kvp.Value.Count()} ratings)");
			}
		}

		private Dictionary<string, IList<double>> ReduceToRatings(IEnumerable<Poll> polls)
		{
			var summaryAverageCollected = new Dictionary<string, IList<double>>();
			foreach (var poll in polls)
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
